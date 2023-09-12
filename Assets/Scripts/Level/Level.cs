using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] GameObject _objectToSpawn;
    [SerializeField] Transform _whereToSpawn;

    [SerializeField] private float _timeBetweenSpawn;

    private void Awake()
    {
        Transform spawnObjectTransform = _objectToSpawn.transform;
        spawnObjectTransform.position = _whereToSpawn.position; // we set only position
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        for(int i = 0; i < 10; i++)
        {
            Instantiate(_objectToSpawn);
            yield return new WaitForSeconds(_timeBetweenSpawn);
        }
    }
}
