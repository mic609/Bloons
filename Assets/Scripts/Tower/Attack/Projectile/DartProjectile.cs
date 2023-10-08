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
            var cannotPopLead = enemy.GetComponent<BloonController>().IsLeadBloon();

            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(cannotPopLead));

            // Dart needs to destroy the whole shield and the bloon is not a lead bloon
            if (enemy.GetComponent<BloonController>().LayerDestroyed() && !cannotPopLead)
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
