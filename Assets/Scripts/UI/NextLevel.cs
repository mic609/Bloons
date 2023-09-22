using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private GameObject _level;

    public void LoadNextLevel()
    {
        if (_level.GetComponent<Level>().GetNumberOfBloons() <= 0)
        {
            _level.GetComponent<Level>().SwitchLevel();
            _level.GetComponent<Level>().spawnEnemyCoroutine();
        }
    }
}
