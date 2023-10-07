using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTowerRange : RangeCollider
{
    private BombTowerAttack _towerAttack;

    private void Start()
    {
        _enemies = new List<Transform>();
        _towerAttack = GetComponent<BombTowerAttack>();
        ChangeRadiusSize(); // Set radius at the beginning from the inspector
    }

    private void Update()
    {
        // Detecting Enemy in Range
        FindEnemy();

        // There are enemies nearby
        if (_enemies.Count > 0)
        {
            // first target shoot
            _towerAttack.StartAttack(ChooseFirstTarget());
        }

        // Changing Range Size
        if (_previuosRadius != _radius)
        {
            ChangeRadiusSize();
            _previuosRadius = _radius;
        }
    }
}
