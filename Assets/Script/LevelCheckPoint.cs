using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    public bool isOpen = false;
    public Material openMaterial;
    public Material closedMaterial;
    [SerializeField] private MeshRenderer visualRenderer;

    public int vehicleTypeIndex;

    private void Awake()
    {
        if (visualRenderer == null)
            visualRenderer = GetComponent<MeshRenderer>();

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (visualRenderer != null)
        {
            visualRenderer.material = isOpen ? openMaterial : closedMaterial;
        }
    }
}

