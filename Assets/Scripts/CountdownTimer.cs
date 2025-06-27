using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class CountdownTimer : Singleton<CountdownTimer>
{
    [Header("Timer Settings")]
    [Tooltip("Start time in seconds (e.g., 180 for 3 minutes)")]
    [SerializeField] private float startTimeInSeconds = 180f;

    [Header("UI Display")]
    [Tooltip("UI Text element to display the timer")]
    [SerializeField] private Text timerText; // Замените на TextMeshProUGUI, если используете TextMeshPro
    [Tooltip("Progress bar image component")]
    [SerializeField] private Image progressBar;

    [Header("Progress Bar Colors")]
    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color middleColor = Color.yellow;
    [SerializeField] private Color endColor = Color.red;

    [Header("Events")]
    [Tooltip("Event invoked when the timer reaches zero")]
    public UnityEvent onTimerEnd; // Сюда можно добавить методы в инспекторе

    private float currentTime;
    private bool timerIsRunning = false;

    void Start()
    {
        // Инициализация таймера при старте
        if (progressBar != null)
        {
            progressBar.type = Image.Type.Filled;
            progressBar.fillMethod = Image.FillMethod.Horizontal;
        }
    }
    public void Init()
    {
        ResetTimer();
        StartTimer();
    }

    void Update()
    {
        if (timerIsRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                TimerEnd();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Округляем до ближайшей секунды для отображения
            float displayTime = Mathf.Max(currentTime, 0f);
            TimeSpan timeSpan = TimeSpan.FromSeconds(displayTime);
            // Форматируем как ММ:СС
            timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        if (progressBar != null)
        {
            // Update progress bar fill amount
            float progress = currentTime / startTimeInSeconds;
            progressBar.fillAmount = progress;

            // Update color based on progress
            if (progress > 0.5f)
            {
                // From green to yellow (100% to 50%)
                float t = (progress - 0.5f) * 2f;
                progressBar.color = Color.Lerp(middleColor, startColor, t);
            }
            else
            {
                // From yellow to red (50% to 0%)
                float t = progress * 2f;
                progressBar.color = Color.Lerp(endColor, middleColor, t);
            }
        }
    }

    private void TimerEnd()
    {
        currentTime = 0;
        timerIsRunning = false;
        UpdateTimerDisplay(); // Обновить дисплей до 00:00
        Debug.Log("Timer finished!");
        UIManager.Instance.CompleteGame();
        // Вызываем событие
        onTimerEnd?.Invoke();
    }

    // --- Публичные методы для управления таймером ---

    /// <summary>
    /// Starts the countdown timer.
    /// </summary>
    public void StartTimer()
    {
        if (!timerIsRunning)
        {
            // Если таймер уже на нуле, сбрасываем его перед стартом
            if (currentTime <= 0)
            {
                ResetTimer();
            }
            timerIsRunning = true;
            Debug.Log("Timer started.");
        }
    }

    /// <summary>
    /// Stops the countdown timer.
    /// </summary>
    public void StopTimer()
    {
        if (timerIsRunning)
        {
            timerIsRunning = false;
            Debug.Log("Timer stopped at: " + currentTime);
        }
    }

    /// <summary>
    /// Resets the timer to the initial start time.
    /// </summary>
    public void ResetTimer()
    {
        currentTime = startTimeInSeconds;
        timerIsRunning = false; // По умолчанию сброшенный таймер не запущен
        UpdateTimerDisplay(); // Обновить отображение
        Debug.Log("Timer reset.");
    }

    /// <summary>
    /// Adds additional time to the current countdown.
    /// </summary>
    /// <param name="secondsToAdd">Seconds to add.</param>
    public void AddTime(float secondsToAdd)
    {
        currentTime += secondsToAdd;
        UpdateTimerDisplay(); // Обновить отображение
        Debug.Log("Added time: " + secondsToAdd);
    }
}
