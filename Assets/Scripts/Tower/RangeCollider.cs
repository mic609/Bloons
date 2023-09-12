// The script describes enemy detection in range of the tower

using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float _radius;
    [SerializeField] private Transform _rangeObject;

    [Header("Enemy in range")]
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private List<Transform> _enemies;
    private TowerAttack _towerAttack;
    private float _previuosRadius;

    private void Start()
    {
        _enemies = new List<Transform>();
        _towerAttack = GetComponent<TowerAttack>();
        ChangeRadiusSize(); // Set radius at the beginning from the inspector
    }

    private void Update()
    {
        // Detecting Enemy in Range
        FindEnemy();

        // There are enemies nearby
        if(_enemies.Count > 0)
        {
            // Attack
            _towerAttack.StartAttack(_enemies[_enemies.Count - 1]);
        }

        // Changing Range Size
        if(_previuosRadius != _radius)
        {
            ChangeRadiusSize();
            _previuosRadius = _radius;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Ustaw kolor gizmos na ¿ó³ty lub inny, który wolisz
        Gizmos.DrawWireSphere(transform.position, _radius); // Narysuj okr¹g wokó³ pozycji wie¿y o okreœlonym promieniu
    }

    private void FindEnemy()
    {
        _enemies.Clear(); // we are creating new list of enemies every new frame
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _layerMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            var enemyTransform = hitCollider.transform;
            _enemies.Add(enemyTransform);
        }
    }

    private void ChangeRadiusSize()
    {
        _rangeObject.localScale = new Vector3(_radius * 2.0f, _radius * 2.0f, _rangeObject.localScale.z);
    }
}
