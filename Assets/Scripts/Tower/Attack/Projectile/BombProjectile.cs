using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : Projectile
{
    [Header("Explosion")]
    [SerializeField] private float _explosionDiameter;
    [SerializeField] private float _numberOfBloonsToPop; // at once
    
    [Header("Enemy")]
    [SerializeField] private LayerMask _layerMask;

    private Transform _explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject;
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound());

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

            for (int i = 0; i < _numberOfBloonsToPop; i++)
            {
                if (bloonsToDestroy.Count == 0)
                    break;

                var index = Random.Range(0, bloonsToDestroy.Count);
                var bloonToDestroy = bloonsToDestroy[index];

                if (enemy.GetComponent<BloonController>().LayerDestroyed())
                {
                    Destroy(bloonToDestroy);
                    transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp();
                }


                bloonsToDestroy.RemoveAt(index);
            }

            ProjectileReset();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionDiameter / 2);
    }
}
