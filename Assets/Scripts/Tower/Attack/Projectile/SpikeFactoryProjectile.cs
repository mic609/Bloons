using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeFactoryProjectile : Projectile
{
    [Header("Spike Alive Time")]
    [SerializeField] private float _timeAlive;

    [Header("Spike States")]
    [SerializeField] private List<Sprite> _sprites;
    private Sprite _initialSprite;
    private int _spriteIndex = 0;
    private int _damageDone = 0;

    private bool _shouldMove = true;

    private void OnEnable()
    {
        _initialSprite = _sprites[0];
        StartCoroutine(TimeAliveCoroutine());
    }

    private IEnumerator TimeAliveCoroutine()
    {
        yield return new WaitForSeconds(_timeAlive);
        ProjectileReset();
        _shouldMove = true;
        GetComponent<SpriteRenderer>().sprite = _initialSprite;
        _spriteIndex = 0;
    }

    private void ChangeSprite()
    {
        _spriteIndex++;

        if(_spriteIndex >= _sprites.Count)
        {
            ProjectileReset();
            _shouldMove = true;
            GetComponent<SpriteRenderer>().sprite = _initialSprite;
            _spriteIndex = 0;
        }
        else
            GetComponent<SpriteRenderer>().sprite = _sprites[_spriteIndex];
    }

    public override void Attack(Transform target, Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.position = _startingPosition;
        //_target = target;
        _shootingDirection = direction;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Path"))
        {
            var pathCollider = collision.GetComponent<PolygonCollider2D>();
            var boxCollider = GetComponent<BoxCollider2D>();
            var colliderPoints = GetColliderPoints(boxCollider);

            bool isInside = true;

            foreach (var point in colliderPoints)
            {
                Vector2 boxCenter = boxCollider.bounds.center;
                var direction = point - boxCenter;
                var hit = Physics2D.Raycast(point, direction.normalized, direction.magnitude, LayerMask.GetMask("Path"));

                if (!hit.collider || hit.collider != pathCollider)
                {
                    isInside = false;
                    break;
                }
            }

            if (isInside)
            {
                _shouldMove = false;
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            // Destroy enemy
            var enemy = collision.gameObject;

            var popAbility = false;
            if (_cannotPopLead)
                popAbility = enemy.GetComponent<BloonController>().IsLeadBloon();
            // If the bloon is lead, play lead sound, otherwise play normal sound
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(popAbility));

            // Dart needs to destroy the whole shield and the bloon is not a lead bloon
            if (enemy.GetComponent<BloonController>().LayerDestroyed() >= 0 && !popAbility)
            {
                Destroy(enemy);

                // Add statistics for the tower
                transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(_damage);
            }

            _damageDone++;

            if (_damageDone >= _damage)
            {
                ChangeSprite();
                _damageDone = 0;
            }
        }
    }

    private Vector2[] GetColliderPoints(BoxCollider2D boxCollider)
    {
        var points = new Vector2[4];

        points[0] = boxCollider.transform.TransformPoint(new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2));
        points[1] = boxCollider.transform.TransformPoint(new Vector2(-boxCollider.size.x / 2, boxCollider.size.y / 2));
        points[2] = boxCollider.transform.TransformPoint(new Vector2(-boxCollider.size.x / 2, -boxCollider.size.y / 2));
        points[3] = boxCollider.transform.TransformPoint(new Vector2(boxCollider.size.x / 2, -boxCollider.size.y / 2));

        return points;
    }

    protected override void Update()
    {
        Level level;
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "MapFlower")
            level = GameObject.Find("Map").GetComponent<Level>();
        else if (currentSceneName == "MapBeach")
            level = GameObject.Find("Map2").GetComponent<Level>();
        else
            level = GameObject.Find("Map").GetComponent<Level>();

        if (level.IsLevelFinished())
        {
            ProjectileReset();
            _shouldMove = true;
            GetComponent<SpriteRenderer>().sprite = _initialSprite;
            _spriteIndex = 0;
        }
        else
        {
            if (_shouldMove)
            {
                var move = Time.deltaTime * _bulletSpeed;
                _currentDistance += move;

                transform.position += _shootingDirection * _bulletSpeed * Time.deltaTime;
            }

            if (_currentDistance >= _maxDistance)
            {
                ProjectileReset();
                _shouldMove = true;
                GetComponent<SpriteRenderer>().sprite = _initialSprite;
                _spriteIndex = 0;
            }
        }
    }

    public override void UpgradeBullet(UpgradeData upgrade)
    {
        _timeAlive = upgrade.timeAlive;
        _cannotPopLead = upgrade.cannotPopLead;
        _damage = upgrade.damage;
    }
}
