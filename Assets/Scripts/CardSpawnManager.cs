using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class CardSpawnManager : MonoBehaviour
{
    [SerializeField] private CardDragController cardPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private List<Sprite> cardSprites;
    [SerializeField] private List<ColorCard> colorCards;
    private int currentSpriteIndex = 0;

    private void Start()
    {
        cardSprites = cardSprites.OrderBy(x => Random.value).ToList();
    }

    public void SpawnCards(int countCard)
    {
        if (cardSprites.Count == 0) return;
        colorCards = colorCards.OrderBy(x => Random.value).ToList();
        StartCoroutine(SpawnCardsCoroutine(countCard));
    }

    public IEnumerator SpawnCardsCoroutine(int countCard)
    {
        int indexColorCard = 0;
        // Используем только указанное количество картинок
        for (int i = 0; i < countCard && currentSpriteIndex < cardSprites.Count; i++)
        {
            var sprite = cardSprites[currentSpriteIndex];
            // Для каждой картинки создаем 4 карточки разных типов
            for (int j = 0; j < 4; j++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                yield return new WaitForSeconds(0.1f);
                CardDragController newCard = Instantiate(cardPrefab,
                    spawnPoint.position + randomOffset,
                    Quaternion.identity);

                // Инициализируем карточку
                newCard.Init(sprite, (PuzzleType)j, currentSpriteIndex, colorCards[indexColorCard]);
            }
            indexColorCard++;
            if (indexColorCard >= colorCards.Count)
            {
                indexColorCard = 0;
            }
            currentSpriteIndex++;
        }

        // Если достигли конца списка, начинаем сначала
        if (currentSpriteIndex >= cardSprites.Count)
        {
            currentSpriteIndex = 0;
            cardSprites = cardSprites.OrderBy(x => Random.value).ToList();
        }
    }
}
[System.Serializable]
public class ColorCard
{
    public Color firstColor;
    public Color secondColor;

}
