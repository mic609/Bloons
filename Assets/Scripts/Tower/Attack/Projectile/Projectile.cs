using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _maxDistance;

    private Transform _target;
    private Vector3 _startingPosition;
    private Vector3 _shootingDirection;
    private Vector3 _shootingPosition;
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
            _shootingPosition = _target.position;

            transform.position = Vector3.MoveTowards(transform.position, _shootingPosition, Time.deltaTime * _bulletSpeed);
            _shootingDirection = _target.transform.position - transform.position;

            // rotation of the dart
            var angle = Mathf.Atan2(_shootingDirection.y, _shootingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // If there is no enemy left to defeat, remain the direction
            transform.position = Vector3.MoveTowards(transform.position, _shootingPosition, Time.deltaTime * _bulletSpeed);
        }

        if (_currentDistance >= _maxDistance)
        {
            ProjectileReset();
        }
    }

    // Object has reached max distance or hit the target
    public void ProjectileReset()
    {
        _currentDistance = 0f;
        transform.position = _startingPosition;
        gameObject.SetActive(false);
    }
}
