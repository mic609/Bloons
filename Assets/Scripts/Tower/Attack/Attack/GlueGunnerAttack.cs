using UnityEngine;

public class GlueGunnerAttack : TowerAttack
{
    // When enemy in range
    public void StartAttack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            _delayTimer += Time.deltaTime;
            if (_delayTimer >= _delay)
            {
                // Attack
                if (!enemy.GetComponent<BloonEffects>().HasGlueEffect() && !enemy.GetComponent<BloonController>().IsMoabClassBloon())
                {
                    // Rotate Tower towards target
                    _towerRotation.Rotate(enemy);

                    _projectile = GetPooledObject().GetComponent<GlueProjectile>();
                    _projectile.Attack(enemy);
                }
                _delayTimer = 0.0f;

                // Animation
                // _animator.SetTrigger("Attack");
            }

        }
    }
}
