// Sniper Monkey attack logic

using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SniperMonkeyAttack : MonoBehaviour
{
    [Header("Attack Details")]
    [SerializeField] private float _delay; // time between shooting
    [SerializeField] private float _delayTimer;
    [SerializeField] private int _damage; // how many bloons the tower can pop through
    private bool _bonusDamageForCeramic; // applied for specific sniper monkey upgrades to destroy ceramic bloon immediately
    
    private GameObject _bloons; // bloons on the map

    private TowerRotation _towerRotation;

    private void Start()
    {
        _bonusDamageForCeramic = false;
        _bloons = GameObject.Find("BloonHolder");
        _towerRotation = GetComponentInChildren<TowerRotation>();
        _delayTimer = _delay;
    }

    private void Update()
    {
        if (_bloons.transform.childCount > 0)
        {
            // Tell the sniper monkey management class that sniper is ready to attack
            // More details in SniperMonkeyManager class
            SniperMonkeyManager.Instance.AddToTheQueue(this);
        }

        _delayTimer += Time.deltaTime;
    }

    // When enemy in range
    public void StartAttack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            if (_delayTimer >= _delay)
            {
                // Rotate Tower towards target
                _towerRotation.Rotate(enemy);

                // Attack with sound
                _delayTimer = 0.0f;
                SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(false));

                var isMoabClassBloon = enemy.gameObject.GetComponent<BloonController>().IsMoabClassBloon();
                var isCeramicBloon = enemy.gameObject.GetComponent<BloonController>().IsCeramicBloon();

                var bloonsPopped = BloonsPoppedAmount(enemy);

                // Destroying bloons with shield
                if ((isMoabClassBloon || isCeramicBloon) && !_bonusDamageForCeramic)
                {
                    // Break the bloon shield
                    enemy.gameObject.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);

                    var hitsLeft = enemy.gameObject.GetComponent<BloonController>().LayerDestroyed();

                    // This is complicated. The shield bloon (ceramic, moab) after being destroyed still can affect
                    // weaker enemies (hitsLeft > 0), but it depends on the shield remaining health
                    if (hitsLeft == 0)
                    {
                        Destroy(enemy.gameObject);
                        transform.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                    }
                    else if(hitsLeft > 0)
                    {
                        // there is +1 for some reason. We will understand why it needs to be like this

                        if (!isMoabClassBloon)
                        {
                            transform.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                            enemy.gameObject.GetComponent<BloonController>().SetIsPopThrough(hitsLeft + 1);
                            Destroy(enemy.gameObject);
                        }
                        else
                        {
                            enemy.gameObject.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);
                            Destroy(enemy.gameObject);
                        }
                    }

                }
                // Destroying bloons without shield
                else
                {
                    // how much bloons the sniper popped
                    transform.gameObject.GetComponent<ManageTower>().BloonsPoppedUp(bloonsPopped);
                    
                    // PopThrough is being set to destroy multiple layers of the bloons at once
                    enemy.gameObject.GetComponent<BloonController>().SetIsPopThrough(_damage);
                    Destroy(enemy.gameObject);
                }

                // Animation
                var fireSprite = GetComponentInChildren<SpriteRenderer>().transform.GetChild(0);
                StartCoroutine(ActivateFireAnimation(fireSprite));
            }

        }
    }

    private int BloonsPoppedAmount(Transform enemy)
    {
        var bloonRbe = enemy.gameObject.GetComponent<BloonController>().getRbe();
        if (bloonRbe < _damage)
            return bloonRbe;
        else
            return _damage;
    }

    private IEnumerator ActivateFireAnimation(Transform fireSprite)
    {
        fireSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        fireSprite.gameObject.SetActive(false);
    }
}
