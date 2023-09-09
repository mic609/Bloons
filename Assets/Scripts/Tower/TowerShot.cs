using UnityEngine;

public class TowerShot : MonoBehaviour
{
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _delay;
    [SerializeField] private float _duration;
    [SerializeField] private float _bulletSpeed;

    public void Attack(Transform target)
    {
        gameObject.SetActive(true);
        if(target != null)
        {
            var direction = (target.position - transform.position).normalized; // direction of the shot vector
            transform.Translate(direction * _bulletSpeed * Time.deltaTime); // change the position of the object using vector
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
}
