using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _bulletSpeed;

    private Transform _target;
    private Vector3 _startingPosition;

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
            var direction = (_target.position - transform.position).normalized; // direction of the shot vector
            transform.Translate(direction * _bulletSpeed * Time.deltaTime); // change the position of the object using vector
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            transform.position = _startingPosition;
            gameObject.SetActive(false);
        }
    }
}
