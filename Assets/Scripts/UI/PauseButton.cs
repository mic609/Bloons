using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    private Transform _pauseScreen;
    private float _previousGameSpeed;

    private void Start()
    {
        _pauseScreen = transform.Find("PauseScreen");
    }

    // button click
    public void OpenSettingsPanel()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        _previousGameSpeed = Time.timeScale;
        Time.timeScale = 0f;
        _pauseScreen.gameObject.SetActive(true);
    }

    // button click
    public void BackToMenu()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        Time.timeScale = 1.0f;
        BloonController.SetIsChangingScene(true);
        SceneManager.LoadScene("MainMenu");
    }

    // button click
    public void ContinueGame()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        Time.timeScale = _previousGameSpeed;
        _pauseScreen.gameObject.SetActive(false);
    }

    // button click
    public void RestartGame()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        Time.timeScale = 1.0f;
        BloonController.SetIsChangingScene(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
