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

    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _parent = GameObject.Find("BloonHolder");
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

    private void OnDestroy()
    {
        if(_enemyMovement != null)
        {
            // The bloon reached the end
            if (_enemyMovement.GetProgress() >= 1.0f)
            {
                PlayerStats.Instance.DecreaseLifeAmount(_rbe);
            }
            // The app is still running
            else if (!_isAppQuitting && _enemyMovement.GetProgress() < 1.0f)
            {
                if (_weakerEnemy != null)
                {
                    SpawnWeakerLayer();
                }

                // for every bloon popped the money amount increases
                PlayerStats.Instance.AddMoneyForBloonPop();
            }
        }
    }

    private void SpawnWeakerLayer()
    {
        float distanceFromCenter = 0.0f;
        float addDistance = 0.0f;

        // In switch we need to define the distance between weaker spawned bloons
        switch (_enemyAmount)
        {
            case 1:
                {
                    distanceFromCenter = 0.0f;
                    addDistance = 0.0f;
                    break;
                }
            case 2:
                {
                    distanceFromCenter = 0.3f * (-1);
                    addDistance = 0.6f;
                    break;
                }
            case 4:
                {
                    distanceFromCenter = 1.5f * (-1);
                    addDistance = 1.0f;
                    break;
                }
        }

        // We need to know the movement direction of the bloons, so we can spawn them in proper places
        var movementDirection = gameObject.GetComponent<EnemyMovement>().GetMovementDirection();

        if (movementDirection == Vector3.zero)
            return;

        if (movementDirection == Vector3.down || movementDirection == Vector3.up)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                // Setting position
                var setPosition = Vector3.zero;
                if (movementDirection == Vector3.down)
                    setPosition = new Vector3(transform.position.x, transform.position.y - distanceFromCenter, transform.position.z);
                else if(movementDirection == Vector3.up)
                    setPosition = new Vector3(transform.position.x, transform.position.y + distanceFromCenter, transform.position.z);

                // Instantiating new bloon
                var newBloon = Instantiate(_weakerEnemy, setPosition, transform.rotation);
                var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
                var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

                var oldEnemyDistance = oldEnemyMovement.GetCurrentDistance();

                // Setting distance, position and progress of the new bloon
                newEnemyMovement.SetCurrentDistance(oldEnemyDistance - distanceFromCenter);
                newEnemyMovement.SetCurrentPosition(setPosition);
                newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());

                distanceFromCenter += addDistance;
            }
        }
        else if(movementDirection == Vector3.right || movementDirection == Vector3.left)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                // Setting position
                var setPosition = Vector3.zero;
                if (movementDirection == Vector3.right)
                    setPosition = new Vector3(transform.position.x + distanceFromCenter, transform.position.y, transform.position.z);
                else if (movementDirection == Vector3.left)
                    setPosition = new Vector3(transform.position.x - distanceFromCenter, transform.position.y, transform.position.z);

                // Instantiating new bloon
                var newBloon = Instantiate(_weakerEnemy, setPosition, transform.rotation);
                var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
                var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

                var oldEnemyDistance = oldEnemyMovement.GetCurrentDistance();

                // Setting distance, position and progress of the new bloon
                newEnemyMovement.SetCurrentDistance(oldEnemyDistance - distanceFromCenter);
                newEnemyMovement.SetCurrentPosition(setPosition);
                newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());

                distanceFromCenter += addDistance;
            }
        }
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
