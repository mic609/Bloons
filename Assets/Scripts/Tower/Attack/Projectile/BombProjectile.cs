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

            // Destroy all of the bloons in the area
            for (int i = 0; i < _numberOfBloonsToPop; i++)
            {
                if (bloonsToDestroy.Count == 0)
                    break;

                var index = Random.Range(0, bloonsToDestroy.Count);
                var bloonToDestroy = bloonsToDestroy[index];

                // The whole shield needs to be destroyed and the bloon cannot be immune to bombs
                if (enemy.GetComponent<BloonController>().LayerDestroyed() >= 0 && !enemy.GetComponent<BloonController>().IsBombImmune())
                {
                    Destroy(bloonToDestroy);
                    transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(1);
                }


                bloonsToDestroy.RemoveAt(index);
            }

            ProjectileReset();
        }
    }

    // Explosion area
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionDiameter / 2);
    }
}
