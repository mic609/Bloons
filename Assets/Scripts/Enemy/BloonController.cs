using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BloonController : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private GameObject _weakerEnemy; // weaker enemy prefab
    [SerializeField] private int _enemyAmount; // enemies count after bloon defeat

    [Header("Health")]
    [SerializeField] private int _rbe;

    [Header("Parent")]
    private GameObject _parent;

    [Header("Level")]
    [SerializeField] private GameObject _levelObject;
    private PlayerStats _playerStats;

    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _parent = GameObject.Find("BloonHolder");
        _playerStats = GameObject.Find("GameManager").GetComponent<PlayerStats>();
        gameObject.transform.SetParent(_parent.transform);
    }

    private void Update()
    {
        // When the bloon reaches the end, it is time to destroy the bloon
        if (_enemyMovement.GetProgress() >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    //public GameObject SpawnWeakerEnemy()
    //{
    //    return _weakerEnemy;
    //}

    private void OnDestroy()
    {
        // The app is still running
        if (!_isAppQuitting && _enemyMovement.GetProgress() < 1.0f)
        {
            if (_weakerEnemy != null)
            {
                SpawnWeakerLayer();
            }

            // for every bloon popped the money amount increases
            _playerStats.AddMoneyForBloonPop();
        }
        // The bloon reached the end
        if(_enemyMovement.GetProgress() >= 1.0f)
        {
            _playerStats.DecreaseLifeAmount(_rbe);
        }
    }

    private void SpawnWeakerLayer()
    {
        var newBloon = Instantiate(_weakerEnemy, transform.position, transform.rotation);
        var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
        var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

        newEnemyMovement.SetCurrentDistance(oldEnemyMovement.GetCurrentDistance());
        newEnemyMovement.SetCurrentPosition(oldEnemyMovement.GetCurrentPosition());
        newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());
        newEnemyMovement.SetProgress(oldEnemyMovement.GetProgress());
    }

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }

    public GameObject GetWeakerEnemy()
    {
        return _weakerEnemy;
    }
}
