using System.Collections;
using UnityEngine;

public class CapybaraSpawner : MonoBehaviour
{
    [Header("Capybara prefab�")]
    public GameObject capybaraPrefab;

    [Header("Toplam �retilecek Capybara say�s�")]
    public int totalCapybaras = 5;

    [Header("�retim s�resi")]
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    [Header("Rastgele hedef noktalar (ayn� zamanda spawn noktalar�)")]
    public Transform[] moveTargets;


    private int spawnedCount = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (spawnedCount < totalCapybaras)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnCapybara();
            spawnedCount++;
        }
    }

    private void SpawnCapybara()
    {
        if (moveTargets.Length < 2)
        {
            Debug.LogWarning("En az 2 target point gerekli (biri spawn i�in, biri hedef i�in)");
            return;
        }

        // Spawn ve hedef noktas� i�in farkl� index se�
        int spawnIndex = Random.Range(0, moveTargets.Length);
        int targetIndex;

        do
        {
            targetIndex = Random.Range(0, moveTargets.Length);
        } while (targetIndex == spawnIndex);

        Vector3 spawnPosition = moveTargets[spawnIndex].position;
        Vector3 targetPosition = moveTargets[targetIndex].position;

        GameObject capybara = Instantiate(capybaraPrefab, spawnPosition, Quaternion.identity);

        CapybaraIdleArea idleScript = capybara.GetComponent<CapybaraIdleArea>();
        if (idleScript != null)
        {
            idleScript.SetTarget(targetPosition);
            idleScript.SetTargetPoints(moveTargets); // s�rekli hareket i�in t�m noktalar� ver
        }

        Debug.Log($"Capybara �retildi. Spawn: {spawnIndex}, Target: {targetIndex}");
    }
}
