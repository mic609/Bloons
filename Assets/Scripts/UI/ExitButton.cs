using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    private Transform _exitScreen;

    private void Start()
    {
        _exitScreen = transform.Find("ExitScreen");
    }

    public void OpenExitScreen()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        _exitScreen.gameObject.SetActive(true);
    }

    public void CloseExitScreen()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        _exitScreen.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        QuitGame();
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
                Application.Quit();
    }
}
