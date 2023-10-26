using System.Collections.Generic;
using UnityEngine;

public class SpikeFactoryRange : RangeCollider
{
    private SpikeFactoryAttack _towerAttack;
    private List<Transform> _pathList;
    private Vector3 _rayDirection;
    private bool _attackAllowed = false;

    private void Start()
    {
        _pathList = new List<Transform>();
        _towerAttack = GetComponent<SpikeFactoryAttack>();
        ChangeRadiusSize(); // Set radius at the beginning from the inspector
    }

    private void Update()
    {
        // Detecting Enemy in Range
        FindPath();

        // There are enemies nearby
        if (_pathList.Count > 0)
        {
            // first target shoot
            if (_attackAllowed)
            {
                //Debug.DrawLine(transform.position, transform.position + _rayDirection, Color.red, 2.0f);
                _towerAttack.StartAttack(ChooseFirstTarget(), _rayDirection);
                _attackAllowed = false;
            }
        }

        // Changing Range Size
        if (_previuosRadius != _radius)
        {
            ChangeRadiusSize();
            _previuosRadius = _radius;
        }
    }

    private void FindPath()
    {
        _pathList.Clear();

        var randomAngleInRadians = Random.Range(0f, 2f * Mathf.PI);
        var direction = new Vector3(Mathf.Cos(randomAngleInRadians), Mathf.Sin(randomAngleInRadians), 0);

        var ray = new Ray2D(transform.position, direction * _radius);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, _radius, _layerMask);

        if (hit.collider != null && hit.collider.CompareTag("Path"))
        {
            _rayDirection = direction * _radius;
            _pathList.Add(hit.collider.transform);
            _attackAllowed = true;
        }
    }

    protected override Transform ChooseFirstTarget()
    {
        return null;
    }

    private Vector2[] GetVisibleColliderPoints(PolygonCollider2D polygonCollider)
    {
        var mainCamera = Camera.main;
        var colliderBounds = polygonCollider.bounds;
        var colliderPoints = polygonCollider.points;

        var visiblePoints = new List<Vector2>();

        foreach (Vector2 point in colliderPoints)
        {
            var worldPoint = polygonCollider.transform.TransformPoint(point);
            var viewportPoint = mainCamera.WorldToViewportPoint(worldPoint);

            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1)
            {
                visiblePoints.Add(worldPoint);
            }
        }

        return visiblePoints.ToArray();
    }
}
