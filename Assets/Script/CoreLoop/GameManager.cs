using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ProgressManager progressManager;
    private Capybara selectedCapybara;
    List<SeatGroup> cachedSeatGroups = new List<SeatGroup>();
    public List<Capybara> cachedCapybaraGroups;
    public LevelManager levelManager;
    public GameTimerManager timerManager;
    public GridSystem gridSystem;
    public UIManager UIManager;
    public GameTimerManager gameTimerManager;

    // [SerializeField]
    // private int startLevelIndex = 0; // For testing, change later
    private int LevelIndex = 0; // For testing, change later

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
            return;
        }
    }

    private void Start()
    {
        LevelStart();
    }

    public void LevelStart()
    {
        LevelIndex = PlayerPrefs.GetInt("Level", 0);
        levelManager.LoadLevelByIndex(LevelIndex);
        UIManager.UpdateLevel(LevelIndex);

        LevelData level = levelManager.levelDatabase.levels[LevelIndex];

        gameTimerManager.StartTimer(level.levelTime);
        Debug.Log($"Loaded level {LevelIndex}");
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
        // Burada kazandığınızda gösterilecek ekranı açabilirsiniz
        Debug.Log("You won! Show win screen here.");
        progressManager.SetMaxReachedLevel(levelManager.GetCurrentLevelIndex());
        UIManager.ShowLevelComplete();
        ParticleManager.Instance.Play(ParticleType.Confetti,new Vector3(0,5,0));
        // progressManager.AddSoftCurrency(100); // Örnek olarak 100 soft currency ekle
    }

    public void ShowLoseScreen()
    {
        Debug.Log("You lost! Show lose screen here.");
        UIManager.ShowLevelFail();
        AudioManager.Instance.PlaySFX("GameOver");
        // Burada kaybettiğinizde gösterilecek ekranı açabilirsiniz
    }

    public void CheckGameCondition()
    {
        Debug.Log("Checking game condition...");
        if (IsAllGroupsMatched())
        {
            // Game Won
            if (!UIManager.levelCompletePanel.activeSelf)
                ShowWinScreen();
            AudioManager.Instance.PlaySFX("LevelComplete");
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
        /* TODO: COMMENTED OUT- REPLACE WITH VISUAL EFFECT

        if (selectedCapybara != null)
        {
            selectedCapybara.SetColor(selectedCapybara.color); // Reset previous selection color
        }
        selectedCapybara = capybara;
        selectedCapybara.SetColor(Color.yellow); // Highlight selected capybara
        */

        if (selectedCapybara != null && selectedCapybara != capybara)
        {
            selectedCapybara.SitAnimation();
            selectedCapybara.capybaraColorMaterialObject.layer = LayerMask.NameToLayer("Default");
        }

        selectedCapybara = capybara;
        selectedCapybara.JumpAnimation();
        selectedCapybara.capybaraColorMaterialObject.layer = LayerMask.NameToLayer("Outline");
        AudioManager.Instance.PlaySFX("CapybaraClick");

        if (UIManager.Instance != null && !UIManager.Instance.seatClickedTutorial)
        {
            UIManager.Instance.MoveToCurrentSeatStep();
            UIManager.Instance.seatClickedTutorial = true;
        }
    }

    public void OnSeatClicked(Seat seat)
    {
        if (selectedCapybara == null)
            return;

        if (selectedCapybara is FatCapybara fat)
        {
            if (IsCorrectMoveFat(seat, fat.currentSlot, fat.secondSlot))
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
        //selectedCapybara.SetColor(selectedCapybara.color); // Reset color after move
        selectedCapybara.capybaraColorMaterialObject.layer = LayerMask.NameToLayer("Default");
        selectedCapybara = null;

        if (UIManager.Instance != null && UIManager.Instance.seatClickedTutorial)
        {
            UIManager.Instance.MoveToCurrentSeatStep();
            UIManager.Instance.seatClickedTutorial = false;
        }
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

    public bool IsCorrectMoveFat(Seat targetSeat, Seat currentSeat, Seat secondSeat)
    {
        // 1. CorridorSeat: currentSeat grubundaki koridor koltuğu (isCorridorSide == true)
        Seat corridorSeat = currentSeat.groupOfSeat.seatsInGroup.FirstOrDefault(s =>
            s.isCorridorSide
        );

        if (corridorSeat == null)
        {
            Debug.LogWarning("Corridor seat not found in current seat's group.");
            return false;
        }

        // 2. corridorSeat currentSeat veya secondSeat eşit mi kontrol et
        Seat oldSeat = null;
        if (corridorSeat == currentSeat || corridorSeat == secondSeat)
        {
            oldSeat = corridorSeat;
        }
        else
        {
            // corridorSeat'e en yakın olanı bul (currentSeat veya secondSeat)
            float distCurrent = Vector2Int.Distance(
                corridorSeat.gridPosition,
                currentSeat.gridPosition
            );
            float distSecond = Vector2Int.Distance(
                corridorSeat.gridPosition,
                secondSeat.gridPosition
            );

            oldSeat = distCurrent < distSecond ? currentSeat : secondSeat;
        }

        // 3. targetSeat'in sağında veya solunda boş koltuk var mı? Öncelikle koridor tarafına en uzak olanı kontrol et.

        // Sağ ve sol komşularını al
        Seat rightNeighbor = GetNeighborSeatRight(targetSeat);
        Seat leftNeighbor = GetNeighborSeatLeft(targetSeat);

        // Koridor tarafını bulmak için seat grubundaki koridor koltuğunun index'ini al
        var seatsGroup = targetSeat.groupOfSeat.seatsInGroup;
        int corridorIndex = seatsGroup.FindIndex(s => s.isCorridorSide);
        int targetIndex = seatsGroup.IndexOf(targetSeat);

        List<Seat> seatsToCheck = new List<Seat>();

        // Koridor tarafına en uzak olan koltuğu önce kontrol edeceğiz
        if (targetIndex < corridorIndex)
        {
            // Koridor sağda, o yüzden önce soldaki koltuk sonra sağdaki
            if (leftNeighbor != null && leftNeighbor.IsEmpty)
                seatsToCheck.Add(leftNeighbor);
            if (rightNeighbor != null && rightNeighbor.IsEmpty)
                seatsToCheck.Add(rightNeighbor);
        }
        else
        {
            // Koridor solda, önce sağdaki koltuk sonra soldaki
            if (rightNeighbor != null && rightNeighbor.IsEmpty)
                seatsToCheck.Add(rightNeighbor);
            if (leftNeighbor != null && leftNeighbor.IsEmpty)
                seatsToCheck.Add(leftNeighbor);
        }

        foreach (var seatToCheck in seatsToCheck)
        {
            if (IsCorrectMove(seatToCheck, oldSeat))
                return true;
        }

        return false;
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

    public void ClearCapybaraGroupCache()
    {
        foreach (var group in cachedSeatGroups)
        {
            foreach (var seat in group.seatsInGroup)
            {
                if (seat.currentCapybara != null)
                    Destroy(seat.currentCapybara.gameObject);
            }
        }
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
