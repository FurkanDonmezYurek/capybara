using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public float levelTime = 60f;
    private float timeRemaining;
    private bool isTimerRunning = false;

    // Standart C# eventler
    public event Action OnTimerFinished;
    public event Action<float> OnTimerTick;

    private void Update()
    {
        if (!isTimerRunning)
            return;

        timeRemaining -= Time.deltaTime;

        // Tick event (örneğin UI'ya saniye güncellemek için)
        OnTimerTick?.Invoke(timeRemaining);

        if (timeRemaining <= 0f)
        {
            isTimerRunning = false;
            timeRemaining = 0f;
            OnTimerFinished?.Invoke(); // Süre bitti → Game Over
        }
    }

    public void StartTimer(float duration)
    {
        levelTime = duration;
        timeRemaining = duration;
        isTimerRunning = true;
    }

    public void ContinueTimer()
    {
        if (isTimerRunning)
            return;

        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void AddTime(float additionalTime)
    {
        if (isTimerRunning)
        {
            timeRemaining += additionalTime;
        }
    }

    public float GetTimeRemaining() => timeRemaining;

    public bool IsRunning() => isTimerRunning;
}
