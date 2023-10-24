using UnityEngine;

public class AceProjectile : MonoBehaviour
{
    [SerializeField] private int _pierce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Destroy enemy
            var enemy = collision.gameObject;
            var cannotPopLead = enemy.GetComponent<BloonController>().IsLeadBloon();

            // If the bloon is lead, play lead sound, otherwise play normal sound
            SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(cannotPopLead));

            // Dart needs to destroy the whole shield and the bloon is not a lead bloon
            if (enemy.GetComponent<BloonController>().LayerDestroyed() >= 0 && !cannotPopLead)
            {
                Destroy(enemy);
                gameObject.SetActive(false);
                // Add statistics for the tower
                transform.parent.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(1);
            }
            else
            {
                gameObject.SetActive(false);
            }

            // Make the Projectile disappear
            gameObject.transform.position = gameObject.transform.parent.position;
        }
    }
}
