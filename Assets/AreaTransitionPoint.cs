using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaTransitionPoint : MonoBehaviour
{
    public string newAreaName; // �rn: "Desert"
    public GameObject newVehiclePrefab; // Ge�i�te kullan�lacak ara�
    [SerializeField] private string triggerTag = "Vehicle";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        Events.VehicleUpdate?.Invoke();
    }
}
