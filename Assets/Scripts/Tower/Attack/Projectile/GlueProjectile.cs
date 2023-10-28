using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlueProjectile : Projectile
{
    [Header("Glue Area")]
    [SerializeField] private float _areaDiameter;
    [SerializeField] private float _numberOfBloonsToPop; // around the whole diameter

    [Header("Glue details")]
    [SerializeField] private float _movementSpeedDecrease; // glue makes bloons slow
    [SerializeField] private float _glueLastingEffect; // how long bloon is being affected by glue
    [SerializeField] private int _layersThrough; // how much layers of bloon glue affects
    [SerializeField] private float _poppingSpeed;

    [Header("Enemy")]
    [SerializeField] private LayerMask _layerMask;

    //[Header("SFX")]
    //[SerializeField] private AudioClip _bombSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject;

            // Glue can affect lead
            // SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(false));

            // Find all of the bloons in the explosion area
            var colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, _areaDiameter / 2, _layerMask);

            // Add found objects to the list
            var bloonsToBeGlued = new List<GameObject>();
            foreach (var col in colliders)
            {
                bloonsToBeGlued.Add(col.gameObject);
            }

            //SoundManager.Instance.PlaySound(_bombSound);

            var firstBloonToBeGlued = ChooseFirstTargetForArea(bloonsToBeGlued);
            if(firstBloonToBeGlued != null && firstBloonToBeGlued.GetComponent<BloonEffects>() != null)
            {
                firstBloonToBeGlued.GetComponent<BloonTowerReference>().SetTowerThatAttacks(transform.parent.parent.GetComponent<ManageTower>());
                firstBloonToBeGlued.GetComponent<BloonEffects>().SetGlueEffect(_movementSpeedDecrease, _glueLastingEffect, _layersThrough, _poppingSpeed);
            }

            // Glue all of the bloons in the area
            for (int i = 0; i < _numberOfBloonsToPop - 1; i++)
            {
                if (bloonsToBeGlued.Count == 0)
                    break;

                if(!(bloonsToBeGlued.Count <= 1))
                {
                    var index = Random.Range(0, bloonsToBeGlued.Count);
                    var bloonToBeGlued = bloonsToBeGlued[index];

                    while (bloonToBeGlued == firstBloonToBeGlued)
                    {
                        index = Random.Range(0, bloonsToBeGlued.Count);
                        bloonToBeGlued = bloonsToBeGlued[index];
                    }

                    bloonToBeGlued.GetComponent<BloonTowerReference>().SetTowerThatAttacks(transform.parent.parent.GetComponent<ManageTower>());
                    bloonToBeGlued.GetComponent<BloonEffects>().SetGlueEffect(_movementSpeedDecrease, _glueLastingEffect, _layersThrough, _poppingSpeed);
                    bloonsToBeGlued.RemoveAt(index);
                }
            }

            ProjectileReset();
        }
    }

    private GameObject ChooseFirstTargetForArea(List<GameObject> bloonsToBeGlued)
    {
        var biggestProgress = 0f;
        var currentProgress = 0f;

        var targetToReturn = new GameObject();

        foreach(var bloon in bloonsToBeGlued)
        {
            currentProgress = bloon.GetComponent<EnemyMovement>().GetProgress();

            if (currentProgress > biggestProgress && bloon.GetComponent<BloonEffects>()!= null && !bloon.GetComponent<BloonEffects>().HasGlueEffect())
            {
                targetToReturn = bloon;
                biggestProgress = currentProgress;
            }
        }

        return targetToReturn;
    }

    public override void UpgradeBullet(UpgradeData _upgrade)
    {
        _numberOfBloonsToPop = _upgrade.numberOfBloonsToPop;
        _movementSpeedDecrease = _upgrade.movementSpeedDecrease;
        _glueLastingEffect = _upgrade.glueLastingEffect;
        _layersThrough = _upgrade.layersAffected;
        _poppingSpeed = _upgrade.poppingSpeed;
    }

    // Glue area
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _areaDiameter / 2);
    }
}
