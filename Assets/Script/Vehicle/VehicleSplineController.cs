using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public SplineAnimate splineAnimate;
    public SplineContainer splineContainer;
    public VehicleManager vehicleManager;


   void Start()
{
    if (vehicleManager == null)
    {
        Debug.LogError("VehicleManager is not assigned!");
        return;
    }

    if (levelCheckpoints == null || levelCheckpoints.Count == 0)
    {
        Debug.LogError("Level checkpoints not assigned!");
        return;
    }

    targetCheckpointIndex = vehicleManager.GetCurrentLevelIndex();
    currentCheckpointIndex = 0;

    for (int i = 0; i < targetCheckpointIndex; i++)
    {
        if (levelCheckpoints[i] != null)
        {
            levelCheckpoints[i].isOpen = true;
            levelCheckpoints[i].UpdateVisual();
        }
    }

    vehicleManager.VehicleSelect(levelCheckpoints[targetCheckpointIndex].vehicleTypeIndex);
    StartFromNearestSplinePoint(levelCheckpoints[targetCheckpointIndex].transform);
}

    public void StartFromNearestSplinePoint(Transform target)
    {
        Spline spline = splineContainer.Spline;

        float closestT = 0f;
        float closestDistance = float.MaxValue;

        // 100 adımda spline üzerindeki pozisyonları tarıyoruz
        for (float t = 0f; t <= 1f; t += 0.01f)
        {
            Vector3 point = SplineUtility.EvaluatePosition(spline, t);
            float distance = Vector3.Distance(target.position, point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestT = t;
            }
        }

        // En yakın spline pozisyonunu al
        Vector3 closestPoint = SplineUtility.EvaluatePosition(spline, closestT);
        target.position = closestPoint;

        // SplineAnimate’i o noktadan başlat
        splineAnimate.NormalizedTime = closestT;
        splineAnimate.Pause();

        Camera.main.transform.DOMove(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z - 10), 0.25f);
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
        Camera.main.transform.DOMove(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z - 10),0.25f);
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
