using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _maxDistance;
    [SerializeField] protected bool _cannotPopLead;
    [SerializeField] protected int _pierce; // how many bloons can one projectile pop
    [SerializeField] protected int _damage;

    protected Transform _target;
    protected Vector3 _startingPosition;
    protected Vector3 _shootingDirection;
    protected Vector3 _shootingPosition;
    protected float _currentDistance = 0f;

    protected virtual void Awake()
    {
        _startingPosition = transform.parent.parent.position;
    }

    public virtual void Attack(Transform target)
    {
        gameObject.SetActive(true);
        transform.position = _startingPosition;
        _target = target;
        _shootingDirection = _target.transform.position - transform.position;
    }

    public virtual void Attack(Transform target, Vector3 direction)
    {
    }

    protected virtual void Update()
    {
        var move = Time.deltaTime * _bulletSpeed;
        _currentDistance += move;

        if (_target != null)
        {
            _shootingPosition = _target.position;

            transform.position += _shootingDirection * _bulletSpeed * Time.deltaTime;

            // rotation of the projectile
            var angle = Mathf.Atan2(_shootingDirection.y, _shootingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // If there is no enemy left to defeat, remain the direction
            transform.position += _shootingDirection * _bulletSpeed * Time.deltaTime;
        }

        if (_currentDistance >= _maxDistance)
        {
            ProjectileReset();
        }
    }

    // Object has reached max distance or hit the target
    protected virtual void ProjectileReset()
    {
        _currentDistance = 0f;
        transform.position = _startingPosition;

        if(gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public virtual void UpgradeBullet(UpgradeData _upgrade)
    {
    }

    // getters
    public int GetPierce()
    {
        return _pierce;
    }
}
