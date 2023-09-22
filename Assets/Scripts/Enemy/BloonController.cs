using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BloonController : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private GameObject _weakerEnemy; // weaker enemy prefab
    [SerializeField] private int _enemyAmount; // enemies count after bloon defeat
    [SerializeField] private int _bloonInOrder;

    [Header("Health")]
    [SerializeField] private int _rbe;

    [Header("Parent")]
    private GameObject _parent;
    
    private EnemyMovement _enemyMovement;
    private bool _isAppQuitting = false;

    private void Start()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _parent = GameObject.Find("BloonHolder");
        gameObject.transform.SetParent(_parent.transform);
    }

    public GameObject SpawnWeakerEnemy()
    {
        return _weakerEnemy;
    }

    //public void SpawnEnemyLayers()
    //{
    //    var firstObject = true;
    //    var speedOfTheHighestBloon = 0f;

    //    // initial object to spawn
    //    var enemyToSpawn = gameObject;

    //    // We want to spawn object with all its inside layers
    //    do
    //    {
    //        var instantiatedEnemy = Instantiate(enemyToSpawn);

    //        if (firstObject)
    //        {
    //            speedOfTheHighestBloon = instantiatedEnemy.GetComponent<EnemyMovement>().GetSpeed();
    //        }
    //        else // layers inside the bloon need to be invisible and the collider of
    //             // the weaker layer needs to be turned off
    //        {
    //            var objMaterial = new Material(instantiatedEnemy.GetComponent<Renderer>().material);
    //            Color newColor = new Color(objMaterial.color.r, objMaterial.color.g, objMaterial.color.b, 0f);
    //            objMaterial.color = newColor;
    //            instantiatedEnemy.GetComponent<Renderer>().material = objMaterial;
    //            instantiatedEnemy.GetComponent<Collider2D>().enabled = false; // disable collider of the weaker layer

    //            // All inner bloons need to have the same speed as the bigger bloon at the beginning
    //            instantiatedEnemy.GetComponent<EnemyMovement>().SetSpeed(speedOfTheHighestBloon);
    //        }

    //        // Get weaker enemy
    //        instantiatedEnemy = instantiatedEnemy.GetComponent<BloonController>().GetWeakerEnemy();
    //        if (instantiatedEnemy == null)
    //            break;

    //        enemyToSpawn = instantiatedEnemy;

    //        firstObject = false;

    //    } while (enemyToSpawn != null);

    //    enemyToSpawn = gameObject;
    //}

    private void OnDestroy()
    {
        // The app is still running
        if (!_isAppQuitting)
        {
            if (_weakerEnemy != null)
            {
                var newBloon = Instantiate(_weakerEnemy, transform.position, transform.rotation);
                var newEnemyMovement = newBloon.GetComponent<EnemyMovement>();
                var oldEnemyMovement = gameObject.GetComponent<EnemyMovement>();

                newEnemyMovement.SetCurrentDistance(oldEnemyMovement.GetCurrentDistance());
                newEnemyMovement.SetCurrentPosition(oldEnemyMovement.GetCurrentPosition());
                newEnemyMovement.SetPointsIndex(oldEnemyMovement.GetPointsIndex());
                newEnemyMovement.SetProgress(oldEnemyMovement.GetProgress());
            }
        }
        //if (!_isAppQuitting)
        //{
        //    var objMaterial = new Material(_enemyToReplace.GetComponent<Renderer>().material);
        //    Color newColor = new Color(objMaterial.color.r, objMaterial.color.g, objMaterial.color.b, 255f);
        //    objMaterial.color = newColor;
        //    _enemyToReplace.GetComponent<Renderer>().material = objMaterial;
        //    _enemyToReplace.GetComponent<Collider2D>().enabled = true;

        //    _enemyToReplace.GetComponent<EnemyMovement>().SetDefaultSpeed();
        //}
    }

    //public void SendEnemy(GameObject newEnemy)
    //{
    //    _enemyToReplace = newEnemy;
    //}

    private void OnApplicationQuit()
    {
        _isAppQuitting = true;
    }

    public GameObject GetWeakerEnemy()
    {
        return _weakerEnemy;
    }

    public int GetBloonInOrder()
    {
        return _bloonInOrder;
    }
}
