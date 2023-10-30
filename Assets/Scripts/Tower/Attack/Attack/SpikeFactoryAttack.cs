using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeFactoryAttack : TowerAttack
{
    // When enemy in range
    public void StartAttack(Transform pathPoint, Vector3 _rayDirection)
    {
        _delayTimer += Time.deltaTime;
        if (_delayTimer >= _delay)
        {
            Level level;
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "MapFlower")
                level = GameObject.Find("Map").GetComponent<Level>();
            else if (currentSceneName == "MapBeach")
                level = GameObject.Find("Map2").GetComponent<Level>();
            else
                level = GameObject.Find("Map").GetComponent<Level>();

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
