// The script describes the dart monkey attack logic

using System.Collections.Generic;
using UnityEngine;

public class DartMonkeyAttack : TowerAttack
{
    // When enemy in range
    public void StartAttack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            _delayTimer += Time.deltaTime;
            if (_delayTimer >= _delay)
            {
                // Rotate Tower towards target
                _towerRotation.Rotate(enemy);

                // Attack
                _projectile = GetPooledObject().GetComponent<DartProjectile>();
                _projectile.Attack(enemy);
                _delayTimer = 0.0f;

                // Animation
                //_animator.SetTrigger("Attack");
            }

        }
    }
}
