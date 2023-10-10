using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueGunnerRange : RangeCollider
{
    private GlueGunnerAttack _towerAttack;

    private void Start()
    {
        _enemies = new List<Transform>();
        _towerAttack = GetComponent<GlueGunnerAttack>();
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

    protected override Transform ChooseFirstTarget()
    {
        var biggestProgress = 0f;
        Transform targetToReturn = null;

        foreach (var enemy in _enemies)
        {
            if (enemy.GetComponent<EnemyMovement>().GetProgress() > biggestProgress && !enemy.GetComponent<BloonEffects>().HasGlueEffect())
            {
                biggestProgress = enemy.GetComponent<EnemyMovement>().GetProgress();
                targetToReturn = enemy;
            }
        }

        return targetToReturn;
    }
}
