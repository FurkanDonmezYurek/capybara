using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //For levelData
    // public LevelDatabase levelDatabase;
    // public GridSystem gridSystem; // GridSystem referansı
    // public GameObject loaderPrefab;

    // private int currentLevelIndex = 0;

    //

    public GameObject[] capybaraPrefabs;
    public Color[] possibleColors;
    public int capybaraCount = 12;

    public LevelDatabase levelDatabase;
    public GridSystem gridSystem;

    private int currentLevelIndex = 0;

    private void Update()
    {
        // TODO: Dont forget to remove on latest version.
        if (Input.GetKeyDown(KeyCode.K))
        {
            CapybaraSpawnerRandom();
        }
    }

    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 >= levelDatabase.levels.Length)
        {
            Debug.Log("Next level couldnt found!");
            return;
        }
        LoadLevelByIndex(currentLevelIndex + 1);
    }

    public void LoadLevelByIndex(int index)
    {
        if (index < 0 || index >= levelDatabase.levels.Length)
        {
            Debug.Log("Invalid level index or all levels completed!");
            return;
        }

        currentLevelIndex = index;

        LevelData level = levelDatabase.levels[index];

        GameManager.Instance.ClearSeatGroupCache();

        gridSystem.SetGridParameters(level.rows, level.columns, level.groupWidth, level.groupHeight);
        gridSystem.GenerateGrid();

        foreach (var capyInfo in level.capybaras)
        {
            GameObject prefab = capybaraPrefabs.FirstOrDefault(p =>
            {
                var capy = p.GetComponent<Capybara>();
                return capy != null && capy.Type == capyInfo.type;
            });

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found for capybara type {capyInfo.type}");
                continue;
            }

            Seat seat = gridSystem.GetSeatAtPosition(capyInfo.gridPosition);

            if (seat == null || !seat.IsEmpty)
            {
                Debug.LogWarning($"Seat not valid at {capyInfo.gridPosition}");
                continue;
            }

            Capybara capy = Instantiate(prefab, seat.transform.position, Quaternion.identity)
                .GetComponent<Capybara>();

            capy.SetColor(capyInfo.color);
            if (capyInfo.isFrozen) capy.Freeze();

            capy.SitSeat(seat);
        }

        GameManager.Instance.InitializeSeatGroupsCache();
    }

    // For testing purposes
    public void CapybaraSpawnerRandom()
    {
        var allSeats = new List<Seat>(FindObjectsOfType<Seat>());
        var available = new List<Seat>(allSeats);

        int placedCount = 0;

        while (placedCount < capybaraCount && available.Count > 0)
        {
            int index = Random.Range(0, available.Count);
            Seat seat = available[index];
            available.RemoveAt(index);

            GameObject prefab = capybaraPrefabs[Random.Range(0, capybaraPrefabs.Length)];
            var capy = Instantiate(prefab, seat.transform.position, Quaternion.identity)
                .GetComponent<Capybara>();

            Color color = possibleColors[Random.Range(0, possibleColors.Length)];
            capy.SetColor(color);

            if (capy is FatCapybara fat)
            {
                Seat right = GameManager.Instance.GetNeighborSeatRight(seat);

                // Eğer sağ koltuk yoksa veya doluysa, spawn iptal edilir
                if (right == null || !seat.IsEmpty || !right.IsEmpty)
                {
                    Destroy(fat.gameObject);
                    continue;
                }

                fat.SitSeat(seat);

                // her iki koltuk da dolu olduğundan listeden çıkart
                available.Remove(right);
            }
            else
            {
                if (!seat.IsEmpty)
                {
                    Destroy(capy.gameObject);
                    continue;
                }

                capy.SitSeat(seat);
            }

            placedCount++;
        }
    }

    // public void LoadLevel(int index)
    // {
    //     if (index < 0 || index >= levelDatabase.levels.Length)
    //     {
    //         Debug.Log("Tüm seviyeler tamamlandı!");
    //         return;
    //     }

    //     currentLevelIndex = index;

    //     // Seviye verilerini GridSystem'e ilet
    //     gridSystem.levelData = levelDatabase.levels[currentLevelIndex];

    //     // Seviye oluşturma
    //     gridSystem.GenerateGrid();
    // }

    // public void LoadNextLevel()
    // {
    //     LoadLevel(currentLevelIndex + 1);
    // }

    // public LevelData GetCurrentLevel() => levelDatabase.levels[currentLevelIndex];
}
