// Tower attack details

using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Attack Details")]
    [SerializeField] protected float _delay;

    [Header("Projectile Details")]
    [SerializeField] private Transform _parent; // where the objects need to be instantiated
    [SerializeField] private GameObject _objectToPool;
    [SerializeField] protected List<GameObject> _bullets;
    [SerializeField] private int _amountToPool;

    protected Animator _animator;
    protected Projectile _projectile;
    protected TowerRotation _towerRotation;
    protected float _delayTimer;

    protected virtual void Start()
    {
        _towerRotation = GetComponentInChildren<TowerRotation>();
        _delayTimer = _delay;
        _animator = GetComponentInChildren<Animator>();

        // Object Pooling
        InitializeBullets();
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

    protected GameObject GetPooledObject()
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

    // getters and setters
    public List<GameObject> GetBullets()
    {
        return _bullets;
    }
    public void SetDelay(float delay)
    {
        _delay = delay;
    }
    public virtual void SetDamage(int damage)
    {
    }
}
