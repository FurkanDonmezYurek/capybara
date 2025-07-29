using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public PlayerProgressData playerData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddSoftCurrency(int amount)
    {
        playerData.softCurrency += amount;
        // TODO: Save if needed
    }

    public void AddHardCurrency(int amount)
    {
        playerData.hardCurrency += amount;
    }

    public void SetMaxReachedLevel(int levelIndex)
    {
        if (levelIndex > playerData.maxReachedLevel)
            playerData.maxReachedLevel = levelIndex;
    }

    public void IncrementAdsWatched()
    {
        playerData.adsWatched++;
    }

    public int GetSoftCurrency() => playerData.softCurrency;
    public int GetHardCurrency() => playerData.hardCurrency;
    public int GetMaxReachedLevel() => playerData.maxReachedLevel;
    public int GetAdsWatchedCount() => playerData.adsWatched;

    public void ResetProgress()
    {
        playerData.softCurrency = 0;
        playerData.hardCurrency = 0;
        playerData.maxReachedLevel = 0;
    }

    // TODO: Save/Load methods if needed
}
