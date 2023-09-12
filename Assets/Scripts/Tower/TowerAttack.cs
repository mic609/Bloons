// The script describes the tower attack logic

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TowerAttack : MonoBehaviour
{
    [Header("Attack Details")]
    [SerializeField] private float _delay;

    [Header("Projectile Details")]
    [SerializeField] private Transform _parent; // where the objects need to be instantiated
    [SerializeField] private GameObject _objectToPool;
    [SerializeField] private List<GameObject> _bullets;
    [SerializeField] private int _amountToPool;

    private Projectile _projectile;
    private float _delayTimer;

    private void Start()
    {
        _delayTimer = _delay;

        // Object Pooling
        InitializeBullets();
    }

    // When enemy in range
    public void StartAttack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            _delayTimer += Time.deltaTime;
            if (_delayTimer >= _delay)
            {
                _projectile = GetPooledObject().GetComponent<Projectile>();
                _projectile.Attack(enemy);
                _delayTimer = 0.0f;
            }
        }
    }

    private void InitializeBullets()
    {
        _bullets = new List<GameObject>();
        for (int i = 0; i < _amountToPool; i++)
        {
            var tmp = Instantiate(_objectToPool, _parent);
            tmp.SetActive(false);
            _bullets.Add(tmp);
        }
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_bullets[i].activeInHierarchy)
            {
                return _bullets[i];
            }
        }
        return null;
    }
}
