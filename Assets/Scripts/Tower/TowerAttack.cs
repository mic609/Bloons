using System.Collections;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private Transform _bullet;
    private Projectile _projectile;

    [SerializeField] private float _delayTimer = 0.0f;
    private bool _isAttacking;

    private void Start()
    {
        _delayTimer = 0;
        _isAttacking = false;
        _projectile = _bullet.GetComponent<Projectile>();
    }

    public void StartAttack(Transform enemy)
    {
        Attack(enemy);
    }

    private void Attack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            _delayTimer += Time.deltaTime;
            if(_delayTimer >= _delay)
            {
                _projectile.Attack(enemy);
                _delayTimer = 0.0f;
            }
        }
        //else if (enemy == null)
        //    _isAttacking = false;
    }
}
