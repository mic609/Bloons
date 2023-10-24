using UnityEngine;

public class DartlingGunnerAttack : TowerAttack
{
    // Attack every time even if there is no enemy
    public void Update()
    {
        _delayTimer += Time.deltaTime;
        if (_delayTimer >= _delay)
        {
            // Attack
            _projectile = GetPooledObject().GetComponent<DartlingGunnerProjectile>();
            if (_projectile != null)
            {
                _projectile.Attack(null);
                _delayTimer = 0.0f;
            }
        }
    }
}
