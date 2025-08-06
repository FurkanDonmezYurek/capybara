using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] capybaraPrefabs;
    public Color[] possibleColors;
    public LevelDatabase levelDatabase;
    public GridSystem gridSystem;
    private int currentLevelIndex = 0;

    public int GetCurrentLevelIndex() => currentLevelIndex;

    public void RestartCurrentLevel()
    {
        LoadLevelByIndex(currentLevelIndex);
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

        GameManager.Instance.ClearCapybaraGroupCache();
        GameManager.Instance.ClearSeatGroupCache();
        // Destroy gameobjects tagged with "LevelEnvironment" in scene
        GameObject[] existingEnvironments = GameObject.FindGameObjectsWithTag("LevelEnvironment");
        foreach (GameObject env in existingEnvironments)
        {
            Destroy(env);
        }
        Instantiate(level.levelEnvironment, Vector3.zero, Quaternion.Euler(0, 180, 0)); // Rotate to face the camera

        gridSystem.ClearGrid();
        gridSystem.SetGridParameters(
            level.rows,
            level.columns,
            level.groupWidth,
            level.groupHeight,
            level.horizontalSpacing,
            level.verticalSpacing
        );
        gridSystem.GenerateGrid(); // This also sets grid position to environment grid spawn point (this is a game object with "GridSpawn" tag)
        gridSystem.FitGridToReferenceArea();
        gridSystem.InitPathGrid();
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
            if (capyInfo.isFrozen)
                capy.Freeze();

            capy.SetSeat(seat);
        }

        GameManager.Instance.InitializeSeatGroupsCache();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            gridSystem.FitGridToReferenceArea();
        }
    }
}
