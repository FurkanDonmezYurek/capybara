using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaTransitionPoint : MonoBehaviour
{
    public string newAreaName; // Örn: "Desert"
    public GameObject newVehiclePrefab; // Geçiþte kullanýlacak araç
    [SerializeField] private string triggerTag = "Vehicle";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        Events.VehicleUpdate?.Invoke();
    }
}
