// Destroying bloons logic, other bloon details

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class BloonController : MonoBehaviour
{
    // Define some special bloons type
    [Header("Type")]
    [SerializeField] private bool _isMoabClassBloon;
    [SerializeField] private bool _isCeramicBloon;
    [SerializeField] private bool _isLeadBloon;
    [SerializeField] private bool _isBombImmune;

    [Header("Layers")]
    [SerializeField] private List <GameObject> _weakerEnemies; // after bloon destroy, _weakerEnemies are going to be spawned
    [SerializeField] private int _enemyAmount; // the number of _weakerEnemies

    // This applies to heavy bloons that need more than 1 damage to be destroyed
    // Sprites represents damage state of heavy bloon
    [Header("Shield")]
    [SerializeField] private List<Sprite> _temporarySprites;
    [SerializeField] private List<Sprite> _temporaryGlueSprites;
    private int _spriteIndex = 0;

    [Header("Health")]
    [SerializeField] private int _rbe; // red bloon equivalent- this is the bloon health
    [SerializeField] private int _layerHp; // additional shield for heavy bloons
    private int _hitCount; // for ceramic, moabs, etc.
    [SerializeField] private List<int> _criticalPoints; // critical points represent the place where the sprite is being changed 

    [Header("Parent")]
    private GameObject _parent; // bloon holder object

    [Header("Level")]
    [SerializeField] private GameObject _levelObject;

    [Header("SFX")]
    [SerializeField] private AudioClip _popSound;
    [SerializeField] private AudioClip _leadSound;

    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;
    private static bool _isChangingScene = false;

    // Info from other towers
    private int _popThrough; // If this variable is greater than zero bloon needs to be destroyed
    private bool _isDestroyed; // Is bloon being destroyed right now?

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _parent = GameObject.Find("BloonHolder");
        gameObject.transform.SetParent(_parent.transform);

        _isDestroyed = false;
        _hitCount = 0;
    }

    private void Update()
    {
        // When the bloon reaches the end, it is time to destroy the bloon
        if (_enemyMovement.GetProgress() >= 1.0f)
        {
            Destroy(gameObject);
        }

        // Destroy the bloon if the projectile has more than 1 point damage
        if (!_isDestroyed && _popThrough > 0)
        {
            Destroy(gameObject);
        }
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
        if (!collision.CompareTag("GlueProjectile"))
            DestroyLayeredEnemy(collision, 1);
    }

    // Destroying enemies with shield logic
    public void DestroyLayeredEnemy(Collider2D collision, int hitAmount)
    {
        // This is from physical projectile on scene
        if (collision != null)
        {
            if (_layerHp > 1 && collision.gameObject.CompareTag("Projectile"))
                ChangeSprite(hitAmount);
        }
        // There is no projectile to destroy enemy (sniper monkey etc.)
        else
        {
            if (_layerHp > 1)
                ChangeSprite(hitAmount);
        }
    }

    // Destroying enemies with shield logic
    private void ChangeSprite(int hitAmount)
    {
        int previousHitCount = _hitCount;
        _hitCount += hitAmount;

        foreach (int criticalPoint in _criticalPoints)
        {
            if (previousHitCount < criticalPoint && _hitCount >= criticalPoint && criticalPoint <= _layerHp)
            {
                // for glued bloons
                if (gameObject.GetComponent<BloonEffects>() != null && gameObject.GetComponent<BloonEffects>().HasGlueEffect() && _temporaryGlueSprites.Count != 0)
                    transform.gameObject.GetComponent<SpriteRenderer>().sprite = _temporaryGlueSprites[_spriteIndex];
                // for standard bloons
                else
                    transform.gameObject.GetComponent<SpriteRenderer>().sprite = _temporarySprites[_spriteIndex];
                _spriteIndex++;
            }
        }
    }

    // On destroy spawn weaker enemies
    private void SpawnWeakerLayer()
    {
        float distanceFromCenter = 0.0f;
        float addDistance = 0.0f;

        // In switch we need to define the distance between weaker spawned bloons
        // For example in case 2: *bloon* (x = -0.3), x = 0, *bloon* (x = 0.3)
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

        // We need to know the movement direction of the bloons, so we can spawn them vertically or horizontally
        var movementDirection = gameObject.GetComponent<EnemyMovement>().GetMovementDirection();
        if (movementDirection == Vector3.zero)
            return;

        var weakerEnemyIndex = 0;

        if (movementDirection == Vector3.down || movementDirection == Vector3.up)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                // Setting position of the spawned bloons
                var setPosition = Vector3.zero;
                if (movementDirection == Vector3.down)
                    setPosition = new Vector3(transform.position.x, transform.position.y - distanceFromCenter, transform.position.z);
                else if(movementDirection == Vector3.up)
                    setPosition = new Vector3(transform.position.x, transform.position.y + distanceFromCenter, transform.position.z);

                InstantiateWeakerEnemy(weakerEnemyIndex, setPosition, distanceFromCenter);

                distanceFromCenter += addDistance;

                if (_weakerEnemies.Count > 1)
                    weakerEnemyIndex++;
            }
        }
        else if(movementDirection == Vector3.right || movementDirection == Vector3.left)
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                // Setting position of the spawned bloons
                var setPosition = Vector3.zero;
                if (movementDirection == Vector3.right)
                    setPosition = new Vector3(transform.position.x + distanceFromCenter, transform.position.y, transform.position.z);
                else if (movementDirection == Vector3.left)
                    setPosition = new Vector3(transform.position.x - distanceFromCenter, transform.position.y, transform.position.z);

                InstantiateWeakerEnemy(weakerEnemyIndex, setPosition, distanceFromCenter);

                distanceFromCenter += addDistance;

                if (_weakerEnemies.Count > 1)
                    weakerEnemyIndex++;
            }
        }
    }

    private void InstantiateWeakerEnemy(int weakerEnemyIndex, Vector3 setPosition, float distanceFromCenter)
    {
        // do not rotate bloon that are not moab class bloon
        if (!_weakerEnemies[weakerEnemyIndex].GetComponent<BloonController>()._isMoabClassBloon)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // Instantiating new bloon
        var newBloon = Instantiate(_weakerEnemies[weakerEnemyIndex], setPosition, transform.rotation);
        if (_popThrough > 0)
        {
            // Don't show inside bloons while shooting pop through
            newBloon.GetComponent<SpriteRenderer>().enabled = false;
        }

        newBloon.GetComponent<BloonController>().SetIsPopThrough(_popThrough);
        var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();

        var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();
        var oldEnemyDistance = oldEnemyMovement.GetCurrentDistance();

        // (We check it for moabs)
        if (gameObject.GetComponent<BloonEffects>() != null)
        {
            GlueEffectsDetails(newBloon);
        }

        // Setting distance, position and progress of the new bloon based on the old bloon
        newEnemyMovement.SetCurrentDistance(oldEnemyDistance - distanceFromCenter);
        newEnemyMovement.SetCurrentPosition(setPosition);
        newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());
    }

    private void GlueEffectsDetails(GameObject newBloon)
    {
        // glue effetcs
        if (gameObject.GetComponent<BloonEffects>().HasGlueEffect())
        {
            var newLayersThrough = gameObject.GetComponent<BloonEffects>().GetGlueLayersThrough() - 1;
            newBloon.GetComponent<BloonEffects>().SetGlueLayersThrough(newLayersThrough);

            if (newLayersThrough > 0)
            {
                var movementSpeedDecrease = gameObject.GetComponent<BloonEffects>().GetMovementSpeedDecrease();
                var glueLastingEffect = gameObject.GetComponent<BloonEffects>().GetGlueLastingEffect();
                var poppingSpeed = gameObject.GetComponent<BloonEffects>().GetPoppingSpeed();
                newBloon.GetComponent<BloonEffects>().SetGlueEffect(movementSpeedDecrease, glueLastingEffect, newLayersThrough, poppingSpeed);
            }

            // remember the tower that has initially attacked
            newBloon.GetComponent<BloonTowerReference>().SetTowerThatAttacks(gameObject.GetComponent<BloonTowerReference>().GetTower());
        }
    }

    // Check if the layer of heavy bloons is destroyed
    public int LayerDestroyed()
    {
        if(_layerHp > 1)
        {
            return _hitCount - _layerHp;
        }
        return 0;
    }

    // Rotate Moab class bloon based on the movement direction
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

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }

    ////////////////////////////////////////////
    // Getters and setters
    ////////////////////////////////////////////

    public Sprite ReturnUnGluedCeramicSprite() 
    {
        return _temporarySprites[_spriteIndex - 1];
    }

    public AudioClip GetPopSound(bool cannotPopLead)
    {
        if (!cannotPopLead)
            return _popSound;
        else
            return _leadSound;
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

    public bool IsUnderAttack()
    {
        return _isDestroyed;
    }

    public int getRbe()
    {
        return _rbe;
    }
}
