using UnityEngine;
using System;
using System.Collections;

public class GameTimerManager : MonoBehaviour
{
    #region Singleton

    public static GameTimerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #endregion

    #region Fields & Properties

    [Header("Timer Settings")]
    public float totalTime = 60f;

    public float currentTime;
    public bool isRunning = false;
    public bool isFrozen = false;
    private Coroutine freezeCoroutine;

    public float RemainingTime => Mathf.Max(0f, currentTime);

    #endregion

    #region Events

    /// <summary>Progress between 0 and 1</summary>
    public event Action<float> OnTimeChanged;

    /// <summary>Invoked when timer reaches zero</summary>
    public event Action OnTimeOver;

    #endregion

    #region Timer Control Methods

    /// <summary>
    /// Starts the timer with given duration.
    /// </summary>
    public void StartTimer(float duration)
    {
        totalTime = duration;
        currentTime = duration;
        isRunning = true;
        isFrozen = false;
        OnTimeChanged?.Invoke(1f);
    }

    /// <summary>
    /// Adds time to the timer and resumes if stopped.
    /// </summary>
    public void AddTime(float seconds, bool silent = false)
    {
        currentTime += seconds;

        if (currentTime > 0f && !isRunning)
            isRunning = true;

        if (!silent)
            OnTimeChanged?.Invoke(currentTime / totalTime);
    }

    /// <summary>
    /// Freezes the timer for given real-time duration.
    /// </summary>
    public void Freeze(float duration)
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    /// <summary>
    /// Cancels any freeze immediately.
    /// </summary>
    public void CancelFreeze()
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }
        isFrozen = false;
        UIManager.Instance.SetFrozenState(false);
    }

    #endregion

    #region Coroutine

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        UIManager.Instance.SetFrozenState(true);

        yield return new WaitForSecondsRealtime(duration);

        isFrozen = false;
        UIManager.Instance.SetFrozenState(false);
        UIManager.Instance.HideBoosterFrame();
    }

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (!isRunning || isFrozen)
            return;

        currentTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(currentTime / totalTime);

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimeOver?.Invoke();
        }
    }

    #endregion
}
