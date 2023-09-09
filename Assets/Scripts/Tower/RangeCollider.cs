using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _rangeObject;
    [SerializeField] private Transform _bullet;
    private List<Transform> _enemies = new List<Transform>();
    bool _enemyFound = false;

    private float _previuosRadius;

    private void Start()
    {
        ChangeRadiusSize(); // Set radius at the beginning from the inspector
    }

    private void Update()
    {
        // Detecting Enemy in Range
        FindEnemy();

        // There are enemies nearby
        if(_enemies.Count > 0)
        {
            var towerShot = GetComponentInChildren<TowerShot>(); // get the bullet
            towerShot.enabled = true;

            if (!towerShot.gameObject.activeSelf) // the bullet is visible now
            {
                towerShot.gameObject.SetActive(true);
            }
            towerShot.Attack(_enemies[_enemies.Count - 1]);
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
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _layerMask);

        Debug.Log(hitColliders.Length);

        foreach (Collider2D hitCollider in hitColliders)
        {
            var enemyTransform = hitCollider.transform;
            _enemies.Add(enemyTransform);
        }
    }

    private void ChangeRadiusSize()
    {
        _rangeObject.localScale = new Vector3(_radius * 2, _radius * 2, _rangeObject.localScale.z);
    }
}
