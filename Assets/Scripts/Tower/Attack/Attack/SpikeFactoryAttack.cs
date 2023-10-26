using UnityEngine;

public class SpikeFactoryAttack : TowerAttack
{
    // When enemy in range
    public void StartAttack(Transform pathPoint, Vector3 _rayDirection)
    {
        _delayTimer += Time.deltaTime;
        if (_delayTimer >= _delay)
        {
            var level = GameObject.Find("Map").GetComponent<Level>();

            if (!level.IsLevelFinished())
            {
                // Attack
                _projectile = GetPooledObject().GetComponent<SpikeFactoryProjectile>();
                _projectile.Attack(pathPoint, _rayDirection);
                _delayTimer = 0.0f;
            }
        }
    }
}
