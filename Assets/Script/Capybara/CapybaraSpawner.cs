using System.Collections;
using UnityEngine;

public class CapybaraSpawner : MonoBehaviour
{
    [Header("Capybara prefabý")]
    public GameObject capybaraPrefab;

    [Header("Toplam üretilecek Capybara sayýsý")]
    public int totalCapybaras = 5;

    [Header("Üretim süresi")]
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    [Header("Rastgele hedef noktalar (ayný zamanda spawn noktalarý)")]
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
            Debug.LogWarning("En az 2 target point gerekli (biri spawn için, biri hedef için)");
            return;
        }

        // Spawn ve hedef noktasý için farklý index seç
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
            idleScript.SetTargetPoints(moveTargets); // sürekli hareket için tüm noktalarý ver
        }

        Debug.Log($"Capybara üretildi. Spawn: {spawnIndex}, Target: {targetIndex}");
    }
}
