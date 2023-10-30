// Spawning enemies logic, path and calculating path details

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] Transform _whereToSpawn; // position of the spawner

    [Header("Levels")]
    [SerializeField] List<LevelData> _levels; // list of all possible levels in the game
    private LevelData _currentLevel;
    private int _currentLevelIndex;
    [SerializeField] private NextLevel _nextLevel;
    [SerializeField] private Transform _bloonHolder; // object that contains all bloons
    private int _numberOfBloons; // current number of bloons on map
    private bool _levelIsFinished;

    [Header("YouWonScreen")]
    [SerializeField] private GameObject _youWonScreen;

    [Header("Level messages")]
    [SerializeField] private GameObject _messageField;

    // Points on the path
    [Header("Path")]
    [SerializeField] private GameObject[] _points;
    private static float _pathLength; // it is the global variable

    private void Awake()
    {
        _levelIsFinished = true;

        // Set the initial level of the game
        _currentLevel = _levels[0];

        // Calculating path length
        _pathLength = 0;
        CalculatePathLength();
    }

    private void Start()
    {
        // Beginning of the game message
        _messageField.GetComponent<MessageField>().ActivateMessage("GameBegin");
    }

    void Update()
    {
        _numberOfBloons = _bloonHolder.transform.childCount;
    }

    public void SpawnEnemyCoroutine()
    {
        _levelIsFinished = false;
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
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
            yield return new WaitForSeconds(0.5f);
        }

        // The level has ended
        _messageField.GetComponent<MessageField>().ActivateMessage("LevelEnd");
        PlayerStats.Instance.CashAtTheEndOfTheLevel();
        _levelIsFinished = true;

        switch (PlayerStats.Instance.GetLevelDifficulty().numberOfLevels)
        {
            case 40:
                if (_currentLevelIndex >= 40)
                {
                    _youWonScreen.SetActive(true);
                }
                break;
            case 60:
                if (_currentLevelIndex >= 60)
                {
                    _youWonScreen.SetActive(true);
                }
                break;
            case 80:
                if (_currentLevelIndex >= 80)
                {
                    _youWonScreen.SetActive(true);
                }
                break;
        }
    }

    private void CalculatePathLength()
    {
        var firstCycle = true;
        for (int i = 0; i < (_points.Length - 1); i++)
        {
            if (firstCycle)
            {
                _pathLength += Vector3.Distance(_whereToSpawn.position, _points[i].transform.position);
                firstCycle = false;
                i--;
            }
            else
            {
                _pathLength += Vector3.Distance(_points[i].transform.position, _points[i + 1].transform.position);
            }
        }
    }

    public void SwitchLevel()
    {
        if (_currentLevelIndex < (_levels.Count + 1))
        {
            _currentLevel = _levels[_currentLevelIndex];
            _currentLevelIndex++;
        }
        else
        {
            // end of the game
        }
    }

    ////////////////////////////////////////
    // Getters and setters
    ////////////////////////////////////////
    
    public Transform GetAllBloons()
    {
        // return the object that contains all of the bloons
        return _bloonHolder;
    }

    public int GetNumberOfBloons()
    {
        return _numberOfBloons;
    }

    public LevelData GetLevel()
    {
        return _currentLevel;
    }

    public GameObject[] GetPoints()
    {
        return _points;
    }

    public float GetPathLength()
    {
        return _pathLength;
    }

    public bool IsLevelFinished()
    {
        return _levelIsFinished;
    }

    public LevelData GetCurrentLevel()
    {
        return _currentLevel;
    }
}