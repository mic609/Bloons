using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BloonController : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private GameObject _weakerEnemy;
    [SerializeField] private int _enemyAmount; // enemies count after bloon defeat

    [Header("Health")]
    [SerializeField] private int _rbe;
    
    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
    }

    private void OnDestroy()
    {
        // The app is still running
        if (!_isAppQuitting)
        {
            if (_weakerEnemy != null)
            {
                GameObject newBloon = Instantiate(_weakerEnemy, transform.position, transform.rotation);
                EnemyMovement newBloonController = newBloon.GetComponent<EnemyMovement>();

                newBloon.GetComponent<EnemyMovement>() = _enemyMovement;
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }
}
