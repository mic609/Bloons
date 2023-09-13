// The script describes the bullet behaviour

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
        _startingPosition = transform.position;
    }

    public void Attack(Transform target)
    {
        gameObject.SetActive(true);
        _target = target;
    }

    private void Update()
    {
        if (_target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _bulletSpeed);

            var move = Time.deltaTime * _bulletSpeed;
            _currentDistance += move;

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
            _currentDistance = 0f;
            transform.position = _startingPosition;
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            transform.position = _startingPosition;
            gameObject.SetActive(false);

            var enemy = collision.gameObject;
            Destroy(enemy);
        }
    }
}