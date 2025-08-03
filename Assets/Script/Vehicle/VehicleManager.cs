using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [Header("Araç script referanslarý")]
    public List<GameObject> vehicles;
    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // PC ve mobilde geçerli
        {
            Vector2 touchPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    if (!IdleUIManager.Instance.PanelActived())
                    {
                        IdleUIManager.Instance.OpenStartLevelPanel();
                    }
                }
            }
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
