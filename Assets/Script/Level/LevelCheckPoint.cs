using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    public bool isOpen = false;
    public GameObject openObje;
    public GameObject closedClosed;

    public int vehicleTypeIndex;

    public void UpdateVisual()
    {
         openObje.SetActive(isOpen);
         closedClosed.SetActive(!isOpen);
    }
}

