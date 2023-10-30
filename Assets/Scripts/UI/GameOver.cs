using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    public void ButtonRestart()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        Time.timeScale = 1.0f;
        BloonController.SetIsChangingScene(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ButtonBackToMenu()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        Time.timeScale = 1.0f;
        BloonController.SetIsChangingScene(true);
        SceneManager.LoadScene("MainMenu");
    }
}
