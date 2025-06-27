using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PointCardController : Singleton<PointCardController>
{
    public PointCard[] pointCards;
    public GameObject startPoint;
    public Transform pointParentCards;
    [SerializeField] private float bounceHeight = 0.5f;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private ParticleSystem effectWin;
    public int score;
    public void CheckPointCards()
    {
        for (int i = 0; i < pointCards.Length; i++)
        {
            if (pointCards[i].currentCard != null)
            {
                pointCards[i].currentCard.EnableAllBorders();
            }
        }

        for (int i = 0; i < pointCards.Length; i++)
        {
            if (pointCards[i].isOccupied)
            {
                CheckNeighbors(i);
            }
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool isWin = true;
        for (int i = 0; i < pointCards.Length; i++)
        {
            if (!pointCards[i].isOccupied ||
                pointCards[i].currentCard.cardType != pointCards[i].pointType ||
                !pointCards[i].currentCard.isCorrect)
            {
                isWin = false;
                break;
            }
        }

        if (isWin)
        {
            WinCard();
            score += 4;
            UIManager.Instance.UpdateScore(score);
            Debug.Log("Победа! Все карты на своих местах!");
        }
    }

    public void WinCard()
    {
        SoundManager.Instance.PlayVictory();
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {

        for (int i = 0; i < pointCards.Length; i++)
        {
            pointCards[i].isWin = true;
            pointCards[i].currentCard.isWin = true;
        }
        yield return new WaitForSeconds(0.7f);
        // Создаем последовательность анимаций для всех карт
        Sequence winSequence = DOTween.Sequence();

        // Добавляем банс для каждой карты

        Vector3 startPos = pointParentCards.transform.position;
        Vector3 startScale = pointParentCards.transform.localScale;

        // Анимация движения вверх и масштабирования
        winSequence.Append(pointParentCards.transform
            .DOMoveY(startPos.y + bounceHeight, bounceDuration / 2)
            .SetEase(Ease.OutQuad));
        winSequence.Join(pointParentCards.transform
            .DOScale(startScale * 1.2f, bounceDuration / 2)
            .SetEase(Ease.OutQuad));
        winSequence.Append(pointParentCards.transform
            .DOMoveY(startPos.y, bounceDuration / 2)
            .SetEase(Ease.InQuad));
        winSequence.Join(pointParentCards.transform
            .DOScale(startScale, bounceDuration / 2)
            .SetEase(Ease.InQuad));
        winSequence.SetLoops(1, LoopType.Yoyo);





        // Ждем завершения анимации
        yield return winSequence.Play().WaitForCompletion();
        effectWin.Play();
        SoundManager.Instance.PlayEffectConfetti();
        // Ждем дополнительное время

        // Удаляем все карты
        foreach (var point in pointCards)
        {
            if (point.currentCard != null)
            {
                Destroy(point.currentCard.gameObject);
                point.currentCard = null;
                point.isOccupied = false;
            }
        }
        for (int i = 0; i < pointCards.Length; i++)
        {
            pointCards[i].isWin = false;
        }
        yield return new WaitForSeconds(1);
        LevelManager.Instance.CheckLevel();
    }

    private void CheckNeighbors(int currentIndex)
    {
        PointCard currentPoint = pointCards[currentIndex];
        CardDragController currentCard = currentPoint.currentCard;
        if (!currentCard.isCorrect)
        {
            currentCard.EnableAllBorders();
            return;
        }
        switch (currentPoint.pointType)
        {
            case PuzzleType.LeftUp:
                CheckCard(pointCards[1], currentCard, PuzzleType.LeftDown);
                CheckCard(pointCards[2], currentCard, PuzzleType.RightUp);
                break;
            case PuzzleType.LeftDown:
                CheckCard(pointCards[0], currentCard, PuzzleType.LeftUp);
                CheckCard(pointCards[3], currentCard, PuzzleType.RightDown);
                break;
            case PuzzleType.RightUp:
                CheckCard(pointCards[0], currentCard, PuzzleType.LeftUp);
                CheckCard(pointCards[3], currentCard, PuzzleType.RightDown);
                break;
            case PuzzleType.RightDown:
                CheckCard(pointCards[1], currentCard, PuzzleType.LeftDown);
                CheckCard(pointCards[2], currentCard, PuzzleType.RightUp);
                break;
        }
    }

    private void CheckCard(PointCard neighborPoint, CardDragController currentCard, PuzzleType borderType)
    {
        if (neighborPoint.isOccupied)
        {
            CardDragController neighborCard = neighborPoint.currentCard;
            neighborPoint.CloseSprite(false);
            if (neighborCard.cardIndex == currentCard.cardIndex)
            {
                currentCard.DisableBorders();
                neighborCard.DisableBorders();
            }
            if (!neighborCard.isCorrect)
            {
                neighborCard.EnableAllBorders();
            }
            if (!currentCard.isCorrect)
            {
                currentCard.EnableAllBorders();
            }
        }
    }
}
