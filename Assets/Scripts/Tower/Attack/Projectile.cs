// The script describes the bullet behaviour

using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _maxDistance;

    private Transform _target;
    private Vector3 _startingPosition;
    private float _currentDistance = 0f;

    private void Awake()
    {
        _startingPosition = transform.parent.parent.position;
    }

    public void Attack(Transform target)
    {
        gameObject.SetActive(true);
        transform.position = _startingPosition;
        _target = target;
    }

    private void Update()
    {
        var move = Time.deltaTime * _bulletSpeed;
        _currentDistance += move;

        if (_target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _bulletSpeed);
            
            // rotation of the dart
            var dir = _target.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // If there is no enemy left to defeat, remain the direction
            transform.Translate(Vector3.right * _bulletSpeed * Time.deltaTime);
        }

        if (_currentDistance >= _maxDistance)
        {
            ProjectileReset();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Make the Projectile disappear
            ProjectileReset();

            // Destroy enemy
            var enemy = collision.gameObject;
            Destroy(enemy);

            // Add statistics for the tower
            transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp();
        }
    }

    // Object has reached max distance or hit the target
    private void ProjectileReset()
    {
        _currentDistance = 0f;
        transform.position = _startingPosition;
        gameObject.SetActive(false);
    }
}
