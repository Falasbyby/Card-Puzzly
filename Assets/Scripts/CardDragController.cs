using System.Net.Mime;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class CardDragController : MonoBehaviour
{
    [SerializeField] private MeshRenderer cardMesh;
    [SerializeField] private MeshRenderer[] cardBordersMeshes;
    private Vector3 offset;
    private float zCoord;
    public bool isDragging = false;
    private Vector3 targetPosition;
    public float smoothSpeed = 10f; // Скорость плавного перемещения
    public float liftHeight = 1f; // Высота подъема при перетаскивании
    private Rigidbody rb; // Ссылка на компонент Rigidbody
    private float dragStartY; // Y-координата при начале перетаскивания
    public float rotationSpeed = 10f; // Скорость поворота
    private Vector3 startScale;
    public PointCard currentPointCard;
    public bool isPlaced = false;
    private PointCard previousPointCard;
    public PuzzleType cardType;
    public int cardIndex;
    public CardBorder[] cardBorders;
    public bool isCorrect = false;
    public bool isGrabbedActive = false;
    private Sequence scaleSequence;
    private Sequence moveSequence;

    [SerializeField] private Image[] icons;
    private Sprite cardImage;
    public bool isWin = false;
    public ColorCard colorCard;
    public void Init(Sprite cardSprite, PuzzleType type, int index, ColorCard colorCard)
    {
        cardImage = cardSprite;
        cardType = type;
        cardIndex = index;
        this.colorCard = colorCard;
        // Устанавливаем иконки в зависимости от типа
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].sprite = cardImage;
        }
        cardMesh.material.color = colorCard.firstColor;
        foreach (var border in cardBordersMeshes)
        {
            border.material.color = colorCard.secondColor;
        }

    }

    private void Start()
    {
        // Получаем компонент Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody не найден на объекте карты!");
        }
        startScale = transform.localScale;
        // Включаем все границы при старте
        EnableAllBorders();
        isGrabbedActive = true;
    }

    public void CheckScale()
    {
        isGrabbedActive = false;
        if (scaleSequence != null)
        {
            scaleSequence.Kill();
        }
        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.DOScale(isCorrect ? startScale : startScale * 0.9f, 0.3f))
            .OnComplete(() => isGrabbedActive = true);
    }

    public void ReturnToStart()
    {
        isGrabbedActive = false;

        // Проверяем, есть ли объект с тегом "ground" под картой
        RaycastHit hit;
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 10f; // Расстояние для raycast

        if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance, LayerMask.GetMask("Ground")))
        {
            Debug.Log("ground found");
            // Если найден ground, просто включаем все границы без движения
            rb.isKinematic = false;
            isPlaced = false;
            currentPointCard = null;
            isCorrect = false;
            EnableAllBorders();
            transform.SetParent(null);
            isGrabbedActive = true;
            return;

        }

        // Если ground не найден, выполняем стандартное возвращение
        if (moveSequence != null)
        {
            moveSequence.Kill();
        }
        moveSequence = DOTween.Sequence();
        moveSequence.Append(transform.DOMove(PointCardController.Instance.startPoint.transform.position +
            new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), 0.5f))
            .OnComplete(() =>
            {
                rb.isKinematic = false;
                isPlaced = false;
                currentPointCard = null;
                isCorrect = false;
                EnableAllBorders();
                transform.SetParent(null);
                isGrabbedActive = true;
            });
    }

    public void EnableAllBorders()
    {
        foreach (var border in cardBorders)
        {
            foreach (var borderObj in border.bordesOff)
            {
                borderObj.SetActive(true);
            }
        }
        CardBorder borders = cardBorders.FirstOrDefault(b => b.puzzleType == cardType);
        borders.firstIconCanvas.SetActive(true);
        borders.correctIconCanvas.SetActive(false);
    }

    public void DisableBorders()
    {
        CardBorder borders = cardBorders.FirstOrDefault(b => b.puzzleType == cardType);
        if (borders != null)
        {
            foreach (var borderObj in borders.bordesOff)
            {
                borderObj.SetActive(false);
            }
            borders.firstIconCanvas.SetActive(false);
            borders.correctIconCanvas.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        if (isWin)
        {
            return;
        }
        if (!isGrabbedActive || isDragging)
        {
            return;
        }

        if (isPlaced)
        {
            EnableAllBorders();
            currentPointCard.RemoveCard();
            isPlaced = false;
        }

        if (rb != null)
        {
            // Отключаем физику при начале перетаскивания
            rb.isKinematic = true;
        }
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
        dragStartY = transform.position.y; // Сохраняем начальную высоту
        isDragging = true;
        isGrabbedActive = false;

        if (scaleSequence != null)
        {
            scaleSequence.Kill();
        }
        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.DOScale(startScale, 0.5f));
    }

    private void OnMouseUp()
    {
        if (isWin)
        {
            return;
        }
        if (!isDragging)
        {
            return;
        }

        if (rb != null)
        {
            // Включаем физику обратно при отпускании
            rb.isKinematic = false;
        }
        isDragging = false;

        // Если карта находится над точкой пазла и точка свободна
        if (currentPointCard != null)
        {
            SoundManager.Instance.PlayCardTransition();
            currentPointCard.PlaceCard(this);
            rb.isKinematic = true;
        }
        else
        {
            ReturnToStart();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void Update()
    {
        if (isDragging)
        {
            targetPosition = GetMouseWorldPos() + offset;
            // Устанавливаем фиксированную высоту подъема от начальной позиции
            targetPosition.y = dragStartY + liftHeight;
            // Плавное перемещение к целевой позиции
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // Плавный поворот к нулевым углам
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (scaleSequence != null)
        {
            scaleSequence.Kill();
        }
        if (moveSequence != null)
        {
            moveSequence.Kill();
        }
    }
}

[System.Serializable]
public class CardBorder
{
    public PuzzleType puzzleType;
    public GameObject[] bordesOff;
    public GameObject firstIconCanvas;
    public GameObject correctIconCanvas;
}