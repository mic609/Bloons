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
    private List<GameObject> _bulletList = new List<GameObject>();

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
            var move = (-1) * direction * _bulletSpeed * Time.deltaTime;
            bullet.transform.position += move;
        }

        _currentDistance += (_bulletSpeed * Time.deltaTime);
    }

    private IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(1.0f);
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
            bullet.gameObject.SetActive(true);
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
            _bulletCount++;

            if (_bulletCount == _bulletsAmount)
                break;
        }
    }
}
