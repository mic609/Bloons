using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] GameObject _objectToSpawn;
    [SerializeField] Transform _whereToSpawn;
    [SerializeField] private float _timeBetweenSpawn;
    [SerializeField] private int _spawnCount;

    [Header("Path")]
    [SerializeField] private Transform[] _points;
    private static float _pathLength; // it is the global variable

    private void Awake()
    {
        // Calculating path length
        CalculatePathLength();

        // Enemy Spawn on the beginning of the level
        Transform spawnObjectTransform = _objectToSpawn.transform;
        spawnObjectTransform.position = _whereToSpawn.position; // we set only position
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        for(int i = 0; i < _spawnCount; i++)
        {
            Instantiate(_objectToSpawn);
            yield return new WaitForSeconds(_timeBetweenSpawn);
        }
    }

    private void CalculatePathLength()
    {
        var firstCycle = true;
        for (int i = 0; i < (_points.Length - 1); i++)
        {
            if (firstCycle)
            {
                _pathLength += Vector3.Distance(_whereToSpawn.position, _points[i].position);
                firstCycle = false;
                i--;
            }
            else
            {
                _pathLength += Vector3.Distance(_points[i].position, _points[i + 1].position);
            }
        }
    }

    public Transform [] GetPoints()
    {
        return _points;
    }

    public float GetPathLength()
    {
        return _pathLength;
    }
}
