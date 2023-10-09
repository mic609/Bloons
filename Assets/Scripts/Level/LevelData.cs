// Level details

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnEvent
{
    public GameObject enemyToSpawn;
    public float timeBetweenSpawn; // time between individual spawned bloon
    public int spawnCount;
    public float timeEndEvent; // how much time we need to wait until next level event
}

[CreateAssetMenu(menuName = "New Level")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    [TextArea] public string levelDescription;
    public List<EnemySpawnEvent> enemiesSpawnEvents;
}
