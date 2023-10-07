using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTowerAttack : TowerAttack
{
    private Transform _explosion;

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
                _projectile = GetPooledObject().GetComponent<BombProjectile>();
                _projectile.Attack(enemy);
                _delayTimer = 0.0f;

                // Animation
                // _animator.SetTrigger("Attack");
            }

        }
    }

    public void ExplosionDuration(Transform bulletTransform, float explosionDiameter)
    {
        _explosion = transform.Find("Explosion");

        _explosion.position = new Vector3(bulletTransform.position.x, bulletTransform.position.y, bulletTransform.position.z);
        _explosion.localScale = new Vector3(explosionDiameter / 2.5f, explosionDiameter / 2.5f, explosionDiameter / 2.5f);

        _explosion.gameObject.SetActive(true);

        Invoke("DeactivateObject", 0.5f);
    }

    public void DeactivateObject()
    {
        _explosion.gameObject.SetActive(false);
    }
}
