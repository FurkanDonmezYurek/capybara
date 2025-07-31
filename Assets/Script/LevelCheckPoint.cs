using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    public bool isOpen = false;
    public Color openColor = Color.white;
    public Color closedColor = Color.gray;
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
            visualRenderer.material.color = isOpen ? openColor : closedColor;
        }
    }
}

