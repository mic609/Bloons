using System;
using UnityEngine;

public class AceProjectile : MonoBehaviour
{
    [SerializeField] private int _pierce;
    [SerializeField] private int _damage;
    private int _bloonsPierced = 0;
    [SerializeField] private bool _cannotPopLead;

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
                    transform.parent.parent.parent.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                }
                else if (hitsLeft > 0)
                {
                    // there is +1 for some reason. We will understand why it needs to be like this

                    if (!isMoabClassBloon)
                    {
                        transform.parent.parent.parent.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
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
                if (enemy.GetComponent<BloonController>().GetPopThrough() == -1)
                    enemy.GetComponent<BloonController>().SetIsPopThrough(_damage); // define the damage

                if (enemy.GetComponent<BloonController>().GetPopThrough() != 0)
                {
                    Destroy(enemy);
                    _bloonsPierced++;
                    enemy.GetComponent<BloonController>().SetIsPopThrough(-1);
                    //Debug.Log("Destroyed enemy: " + enemy.name + ": " + enemy.GetComponent<BloonController>().GetPopThrough());
                }
                else
                    enemy.GetComponent<BloonController>().SetIsPopThrough(-1);

                // Add statistics for the tower
                transform.parent.parent.parent.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
            }

            //Debug.Log("Finally: " + enemy.GetComponent<BloonController>().GetPopThrough());

            if (popAbility)
            {
                ProjectileReset();
            }
            else if ((isMoabClassBloon || isCeramicBloon) && _bloonsPierced >= _pierce)
            {
                ProjectileReset();
            }
            else if (_bloonsPierced >= _pierce && (enemy.GetComponent<BloonController>().GetPopThrough() == -1 || enemy.name == "Red Bloon(Clone)"
                || _damage == enemy.GetComponent<BloonController>().GetRbe()))
            {
                // Make the Projectile disappear
                ProjectileReset();
            }
        }
    }

    protected void ProjectileReset()
    {
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
        _bloonsPierced = 0;
    }

    private int BloonsPoppedAmount(Transform enemy)
    {
        var bloonRbe = enemy.gameObject.GetComponent<BloonController>().GetRbe();
        if (bloonRbe < _damage)
            return bloonRbe;
        else
            return _damage;
    }

    public void SetCannotPopLead(bool cannotPopLead)
    {
        _cannotPopLead = cannotPopLead;
    }

    internal void SetDamage(int damage)
    {
        _damage = damage;
    }

    internal void SetPierce(int pierce)
    {
        _pierce = pierce;
    }
}
