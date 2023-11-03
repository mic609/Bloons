using UnityEngine;

public class DartlingGunnerProjectile : Projectile
{
    private int _bloonsPierced = 0;

    [SerializeField] private Vector3 _borderVectorA;
    [SerializeField] private Vector3 _borderVectorB;

    private Vector3 _initialVectorA;
    private Vector3 _initialVectorB;
    private Vector3 _previousBorderVectorA;
    private Vector3 _previousBorderVectorB;

    protected override void Awake()
    {
        _startingPosition = transform.parent.parent.Find("Sprite").Find("AttackPoint").position;

        _initialVectorA = _borderVectorA;
        _initialVectorB = _borderVectorB;
        _previousBorderVectorA = _borderVectorA;
        _previousBorderVectorB = _borderVectorB;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Destroy enemy
            var enemy = collision.gameObject;

            var popAbility = false;
            if (_cannotPopLead)
                popAbility = enemy.GetComponent<BloonController>().IsLeadBloon();

            // If the bloon is lead, play lead sound, otherwise play normal sound
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(popAbility));


            var isMoabClassBloon = enemy.GetComponent<BloonController>().IsMoabClassBloon();
            var isCeramicBloon = enemy.GetComponent<BloonController>().IsCeramicBloon();

            var bloonsPopped = BloonsPoppedAmount(enemy.transform);

            if ((isMoabClassBloon || isCeramicBloon))
            {
                enemy.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);

                var hitsLeft = enemy.GetComponent<BloonController>().LayerDestroyed();

                _bloonsPierced++;

                if (hitsLeft == 0)
                {
                    Destroy(enemy);
                    transform.parent.parent.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                }
                else if (hitsLeft > 0)
                {
                    // there is +1 for some reason. We will understand why it needs to be like this

                    if (!isMoabClassBloon)
                    {
                        transform.parent.parent.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                        enemy.GetComponent<BloonController>().SetIsPopThrough(hitsLeft + 1);
                        Destroy(enemy);
                    }
                    else
                    {
                        enemy.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);
                        Destroy(enemy);
                    }
                }

            }
            // Dart needs to destroy the whole shield and the bloon is not a lead bloon
            else if (enemy.GetComponent<BloonController>().LayerDestroyed() >= 0 && !popAbility)
            {
                // Set popthrough for the initial spawned bloon
                if (enemy.GetComponent<BloonController>().GetPopThrough() <= -1)
                {
                    enemy.GetComponent<BloonController>().SetIsPopThrough(_damage); // define the damage
                    // Add statistics for the tower
                    transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                }

                _bloonsPierced++;
            }

            if (popAbility)
            {
                ProjectileReset();
            }
            else if ((isMoabClassBloon || isCeramicBloon) && _bloonsPierced >= _pierce)
            {
                ProjectileReset();
            }
            else if (_bloonsPierced >= _pierce)
            {
                // Make the Projectile disappear
                ProjectileReset();
            }
        }
    }

    protected override void Update()
    {
        var move = Time.deltaTime * _bulletSpeed;
        _currentDistance += move;

        transform.position += _shootingDirection * _bulletSpeed * Time.deltaTime;

        if (_currentDistance >= _maxDistance)
        {
            ProjectileReset();
        }
    }

    public override void Attack(Transform target)
    {
        _startingPosition = transform.parent.parent.Find("Sprite").Find("AttackPoint").position;

        gameObject.SetActive(true);

        transform.position = _startingPosition;

        var angleInDegrees = transform.parent.parent.Find("Sprite").rotation.eulerAngles.z;
        var angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        var distance = Mathf.Sin(angleInRadians);
        var rotatedVectorA = Quaternion.Euler(0, 0, angleInDegrees) * new Vector3(_initialVectorA.x, _initialVectorA.y, distance);
        var rotatedVectorB = Quaternion.Euler(0, 0, angleInDegrees) * new Vector3(_initialVectorB.x, _initialVectorB.y, -distance);
        _borderVectorA = rotatedVectorA;
        _borderVectorB = rotatedVectorB;

        if (_previousBorderVectorA != _borderVectorA || _previousBorderVectorB != _borderVectorB)
        {
            var random = Random.value;
            _shootingDirection = Vector3.Lerp(_borderVectorA, _borderVectorB, random);
            _shootingDirection.Normalize();
            var angle = Mathf.Atan2(_shootingDirection.y, _shootingDirection.x) * Mathf.Rad2Deg + 90.0f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            _previousBorderVectorA = _borderVectorA;
            _previousBorderVectorB = _borderVectorB;
        }
    }

    public override void UpgradeBullet(UpgradeData upgrade)
    {
        _initialVectorA = upgrade.borderVectorA;
        _initialVectorB = upgrade.borderVectorB;
        _cannotPopLead = upgrade.cannotPopLead;
        _pierce = upgrade.pierce;
        _damage = upgrade.damage;
    }

    private int BloonsPoppedAmount(Transform enemy)
    {
        var bloonRbe = enemy.gameObject.GetComponent<BloonController>().GetRbe();
        if (bloonRbe < _damage)
            return bloonRbe;
        else
            return _damage;
    }

    protected override void ProjectileReset()
    {
        base.ProjectileReset();
        _bloonsPierced = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _borderVectorA);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _borderVectorB);
    }
}
