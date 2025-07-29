using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ProgressManager progressManager;
    private Capybara selectedCapybara;
    List<SeatGroup> cachedSeatGroups;
    public LevelManager levelManager;
    public TimerManager timerManager;
    [SerializeField] private int startLevelIndex = 0; // For testing, change later

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitializeSeatGroupsCache();
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    private void Start()
    {
        progressManager = GetComponent<ProgressManager>();
        if (progressManager == null)
        {
            Debug.LogError("ProgressManager reference is missing in GameManager!");
            return;
        }
        levelManager = GetComponent<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LoadLevelByIndex(startLevelIndex);
            Debug.Log($"Loaded level {startLevelIndex}");
        }
        else
        {
            Debug.LogError("LevelManager reference is missing in GameManager!");
        }

        timerManager = GetComponent<TimerManager>();
        if (timerManager != null)
        {
            timerManager.OnTimerFinished += OnTimeExpired;
            timerManager.OnTimerTick += UpdateTimerUI; // Eğer UI göstereceksen
        }
        else
        {
            Debug.LogError("TimerManager reference is missing in GameManager!");
        }
    }

    public void OnTimeExpired()
    {
        CheckGameCondition();
    }

    private void UpdateTimerUI(float timeRemaining)
    {
        // UI güncellemesi için kullanılır
        // Örnek: timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    public void ShowWinScreen()
    {
        Debug.Log("You won! Show win screen here.");
        // Burada kazandığınızda gösterilecek ekranı açabilirsiniz
    }

    public void ShowLoseScreen()
    {
        Debug.Log("You lost! Show lose screen here.");
        // Burada kaybettiğinizde gösterilecek ekranı açabilirsiniz
    }

    public void CheckGameCondition()
    {
        Debug.Log("Checking game condition...");
        if (IsAllGroupsMatched())
        {
            // Game Won
            ShowWinScreen();
            progressManager.SetMaxReachedLevel(levelManager.GetCurrentLevelIndex());
            progressManager.AddSoftCurrency(100); // Örnek olarak 100 soft currency ekle
        }
        else
        {
            // Game Lost
            ShowLoseScreen();
        }
    }

    public void RestartCurrentLevel()
    {
        levelManager.RestartCurrentLevel();
    }

    public void LoadLevelByIndex(int index)
    {
        levelManager.LoadLevelByIndex(index);
    }

    public void LoadNextLevel()
    {
        levelManager.LoadNextLevel();
    }

    #region On Clicked Functions

    // TODO: Make these so that there is no material change, instead a little visual effect plays (particle or animation)
    public void OnCapybaraClicked(Capybara capybara)
    {
        if (!capybara.IsMovable())
        {
            Debug.Log("Clicked capybara is not movable!");
            return;
        }

        if (selectedCapybara != null)
        {
            selectedCapybara.GetComponent<Renderer>().material.color = selectedCapybara.color;
        }

        selectedCapybara = capybara;
        selectedCapybara.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void OnSeatClicked(Seat seat)
    {
        if (selectedCapybara == null)
            return;

        if (selectedCapybara is FatCapybara fat)
        {
            if (IsCorrectMoveFat(seat, fat.currentSlot))
            {
                fat.SitSeat(seat);
            }
        }
        else
        {
            if (IsCorrectMove(seat, selectedCapybara.currentSlot))
            {
                selectedCapybara.SitSeat(seat);
            }
        }
        selectedCapybara.GetComponent<Renderer>().material.color = selectedCapybara.color;
        selectedCapybara = null;
    }
    #endregion

    #region Move Checks
    //You can access the decision tree from Miro
    public bool IsCorrectMove(Seat seat, Seat oldSeat)
    {
        // 1. Hedef koltuk dolu mu?
        if (!seat.IsEmpty)
            return false;

        // Eğer aynı grupta ve araları boşsa geçişe izin ver
        if (seat.groupOfSeat == oldSeat.groupOfSeat && IsPathClear(oldSeat, seat))
            return true;
        // 2. Hedef koltuk koridor tarafında mı?
        if (seat.isCorridorSide)
        {
            // 2a. Kaynak koltuk koridor tarafında mı?
            if (oldSeat.isCorridorSide)
                return true;

            // 2b. Değilse → oldSeat'in grubundaki koridor koltuk boş mu?
            var oldCorridor = oldSeat.groupOfSeat.seatsInGroup.FirstOrDefault(s =>
                s.isCorridorSide && s.IsEmpty
            );

            if (oldCorridor == null)
                return false;

            // 2c. Bu koltuk oldSeat'e komşu mu?
            if (AreNeighbors(oldCorridor, oldSeat))
                return true;

            // 2d. Aradaki koltuklar boş mu?
            if (IsPathClear(oldCorridor, oldSeat))
                return true;

            return false;
        }

        // 3. Hedef koltuk koridor tarafında değilse →
        //    seat'in grubundaki koridor koltuğu boş mu?
        var seatCorridor = seat.groupOfSeat.seatsInGroup.FirstOrDefault(s =>
            s.isCorridorSide && s.IsEmpty
        );

        if (seatCorridor == null)
            return false;

        // 3a. Bu koltuk seat'e komşu mu?
        if (AreNeighbors(seatCorridor, seat))
        {
            // 3a-i. oldSeat koridor tarafında mı?
            if (oldSeat.isCorridorSide)
                return true;

            // 3a-ii. Değilse → oldSeat'in grubundaki koridor koltuk boş mu?
            var oldCorridor = oldSeat.groupOfSeat.seatsInGroup.FirstOrDefault(s =>
                s.isCorridorSide && s.IsEmpty
            );

            if (oldCorridor == null)
                return false;

            if (AreNeighbors(oldCorridor, oldSeat))
                return true;

            if (IsPathClear(oldCorridor, oldSeat))
                return true;

            return false;
        }
        else
        {
            // 3b. seatCorridor ile seat arasında boşluk var mı?
            if (!IsPathClear(seatCorridor, seat))
                return false;

            // 3b-i. oldSeat koridor tarafında mı?
            if (oldSeat.isCorridorSide)
                return true;

            // 3b-ii. Değilse → oldSeat'in grubundaki koridor koltuk boş mu?
            var oldCorridor = oldSeat.groupOfSeat.seatsInGroup.FirstOrDefault(s =>
                s.isCorridorSide && s.IsEmpty
            );

            if (oldCorridor == null)
                return false;

            if (AreNeighbors(oldCorridor, oldSeat))
                return true;

            if (IsPathClear(oldCorridor, oldSeat))
                return true;

            return false;
        }
    }

    public bool IsCorrectMoveFat(Seat targetLeft, Seat currentLeft)
    {
        // 1. Hedefin sağındaki koltuk var mı?
        Seat targetRight = GetNeighborSeatRight(targetLeft);
        if (targetRight == null || !targetLeft.IsEmpty || !targetRight.IsEmpty)
            return false;

        // 2. Kaynağın sağ koltuğu
        Seat currentRight = GetNeighborSeatRight(currentLeft);
        if (currentRight == null)
            return false;

        // 3. İki koltuğun da grup geçiş izinleri kontrol edilmeli
        bool leftMoveValid = IsCorrectMove(targetLeft, currentLeft);
        bool rightMoveValid = IsCorrectMove(targetRight, currentRight);
        Debug.Log("Fat capybara is move valid:\nLeft:" + leftMoveValid + "\nRight: " + rightMoveValid);
        return leftMoveValid || rightMoveValid;
    }

    public bool IsPathClear(Seat a, Seat b)
    {
        if (a.groupOfSeat != b.groupOfSeat)
            return false;

        if (a.gridPosition.x == b.gridPosition.x) // Aynı sütun
        {
            int minY = Mathf.Min(a.gridPosition.y, b.gridPosition.y);
            int maxY = Mathf.Max(a.gridPosition.y, b.gridPosition.y);

            for (int y = minY + 1; y < maxY; y++)
            {
                var midSeat = a.groupOfSeat.seatsInGroup.Find(s =>
                    s.gridPosition == new Vector2Int(a.gridPosition.x, y)
                );
                if (midSeat != null && !midSeat.IsEmpty)
                    return false;
            }
            return true;
        }
        else if (a.gridPosition.y == b.gridPosition.y) // Aynı satır
        {
            int minX = Mathf.Min(a.gridPosition.x, b.gridPosition.x);
            int maxX = Mathf.Max(a.gridPosition.x, b.gridPosition.x);

            for (int x = minX + 1; x < maxX; x++)
            {
                var midSeat = a.groupOfSeat.seatsInGroup.Find(s =>
                    s.gridPosition == new Vector2Int(x, a.gridPosition.y)
                );
                if (midSeat != null && !midSeat.IsEmpty)
                    return false;
            }
            return true;
        }

        return false; // Diagonal değilse false
    }
    #endregion

    #region Seat Group Cache

    // Cache seat groups for performance
    public void InitializeSeatGroupsCache()
    {
        cachedSeatGroups = new List<SeatGroup>();

        SeatGroup[] allGroups = FindObjectsOfType<SeatGroup>();

        // Index groups by positions
        Dictionary<(int x, int y), SeatGroup> groupMap = new Dictionary<(int, int), SeatGroup>();

        foreach (var group in allGroups)
        {
            cachedSeatGroups.Add(group);
            groupMap[(group.groupX, group.groupY)] = group;
            Debug.Log("Cached group: " + group.name);
        }

        // Check target's neighborhood
        foreach (var group in allGroups)
        {
            // If there is a left-hand neighbor, the first seat of this group is the corridor side.
            if (groupMap.ContainsKey((group.groupX - 1, group.groupY)))
            {
                group.seatsInGroup.First().isCorridorSide = true;
            }

            // If there is a right-hand neighbor, the last seat of this group is the corridor side.
            if (groupMap.ContainsKey((group.groupX + 1, group.groupY)))
            {
                group.seatsInGroup.Last().isCorridorSide = true;
            }
        }
    }

    public List<SeatGroup> GetCachedSeatGroups()
    {
        return cachedSeatGroups;
    }

    // DONT FORGET TO USE THIS WHEN RESETTING THE LEVEL!
    public void ClearSeatGroupCache()
    {
        cachedSeatGroups.Clear();
    }
    #endregion

    #region Helper Functions

    // True if group is locked, if not check if there is capybara in the group, if group has capybara, return false
    public bool IsAllGroupsMatched()
    {
        foreach (var group in cachedSeatGroups)
        {
            if (!group.IsGroupLocked)
            {
                if (group.seatsInGroup.Any(s => s.currentCapybara != null))
                {
                    return false; // Group has capybara but not matched
                }
            }
        }
        return true; // All groups are either locked or empty
    }

    public Seat GetRandomAvailableSeat()
    {
        List<Seat> availableSeats = GetAvailableSeatsFromCache();
        if (availableSeats == null || availableSeats.Count == 0)
            return null;

        int index = Random.Range(0, availableSeats.Count);
        return availableSeats[index];
    }

    public List<Seat> GetAvailableSeatsFromCache()
    {
        List<Seat> availableSeats = new();

        foreach (var group in cachedSeatGroups)
        {
            foreach (var seat in group.seatsInGroup)
            {
                if (seat.IsEmpty)
                    availableSeats.Add(seat);
            }
        }

        return availableSeats;
    }

    public bool AreNeighbors(Seat a, Seat b)
    {
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x)
                + Mathf.Abs(a.gridPosition.y - b.gridPosition.y)
            == 1;
    }

    public Seat GetNeighborSeatRight(Seat seat)
    {
        var group = seat.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int index = groupSeats.IndexOf(seat);

        if (index >= 0 && index < groupSeats.Count - 1)
            return groupSeats[index + 1];
        return null;
    }

    public Seat GetNeighborSeatLeft(Seat seat)
    {
        var group = seat.groupOfSeat;
        var groupSeats = group.seatsInGroup;
        int index = groupSeats.IndexOf(seat);

        if (index > 0)
            return groupSeats[index - 1];
        return null;
    }

    public Seat GetRightNeighborSlot(Seat seat)
    {
        foreach (var group in cachedSeatGroups)
        {
            var index = group.seatsInGroup.IndexOf(seat);
            if (index >= 0 && index < group.seatsInGroup.Count - 1)
            {
                return group.seatsInGroup[index + 1];
            }
        }

        return null;
    }
    #endregion
}
