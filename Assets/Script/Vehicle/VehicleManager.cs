using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [Header("Araç script referanslarý")]
    public List<GameObject> vehicles;
    private int currentIndex = 0;
    private bool StartPanelActive;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (!StartPanelActive)
        {
            StartPanelActive = true;
            IdleUIManager.Instance.OpenStartLevelPanel();
        }
    }
    public void VehicleSelect(int VehicleIndex)
    {
        ShowOnly(VehicleIndex);
    }
    void ShowOnly(int indexToShow)
    {
        for (int i = 0; i < vehicles.Count; i++)
        {
            vehicles[i].SetActive(i == indexToShow);
        }
    }

    public GameObject GetCurrentVehicle()
    {
        return vehicles[currentIndex];
    }
    public int GetCurrentLevelIndex()
    {
        return PlayerPrefs.GetInt("Level", 0);
    }
}
