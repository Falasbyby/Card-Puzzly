using UnityEngine;
using TMPro; // Для TextMeshPro
using DG.Tweening;
using UnityEngine.UI; // Для анимации (DOTween)

public class UIManager : Singleton<UIManager> // Используем ваш базовый класс Singleton<T>
{
    [Header("UI Elements")]
    [Tooltip("Текстовый элемент для отображения счета")]
    public Text scoreText;

    [Header("Animations")]
    [Tooltip("Длительность анимации 'подпрыгивания' счета")]
    public float scoreBounceDuration = 0.3f;
    [Tooltip("Насколько сильно увеличится текст счета во время анимации")]
    public float scoreBounceScale = 1.1f;

    public GameObject scalePanel;

    private Sequence scoreSequence; // Для управления анимацией DOTween
    private int score;

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in UIManager!");
        }
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            this.score = score;
            scoreText.text = score.ToString();
        }
    }
    public void CompleteGame()
    {
        SoundManager.Instance.PlayVictory();
        SocketSystem.Instance.SendFinishMessage(score);
    }

    public void TriggerScoreBounce()
    {
        if (scoreText != null)
        {
            if (scoreSequence != null && scoreSequence.IsActive())
            {
                scoreSequence.Kill();
            }

            scoreText.transform.localScale = Vector3.one;
            scoreSequence = DOTween.Sequence();
            scoreSequence.Append(scalePanel.transform.DOScale(scoreBounceScale, scoreBounceDuration / 2))
                         .Append(scalePanel.transform.DOScale(1f, scoreBounceDuration / 2).SetEase(Ease.OutBounce))
                         .SetUpdate(true);
        }
    }
}