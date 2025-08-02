using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class CapybaraIdleArea : Capybara
{
    [Header("Altýn üretme süresi")]
    public float minDelay = 2f;
    public float maxDelay = 5f;

    public GameObject goldPrefab;
    public Transform spawnPoint;

    [Header("Hedef noktalar (hareket için)")]
    public Transform[] moveTargets;

    public NavMeshAgent agent;
    public CapybaraStateMachine capybaraStateMachine;
    private bool hasStartedMoving = false;
    private Vector3 targetPosition;

    private void Awake()
    {
        agent.enabled = false; // ilk baþta hareket etmesin
    }

    public void SetTarget(Vector3 pos)
    {
        targetPosition = pos;
        hasStartedMoving = true;
    }

    public void SetTargetPoints(Transform[] points)
    {
        moveTargets = points;
    }

    private void Start()
    {
        capybaraStateMachine.FonksiyonStart();
        StartCoroutine(WaitAndMove());
        StartCoroutine(GenerateGoldLoop());
    }

    private IEnumerator WaitAndMove()
    {
        float delay = Random.Range(0.3f, 1.5f);
        yield return new WaitForSeconds(delay);

        if (hasStartedMoving)
        {
            agent.enabled = true;
            MoveToTarget(targetPosition);
        }
        else if (moveTargets != null && moveTargets.Length > 0)
        {
            agent.enabled = true;
            SetNewRandomTarget();
        }
    }

    private void MoveToTarget(Vector3 destination)
    {
        agent.SetDestination(destination);

        if (capybaraStateMachine != null)
            capybaraStateMachine.SetState(capybaraStateMachine.walkState);

        StartCoroutine(WatchArrival());
    }

    private IEnumerator WatchArrival()
    {
        while (agent != null && agent.enabled && agent.remainingDistance > 0.2f)
        {
            yield return null;
        }

        if (capybaraStateMachine != null)
            capybaraStateMachine.SetState(capybaraStateMachine.idleState);

        yield return new WaitForSeconds(0.5f); // küçük bekleme

        // Yeni hedef belirleyip tekrar hareket et
        SetNewRandomTarget();
    }

    private void SetNewRandomTarget()
    {
        if (moveTargets == null || moveTargets.Length == 0)
            return;

        Transform randomTarget = moveTargets[Random.Range(0, moveTargets.Length)];
        MoveToTarget(randomTarget.position);
    }

    private IEnumerator GenerateGoldLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            SpawnGold();
        }
    }

    private void SpawnGold()
    {
        Vector3 position = spawnPoint ? spawnPoint.position : transform.position;
        GameObject coin = Instantiate(goldPrefab, position, Quaternion.identity);

        coin.transform.DOMoveY(position.y + 1f, 1.2f).SetEase(Ease.OutCubic);
        coin.transform.DOScale(Vector3.zero, 1.2f).SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(coin));

        CurrencyManager.Instance.AddCoin(1);
    }
}

