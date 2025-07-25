using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("Ara� script referanslar�")]
    public List<GameObject> vehicles;

    private int currentIndex = 0;
    private const string PREF_KEY = "SelectedVehicleIndex";

    void Start()
    {
        // Haf�zadan se�im al
        currentIndex = PlayerPrefs.GetInt(PREF_KEY, 0);
        ShowOnly(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Bir sonraki araca ge�
            currentIndex++;

            // Son ara�tan sonra ba�a d�n
            if (currentIndex >= vehicles.Count)
                currentIndex = 0;

            // Arac� g�ster ve PlayerPrefs'e yaz
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


