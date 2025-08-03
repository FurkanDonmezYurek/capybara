using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    public float rotationSpeed = 360f; // derece/saniye
    private bool isRotatingPaused = false;
    private void OnEnable()
    {
        Events.VehiclePause += Paused;
    }
    private void OnDisable()
    {
        Events.VehiclePause -= Paused;
    }
    void Update()
    {
        if (!isRotatingPaused)
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }

    public void Paused(bool Rotat)
    {
        isRotatingPaused = Rotat;
    }
}

