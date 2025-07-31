using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class VehicleSplineController : MonoBehaviour
{
    [Header("Checkpoint Ayarları")]
    public List<LevelCheckpoint> levelCheckpoints;

    private int targetCheckpointIndex;
    private int currentCheckpointIndex = 0;

    private bool isPaused = false;

    private SplineAnimate splineAnimate;
    private VehicleManager vehicleManager;


    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        vehicleManager = GetComponent<VehicleManager>();

        targetCheckpointIndex = vehicleManager.GetCurrentLevelIndex();
        currentCheckpointIndex = 0;
        

        for (int i = 0; i < targetCheckpointIndex; i++)
        {
            levelCheckpoints[targetCheckpointIndex].isOpen = true;
            levelCheckpoints[targetCheckpointIndex].UpdateVisual();

        }

        vehicleManager.VehicleSelect(levelCheckpoints[targetCheckpointIndex].vehicleTypeIndex);

        ResumeMovement();
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

                if (currentCheckpointIndex >= targetCheckpointIndex)
                {
                    PauseMovement();
                }
                currentCheckpointIndex++;
                vehicleManager.VehicleSelect(levelCheckpoints[currentCheckpointIndex-1].vehicleTypeIndex);
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
        Camera.main.transform.DOMoveZ(transform.position.z - 20,0.25f);
    }

    private void ResumeMovement()
    {
        if (splineAnimate != null)
        {
            splineAnimate.Play();
            isPaused = false;
            Events.VehiclePause?.Invoke(isPaused);

        }
    }
}
