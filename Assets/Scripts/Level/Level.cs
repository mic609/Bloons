using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] Transform _whereToSpawn; // position of the spawner

    [Header("Levels")]
    [SerializeField] List<LevelData> _levels; // list of all possible levels in the game
    [SerializeField] private NextLevel _nextLevel;
    [SerializeField] private GameObject _bloonHolder; // object that contains all bloons
    private LevelData _currentLevel;
    private int _currentLevelIndex;
    private int _numberOfBloons; // current number of bloons on map

    [Header("Messages")]
    [SerializeField] private GameObject _messageField;

    [Header("Path")]
    [SerializeField] private Transform[] _points;
    private static float _pathLength; // it is the global variable

    private void Awake()
    {
        // Calculating path length
        CalculatePathLength();
    }

    void Update()
    {
        _numberOfBloons = _bloonHolder.transform.childCount;
    }

    public void spawnEnemyCoroutine()
    {
        StartCoroutine(spawnEnemy());
    }

    private IEnumerator spawnEnemy()
    {
        // next events
        for (int j = 0; j < _currentLevel.enemiesSpawnEvents.Count; j++)
        {
            var levelEvent = _currentLevel.enemiesSpawnEvents[j]; // current level event

            // spawning enemies in events
            for (int i = 0; i < levelEvent.spawnCount; i++)
            {
                Instantiate(levelEvent.enemyToSpawn, _whereToSpawn.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(levelEvent.timeBetweenSpawn);
            }
            yield return new WaitForSeconds(levelEvent.timeEndEvent);
        }
        while (_numberOfBloons > 0)
        {
            yield return null;
        }
        _messageField.GetComponent<MessageField>().ActivateMessage();
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

    public int GetNumberOfBloons()
    {
        return _numberOfBloons;
    }

    public void SwitchLevel()
    {
        if (_currentLevelIndex < (_levels.Count + 1))
        {
            _currentLevel = _levels[_currentLevelIndex];
            _currentLevelIndex++;
        }
    }

    public Transform[] GetPoints()
    {
        return _points;
    }

    public float GetPathLength()
    {
        return _pathLength;
    }

    public LevelData GetCurrentLevel()
    {
        return _currentLevel;
    }
}