// The script describes the bullet behaviour

using UnityEngine;

public class DartProjectile : Projectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Destroy enemy
            var enemy = collision.gameObject;
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound());
            if (enemy.GetComponent<BloonController>().LayerDestroyed())
            {
                Destroy(enemy);

                // Add statistics for the tower
                transform.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp();
            }

            // Make the Projectile disappear
            ProjectileReset();
        }
    }
}
