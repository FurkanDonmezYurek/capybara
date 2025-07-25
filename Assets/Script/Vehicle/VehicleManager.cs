using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("Araç script referanslarý")]
    public List<GameObject> vehicles;

    private int currentIndex = 0;
    private const string PREF_KEY = "SelectedVehicleIndex";

    void Start()
    {
        // Hafýzadan seçim al
        currentIndex = PlayerPrefs.GetInt(PREF_KEY, 0);
        ShowOnly(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Bir sonraki araca geç
            currentIndex++;

            // Son araçtan sonra baþa dön
            if (currentIndex >= vehicles.Count)
                currentIndex = 0;

            // Aracý göster ve PlayerPrefs'e yaz
            ShowOnly(currentIndex);
            PlayerPrefs.SetInt(PREF_KEY, currentIndex);
        }
    }

    void ShowOnly(int indexToShow)
    {
        for (int i = 0; i < vehicles.Count; i++)
        {
            vehicles[i].SetActive(i == indexToShow);
        }
    }
}


