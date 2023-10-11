// This script is used to communicate between sniper monkeys on the map

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperMonkeyManager : MonoBehaviour
{
    [SerializeField] private float _sniperWaitOnAnotherTime;

    public static SniperMonkeyManager Instance; // Singleton

    // Sniper monkeys that want to attack need to wait in queue to get the target to attack from this class
    private Queue<SniperMonkeyAttack> _sniperMonkeys = new Queue<SniperMonkeyAttack>();

    [SerializeField] private List<Transform> _attackedBloons = new List<Transform>(); // bloons that are currently being attacked by sniper monkeys
    private GameObject _bloons; // bloons on the map

    // For threads
    private object lockObject = new object();
    private bool _methodIsUsed;

    private void Awake()
    {
        _methodIsUsed = false;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _bloons = GameObject.Find("BloonHolder");
    }

    private void Update()
    {
        if(_sniperMonkeys.Count > 0)
        {
            ReturnTargetForSniperMonkey();
        }
    }

    public void AddToTheQueue(SniperMonkeyAttack sniperMonkey)
    {
        _sniperMonkeys.Enqueue(sniperMonkey);
    }

    // Decide what target should sniper monkey attack. There is a lock so multiple towers can not use this method at once
    public void ReturnTargetForSniperMonkey()
    {
        lock (lockObject)
        {
            if (_methodIsUsed)
            {
                return;
            }

            _methodIsUsed = true;
        }

        try
        {
            var towerEnemyPairs = new Dictionary<SniperMonkeyAttack, Transform>();

            for (int i = 0; i < _sniperMonkeys.Count; i++)
            {
                var sniperMonkeyCurrent = _sniperMonkeys.Dequeue();

                var biggestProgress = 0f;
                Transform targetToReturn = null;

                foreach (Transform enemy in _bloons.transform)
                {
                    // Sniper monkeys cannot attack the same target, unless the amount of bloons is smaller than number of sniper monkeys. The attacked bloon needs to be far away from the distance
                    if (enemy.GetComponent<EnemyMovement>().GetProgress() > biggestProgress && (!_attackedBloons.Contains(enemy) || _bloons.transform.childCount <= _sniperMonkeys.Count))
                    {
                        biggestProgress = enemy.GetComponent<EnemyMovement>().GetProgress();
                        targetToReturn = enemy;
                    }
                }

                // Let the sniper monkey know what to attack
                _attackedBloons.Add(targetToReturn);
                towerEnemyPairs.Add(sniperMonkeyCurrent, targetToReturn);

                // Sniper monkeys can attack three first targets in maximum
                if (_attackedBloons.Count >= 3)
                    _attackedBloons.Clear();
            }
            foreach (var towerEnemyPair in towerEnemyPairs)
            {
                var sniperMonkey = towerEnemyPair.Key;
                var target = towerEnemyPair.Value;

                if (towerEnemyPairs.TryGetValue(sniperMonkey, out target))
                    sniperMonkey.StartAttack(target);
            }
            towerEnemyPairs.Clear();
        }
        finally
        {
            lock (lockObject)
            {
                _methodIsUsed = false;
            }
        }
        
    }

}
