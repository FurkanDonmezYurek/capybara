using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [SerializeField] private List<ParticleData> particles;

    private Dictionary<ParticleType, ParticleSystem> particleDict;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        particleDict = new Dictionary<ParticleType, ParticleSystem>();
        foreach (var data in particles)
        {
            var ps = Instantiate(data.prefab, transform);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleDict[data.type] = ps;
        }
    }

    public void Play(ParticleType type, Vector3 _position)
    {
        if (particleDict.TryGetValue(type, out var particle))
        {
            particle.transform.position = _position;
            particle.Play();
        }
    }
}

