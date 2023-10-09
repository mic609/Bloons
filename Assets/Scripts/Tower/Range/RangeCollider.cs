// The script describes enemy detection in range of the tower

using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] protected float _radius;
    [SerializeField] private Transform _rangeObject;

    [Header("Enemy in range")]
    [SerializeField] private LayerMask _layerMask;

    protected List<Transform> _enemies;
    protected float _previuosRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    protected void FindEnemy()
    {
        _enemies.Clear(); // we are creating new list of enemies every new frame
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _layerMask);

        foreach (var hitCollider in hitColliders)
        {
            var enemyTransform = hitCollider.transform;
            _enemies.Add(enemyTransform);
        }
    }

    // We want to attack the enemy that is closest to exit
    protected Transform ChooseFirstTarget()
    {
        var biggestProgress = 0f;
        Transform targetToReturn = null;

        foreach (var enemy in _enemies)
        {
            if(enemy.GetComponent<EnemyMovement>().GetProgress() > biggestProgress)
            {
                biggestProgress = enemy.GetComponent<EnemyMovement>().GetProgress();
                targetToReturn = enemy;
            }
        }
        return targetToReturn;
    }

    public void ChangeRadiusSize()
    {
        _rangeObject.localScale = new Vector3(_radius * 2.0f, _radius * 2.0f, _rangeObject.localScale.z);
    }
}
