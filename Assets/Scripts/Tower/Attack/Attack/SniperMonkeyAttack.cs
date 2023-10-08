using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SniperMonkeyAttack : MonoBehaviour
{
    [Header("Attack Details")]
    [SerializeField] private float _delay;
    [SerializeField] private int _damage; // how many bloons the tower can pop through
    private bool _bonusDamageForCeramic;
    
    private GameObject _bloons;
    
    private TowerRotation _towerRotation;
    private float _delayTimer;

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
            // first target shoot
            StartAttack(ChooseFirstTarget());
        }
    }

    // When enemy in range
    public void StartAttack(Transform enemy)
    {
        if (enemy != null) // there is enemy
        {
            _delayTimer += Time.deltaTime;
            if (_delayTimer >= _delay)
            {
                // Rotate Tower towards target
                _towerRotation.Rotate(enemy);

                // Attack with sound
                _delayTimer = 0.0f;
                SoundManager.Instance.PlaySound(enemy.GetComponent<BloonController>().GetPopSound(false));

                var isMoabClassBloon = enemy.gameObject.GetComponent<BloonController>().IsMoabClassBloon();
                var isCeramicBloon = enemy.gameObject.GetComponent<BloonController>().IsCeramicBloon();

                if ((isMoabClassBloon || isCeramicBloon) && !_bonusDamageForCeramic)
                {
                    enemy.gameObject.GetComponent<BloonController>().DestroyLayeredEnemy(null, _damage);
                    if (enemy.gameObject.GetComponent<BloonController>().LayerDestroyed())
                        Destroy(enemy.gameObject);
                }
                else
                {
                    enemy.gameObject.GetComponent<BloonController>().SetIsPopThrough(_damage);
                    Destroy(enemy.gameObject);
                }

                // Animation
                //_animator.SetTrigger("Attack");
            }

        }
    }

    private Transform ChooseFirstTarget()
    {
        var biggestProgress = 0f;
        Transform targetToReturn = null;

        foreach (Transform enemy in _bloons.transform)
        {
            if (enemy.GetComponent<EnemyMovement>().GetProgress() > biggestProgress)
            {
                biggestProgress = enemy.GetComponent<EnemyMovement>().GetProgress();
                targetToReturn = enemy;
            }
        }
        return targetToReturn;
    }
}
