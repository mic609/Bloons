using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    [SerializeField] private GameObject _level;

    public void LoadNextLevel()
    {
        if (_level.GetComponent<Level>().GetNumberOfBloons() <= 0)
        {
            SoundManager.Instance.PlaySound(_buttonClick);
            _level.GetComponent<Level>().SwitchLevel();
            _level.GetComponent<Level>().spawnEnemyCoroutine();
        }
    }
}
