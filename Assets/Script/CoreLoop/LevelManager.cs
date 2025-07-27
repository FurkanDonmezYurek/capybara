using System.Collections.Generic;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CapybaraSpawnerRandom();
        }
    }

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
                Seat right = GameManager.Instance.GetRightNeighborSlot(seat);

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
