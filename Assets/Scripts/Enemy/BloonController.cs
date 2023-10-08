using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class BloonController : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private bool _isMoabClassBloon;
    [SerializeField] private bool _isCeramicBloon;
    [SerializeField] private bool _isLeadBloon;
    [SerializeField] private bool _isBombImmune;

    [Header("Layers")]
    [SerializeField] private List <GameObject> _weakerEnemies; // weaker enemy prefab
    [SerializeField] private int _enemyAmount; // enemies count after bloon defeat
    [SerializeField] private List<Sprite> _temporarySprites;
    private int _spriteIndex = 0;

    [Header("Health")]
    [SerializeField] private int _rbe;
    [SerializeField] private int _layerHp; // for ceramic, moabs, etc.
    [SerializeField] private int _hitCount; // for ceramic, moabs, etc.
    [SerializeField] private int _hitsToChangeSprite;
    [SerializeField] private List<int> _criticalPoints;

    [Header("Parent")]
    private GameObject _parent;

    [Header("Level")]
    [SerializeField] private GameObject _levelObject;

    [Header("SFX")]
    [SerializeField] private AudioClip _popSound;
    [SerializeField] private AudioClip _leadSound;

    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;
    private static bool _isChangingScene = false;

    // Info from other towers
    private int _popThrough;
    private bool _isDestroyed;

    private void Start()
    {
        _isDestroyed = false;

        _enemyMovement = GetComponent<EnemyMovement>();
        _parent = GameObject.Find("BloonHolder");
        gameObject.transform.SetParent(_parent.transform);
        _hitCount = 0;
    }

    private void Update()
    {
        // When the bloon reaches the end, it is time to destroy the bloon
        if (_enemyMovement.GetProgress() >= 1.0f)
        {
            Destroy(gameObject);
        }

        if (!_isDestroyed && _popThrough > 0)
        {
            DestroyBloonsTillPierceEnd();
        }
    }

    private void DestroyBloonsTillPierceEnd()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _isDestroyed = true;

        if(_enemyMovement != null)
        {
            // The bloon reached the end
            if (_enemyMovement.GetProgress() >= 1.0f)
            {
                PlayerStats.Instance.DecreaseLifeAmount(_rbe);
            }
            // The app is still running, on destroying objects spawn new bloons
            else if (!_isAppQuitting && _enemyMovement.GetProgress() < 1.0f && !_isChangingScene)
            {
                if (_weakerEnemies.Count != 0)
                {
                    _popThrough--;
                    SpawnWeakerLayer();
                }

                // for every bloon popped the money amount increases
                PlayerStats.Instance.AddMoneyForBloonPop();
            }
        }
    }

    // Destroy object hit by physical projectile (1 damage)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroyLayeredEnemy(collision, 1);
    }

    public void DestroyLayeredEnemy(Collider2D collision, int hitAmount)
    {
        // Change Sprites while shooting
        if (collision != null)
        {
            if (_layerHp > 1 && collision.gameObject.CompareTag("Projectile"))
            {
                Debug.Log("DART HIT");
                int previousHitCount = _hitCount;
                _hitCount++;

                foreach (int criticalPoint in _criticalPoints)
                {
                    if (previousHitCount < criticalPoint && _hitCount >= criticalPoint && criticalPoint <= _layerHp)
                    {
                        transform.gameObject.GetComponent<SpriteRenderer>().sprite = _temporarySprites[_spriteIndex];
                        _spriteIndex++;
                    }
                }
            }
        }
        else
        {
            if (_layerHp > 1)
            {
                Debug.Log("SNIPER HIT");

                int previousHitCount = _hitCount;
                _hitCount += hitAmount;

                foreach (int criticalPoint in _criticalPoints)
                {
                    if (previousHitCount < criticalPoint && _hitCount >= criticalPoint && criticalPoint <= _layerHp)
                    {
                        transform.gameObject.GetComponent<SpriteRenderer>().sprite = _temporarySprites[_spriteIndex];
                        _spriteIndex++;
                    }
                }
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

        var weakerEnemyIndex = 0;

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

                // do not rotate bloon that are not moab class bloon
                if (!_weakerEnemies[weakerEnemyIndex].GetComponent<BloonController>()._isMoabClassBloon)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                // Instantiating new bloon
                var newBloon = Instantiate(_weakerEnemies[weakerEnemyIndex], setPosition, transform.rotation);
                newBloon.GetComponent<BloonController>().SetIsPopThrough(_popThrough);

                var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
                var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

                var oldEnemyDistance = oldEnemyMovement.GetCurrentDistance();

                // Setting distance, position and progress of the new bloon
                newEnemyMovement.SetCurrentDistance(oldEnemyDistance - distanceFromCenter);
                newEnemyMovement.SetCurrentPosition(setPosition);
                newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());

                distanceFromCenter += addDistance;

                if (_weakerEnemies.Count > 1)
                    weakerEnemyIndex++;
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

                // do not rotate bloon that are not moab class bloon
                if (!_weakerEnemies[weakerEnemyIndex].GetComponent<BloonController>()._isMoabClassBloon)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                // Instantiating new bloon
                var newBloon = Instantiate(_weakerEnemies[weakerEnemyIndex], setPosition, transform.rotation);
                newBloon.GetComponent<BloonController>().SetIsPopThrough(_popThrough);

                var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
                var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

                var oldEnemyDistance = oldEnemyMovement.GetCurrentDistance();

                // Setting distance, position and progress of the new bloon
                newEnemyMovement.SetCurrentDistance(oldEnemyDistance - distanceFromCenter);
                newEnemyMovement.SetCurrentPosition(setPosition);
                newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());

                distanceFromCenter += addDistance;

                if (_weakerEnemies.Count > 1)
                    weakerEnemyIndex++;
            }
        }
    }

    public bool LayerDestroyed()
    {
        if(_layerHp > 1)
        {
            if (_hitCount >= _layerHp)
                return true;
            else
                return false;
        }
        else
        {
            return true;
        }
    }

    public void RotateMoabClassBloon()
    {
        if (_isMoabClassBloon)
        {
            var movementDirection = gameObject.GetComponent<EnemyMovement>().GetMovementDirection();

            if(movementDirection == Vector3.down)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 180.0f);
            }
            else if(movementDirection == Vector3.up)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0.0f);
            }
            else if(movementDirection == Vector3.right)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 270.0f);
            }
            else if(movementDirection == Vector3.left)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 90.0f);
            }
        }
    }

    public void SetIsPopThrough(int popThrough)
    {
        _popThrough = popThrough;
    }

    public static void SetIsChangingScene(bool value)
    {
        _isChangingScene = value;
    }

    public bool IsCeramicBloon()
    {
        return _isCeramicBloon;
    }

    public bool IsMoabClassBloon()
    {
        return _isMoabClassBloon;
    }

    public bool IsLeadBloon()
    {
        return _isLeadBloon;
    }

    public bool IsBombImmune()
    {
        return _isBombImmune;
    }

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }

    public AudioClip GetPopSound(bool cannotPopLead)
    {
        if(!cannotPopLead)
            return _popSound;
        else
            return _leadSound;
    }
}
