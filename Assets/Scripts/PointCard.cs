using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PointCard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PointCardController pointCardController;
    public bool isOccupied = false;
    public CardDragController currentCard;
    public PuzzleType pointType;
    private Color defaultColor;

    public bool isWin = false;

    private void Start()
    {
        defaultColor = spriteRenderer.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isWin)
            return;

        if (other.gameObject.TryGetComponent<CardDragController>(out CardDragController cardDragController))
        {
            if (!cardDragController.isPlaced)
            {
                CloseSprite(true);
            }
            cardDragController.currentPointCard = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isWin)
            return;
        if (other.gameObject.TryGetComponent<CardDragController>(out CardDragController cardDragController))
        {
            CloseSprite(false);
            spriteRenderer.color = defaultColor;
            if (cardDragController.currentPointCard == this)
            {
                cardDragController.currentPointCard = null;
            }
        }
    }

    public void CloseSprite(bool isClose)
    {
        spriteRenderer.gameObject.SetActive(isClose);
    }

    public void PlaceCard(CardDragController card)
    {
        if (isWin)
            return;
        if (!isOccupied)
        {
            isOccupied = true;
            currentCard = card;
            card.transform.SetParent(PointCardController.Instance.pointParentCards);

            card.transform.DOMove(transform.position, 0.3f);
            card.transform.DORotateQuaternion(transform.rotation, 0.3f).OnComplete(() =>
            {
                card.isGrabbedActive = true;
            });
            card.isPlaced = true;
            if (card.cardType == pointType)
            {
                card.isCorrect = true;
                pointCardController.CheckPointCards();
            }
            card.CheckScale();
        }
        else if (isOccupied && currentCard != null)
        {
            PointCard noOccupiedPoint = pointCardController.pointCards.FirstOrDefault(p => !p.isOccupied);
            if (noOccupiedPoint != null)
            {
                currentCard.isPlaced = false;
                currentCard.currentPointCard = null;
                currentCard.isCorrect = false;
                currentCard.EnableAllBorders();
                currentCard.transform.SetParent(null);
                noOccupiedPoint.PlaceCard(currentCard);
            }
            else
            {
                currentCard.transform.SetParent(null);
                currentCard.ReturnToStart();
            }

            isOccupied = true;
            currentCard = card;
            card.transform.SetParent(PointCardController.Instance.pointParentCards);

            card.transform.DOMove(transform.position, 0.3f);
            card.transform.DORotateQuaternion(transform.rotation, 0.3f).OnComplete(() =>
            {
                card.isGrabbedActive = true;
            });
            card.isPlaced = true;
            if (card.cardType == pointType)
            {
                card.isCorrect = true;
                pointCardController.CheckPointCards();
            }
            card.CheckScale();
        }
    }

    public void RemoveCard()
    {
        
        if (isOccupied)
        {
            currentCard.isCorrect = false;
            currentCard.transform.SetParent(null);
            isOccupied = false;
            currentCard = null;
            spriteRenderer.color = defaultColor;
            pointCardController.CheckPointCards();
            CloseSprite(false);
        }
    }
}
