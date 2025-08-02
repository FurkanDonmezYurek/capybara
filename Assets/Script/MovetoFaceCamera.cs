using UnityEngine;

public class MoveToFaceCamera : MonoBehaviour
{
    void Start()
    {
        LookAtCamera();
    }

    public void LookAtCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        transform.LookAt(cam.transform);
    }
}

