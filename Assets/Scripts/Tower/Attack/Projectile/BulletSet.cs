// Monkey Ace dart attack

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _maxDistance;
    [SerializeField] private int _bulletsAmount;
    private int _bulletCount;
    private Transform _startingPosition;

    private float _currentDistance;
    private readonly List<GameObject> _bulletList = new List<GameObject>();

    private void Start()
    {
        _startingPosition = transform.parent.parent.Find("MonkeyAceSprite");
        _currentDistance = 0;

        foreach (Transform child in transform)
        {
            _bulletList.Add(child.gameObject);
            child.gameObject.SetActive(true);
            _bulletCount++;

            if (_bulletCount == _bulletsAmount)
                break;
        }

        ActivateBullets();
    }

    private void Update()
    {
        if (_currentDistance >= _maxDistance)
        {
            RestartBullets();
        }

        foreach (var bullet in _bulletList)
        {
            var direction = bullet.transform.rotation * Vector3.up; // based on the angle
            var move = (-1) * _bulletSpeed * Time.deltaTime * direction;
            bullet.transform.position += move;
        }

        _currentDistance += (_bulletSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        _startingPosition = transform.parent.parent.Find("MonkeyAceSprite");

        gameObject.SetActive(true);
        transform.position = _startingPosition.position;
    }

    private void ActivateBullets()
    {
        gameObject.transform.position = _startingPosition.position;
    }

    public void RestartBullets()
    {
        _currentDistance = 0f;
        //gameObject.transform.position = _startingPosition.position;

        foreach(var bullet in _bulletList)
        {
            bullet.transform.position = gameObject.transform.position;
            bullet.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    public void UpgradeBullet(UpgradeData upgrade)
    {
        _bulletsAmount = upgrade.bulletsAmount;
        _bulletList.Clear();
        _bulletCount = 0;

        foreach (Transform child in transform)
        {
            _bulletList.Add(child.gameObject);
            child.gameObject.SetActive(true);
            child.gameObject.GetComponent<AceProjectile>().SetCannotPopLead(upgrade.cannotPopLead);
            child.gameObject.GetComponent<AceProjectile>().SetDamage(upgrade.damage);
            child.gameObject.GetComponent<AceProjectile>().SetPierce(upgrade.pierce);
            _bulletCount++;

            if (_bulletCount == _bulletsAmount)
                break;
        }
    }
}
