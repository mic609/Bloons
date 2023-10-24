using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _maxDistance;

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
    }

    protected virtual void Update()
    {
        var move = Time.deltaTime * _bulletSpeed;
        _currentDistance += move;

        if (_target != null)
        {
            _shootingPosition = _target.position;

            transform.position = Vector3.MoveTowards(transform.position, _shootingPosition, Time.deltaTime * _bulletSpeed);
            _shootingDirection = _target.transform.position - transform.position;

            // rotation of the projectile
            var angle = Mathf.Atan2(_shootingDirection.y, _shootingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // If there is no enemy left to defeat, remain the direction
            transform.position += transform.right * _bulletSpeed * Time.deltaTime;
        }

        if (_currentDistance >= _maxDistance)
        {
            ProjectileReset();
        }
    }

    // Object has reached max distance or hit the target
    protected void ProjectileReset()
    {
        _currentDistance = 0f;
        transform.position = _startingPosition;
        gameObject.SetActive(false);
    }
}
