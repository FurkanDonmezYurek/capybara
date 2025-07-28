using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class VehicleSplineController : MonoBehaviour
{
    private SplineAnimate splineAnimate;
    private bool isPaused = false;

    void Awake()
    {
        splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeMovement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LevelCheckpoint checkpoint))
        {
            if (!checkpoint.isOpen)
            {
                checkpoint.isOpen = true;
                checkpoint.UpdateVisual();

                PauseMovement();
            }
        }
    }

    private void PauseMovement()
    {
        if (splineAnimate != null)
        {
            splineAnimate.Pause();
            isPaused = true;
            Events.VehiclePause?.Invoke(isPaused);
        }
    }

    private void ResumeMovement()
    {
        if (splineAnimate != null)
        {
            splineAnimate.Play();
            isPaused = false;
            Debug.Log("Vehicle resumed spline movement.");
            Events.VehiclePause?.Invoke(isPaused);
        }
    }
}
