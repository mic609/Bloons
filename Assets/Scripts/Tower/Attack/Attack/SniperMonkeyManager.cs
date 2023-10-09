// This script is used to communicate between sniper monkeys on the map

using System.Collections.Generic;
using UnityEngine;

public class SniperMonkeyManager : MonoBehaviour
{
    public static SniperMonkeyManager Instance; // Singleton

    // sniper monkeys that want to attack need to wait in queue to get the target to attack from this class
    private Queue<SniperMonkeyAttack> _sniperMonkeys = new Queue<SniperMonkeyAttack>();

    private List<Transform> _attackedBloons = new List<Transform>(); // bloons that are currently being attacked by sniper monkeys
    private GameObject _bloons; // bloons on the map

    private void Awake()
    {
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

    // Decide what target should sniper monkey attack
    public void ReturnTargetForSniperMonkey()
    {
        for(int i = 0; i < _sniperMonkeys.Count; i++)
        {
            var sniperMonkeyCurrent = _sniperMonkeys.Dequeue();

            var biggestProgress = 0f;
            Transform targetToReturn = null;

            foreach (Transform enemy in _bloons.transform)
            {
                // Sniper monkeys cannot attack the same target, unless the amount of bloons is smaller than number of sniper monkeys. The attacked bloon needs to be far away from the distance
                if (enemy.GetComponent<EnemyMovement>().GetProgress() > biggestProgress && (!_attackedBloons.Contains(enemy) || _bloons.transform.childCount <= 3))
                {
                    biggestProgress = enemy.GetComponent<EnemyMovement>().GetProgress();
                    targetToReturn = enemy;
                    _attackedBloons.Add(targetToReturn);
                }
            }

            // Let the sniper monkey know what to attack
            sniperMonkeyCurrent.StartAttack(targetToReturn);

            // Sniper monkeys can attack three first targets in maximum
            if (_attackedBloons.Count >= 3)
                _attackedBloons.Clear();
        }
    }
}
