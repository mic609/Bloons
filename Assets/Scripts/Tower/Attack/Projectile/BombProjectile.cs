// Bomb projectile logic

using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : Projectile
{
    [Header("Explosion")]
    [SerializeField] private float _explosionDiameter;
    [SerializeField] private float _numberOfBloonsToPop; // around the whole diameter
    
    [Header("Enemy")]
    [SerializeField] private LayerMask _layerMask;

    [Header("SFX")]
    [SerializeField] private AudioClip _bombSound;
    [SerializeField] private AudioClip _ceramicPop;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject;

            // Bomb can pop lead so we use normal pop sound
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(false));

            // Find all of the bloons in the explosion area
            var colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, _explosionDiameter / 2, _layerMask);

            // Add found objects to the list
            var bloonsToDestroy = new List<GameObject>();
            foreach (var col in colliders)
            {
                bloonsToDestroy.Add(col.gameObject);
            }

            // BOOM!
            transform.GetComponentInParent<BombTowerAttack>().ExplosionDuration(collision.gameObject.transform, _explosionDiameter);
            SoundManager.Instance.PlaySound(_bombSound);

            var ifStatementReached = false;

            // Destroy all of the bloons in the area
            for (int i = 0; i < _numberOfBloonsToPop; i++)
            {
                if (bloonsToDestroy.Count == 0)
                    break;

                var index = Random.Range(0, bloonsToDestroy.Count);
                var bloonToDestroy = bloonsToDestroy[index];

                // play sound for ceramic bloons
                if(bloonToDestroy.GetComponent<BloonController>().IsCeramicBloon() && !ifStatementReached)
                {
                    SoundManager.Instance.PlaySound(bloonToDestroy.GetComponent<BloonController>().GetPopSound(false));
                    ifStatementReached = true;
                }

                var isMoabClassBloon = bloonToDestroy.GetComponent<BloonController>().IsMoabClassBloon();
                var isCeramicBloon = bloonToDestroy.GetComponent<BloonController>().IsCeramicBloon();

                var bloonsPopped = BloonsPoppedAmount(bloonToDestroy.transform);

                if ((isMoabClassBloon || isCeramicBloon))
                {
                    // Break the bloon shield
                    bloonToDestroy.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);

                    var hitsLeft = bloonToDestroy.GetComponent<BloonController>().LayerDestroyed();

                    // This is complicated. The shield bloon (ceramic, moab) after being destroyed still can affect
                    // weaker enemies (hitsLeft > 0), but it depends on the shield remaining health
                    if (hitsLeft == 0)
                    {
                        Destroy(bloonToDestroy);
                        transform.parent.parent.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                    }
                    else if (hitsLeft > 0)
                    {
                        // there is +1 for some reason. We will understand why it needs to be like this

                        if (!isMoabClassBloon)
                        {
                            transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                            bloonToDestroy.GetComponent<BloonController>().SetIsPopThrough(hitsLeft + 1);
                            Destroy(bloonToDestroy);
                        }
                        else
                        {
                            bloonToDestroy.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);
                            Destroy(bloonToDestroy);
                        }
                    }

                }
                // The whole shield needs to be destroyed and the bloon cannot be immune to bombs
                else if (bloonToDestroy.GetComponent<BloonController>().LayerDestroyed() >= 0 && !bloonToDestroy.GetComponent<BloonController>().IsBombImmune())
                {
                    Destroy(bloonToDestroy);
                    bloonToDestroy.GetComponent<BloonController>().SetIsPopThrough(_damage);
                    transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(_damage);
                }


                bloonsToDestroy.RemoveAt(index);
            }

            ProjectileReset();
        }
    }

    private int BloonsPoppedAmount(Transform enemy)
    {
        var bloonRbe = enemy.gameObject.GetComponent<BloonController>().GetRbe();
        if (bloonRbe < _damage)
            return bloonRbe;
        else
            return _damage;
    }

    public override void UpgradeBullet(UpgradeData _upgrade)
    {
        _explosionDiameter = _upgrade.projectileArea;
        _numberOfBloonsToPop = _upgrade.numberOfBloonsToPop;
        _damage = _upgrade.damage;
    }

    // Explosion area
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionDiameter / 2);
    }
}
