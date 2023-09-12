// The script describes the bullet behaviour

using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _bulletSpeed;

    private Transform _target;
    private Vector3 _startingPosition;
    private Vector3 _direction;

    private void Awake()
    {
        _startingPosition = transform.position;
    }

    public void Attack(Transform target)
    {
        gameObject.SetActive(true);
        _target = target;
        _direction = (_target.position - transform.position).normalized; // direction of the shot vector
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = rotation;
    }

    private void Update()
    {
        transform.Translate(_direction * _bulletSpeed * Time.deltaTime); // change the position of the object using vector
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
