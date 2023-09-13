using UnityEngine;

public class BloonController : MonoBehaviour
{
    private EnemyMovement _enemyMovement;

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
    }
}
