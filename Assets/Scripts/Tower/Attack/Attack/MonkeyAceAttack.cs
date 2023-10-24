using UnityEngine;

public class MonkeyAceAttack : TowerAttack
{
    private BulletSet _bulletSet;

    // Attack every time even if there is no enemy
    public void Update()
    {
        _delayTimer += Time.deltaTime;
        if (_delayTimer >= _delay)
        {
            // Attack
            _bulletSet = GetPooledObject().GetComponent<BulletSet>();
            if(_bulletSet != null)
            {
                _bulletSet.Attack();
                _delayTimer = 0.0f;
            }
        }
    }
}
