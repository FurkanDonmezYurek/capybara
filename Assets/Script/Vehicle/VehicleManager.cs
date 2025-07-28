using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [Header("Araç script referanslarý")]
    public List<GameObject> vehicles;

    private int currentIndex = 0;
    private const string PREF_KEY = "SelectedVehicleIndex";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt(PREF_KEY, 0);
        ShowOnly(currentIndex);
    }

    public void VehicleUpdate()
    {
        currentIndex++;

        if (currentIndex >= vehicles.Count)
            currentIndex = 0;

        ShowOnly(currentIndex);
        PlayerPrefs.SetInt(PREF_KEY, currentIndex);
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
    private void OnEnable()
    {
        Events.VehicleUpdate += VehicleUpdate;
    }
    private void OnDisable()
    {
        Events.VehicleUpdate -= VehicleUpdate;
    }
}
