// Choosing map in the main menu logic

using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseMap : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    [SerializeField] private string _sceneName;
    private Transform _chooseDifficultyScreen;
    private ExitButton _exitButton;
    private GameObject _map;
    private int _click = 0;

    public void ClickOnMap()
    {
        SoundManager.Instance.PlaySound(_buttonClick);
        _exitButton = FindObjectOfType<ExitButton>();
        _chooseDifficultyScreen = transform.Find("ChooseLevelDifficulty");

        if (gameObject.name == "MapBeach")
        {
            if(_click <= 0)
                _map = GameObject.Find("MapFlowers");
        }
        else
        {
            if (_click <= 0)
                _map = GameObject.Find("MapBeach");
        }
        _click++;

        if (_map != null)
            _map.SetActive(false);

        if (_exitButton != null)
            _exitButton.gameObject.SetActive(false);
        
        _chooseDifficultyScreen.gameObject.SetActive(true);
    }

    // on click
    public void ChooseEasy()
    {
        // remember mode chosen by player
        PlayerPrefs.SetString("Level Difficulty", "Easy");
    }
    // on click
    public void ChooseMedium()
    {
        // remember mode chosen by player
        PlayerPrefs.SetString("Level Difficulty", "Medium");
    }
    // on click
    public void ChooseHard()
    {
        // remember mode chosen by player
        PlayerPrefs.SetString("Level Difficulty", "Hard");
    }
    // on click
    public void ChooseImpoppable()
    {
        // remember mode chosen by player
        PlayerPrefs.SetString("Level Difficulty", "Impoppable");
    }
    // on click
    public void ChooseSandbox()
    {
        // remember mode chosen by player
        PlayerPrefs.SetString("Level Difficulty", "Sandbox");
    }
    public void Back()
    {
        _map.gameObject.SetActive(true);
        _click = 0;

        SoundManager.Instance.PlaySound(_buttonClick);

        if(_exitButton != null)
            _exitButton.gameObject.SetActive(true);
        
        _chooseDifficultyScreen.gameObject.SetActive(false);
    }

    public void LoadMap()
    {
        _map.gameObject.SetActive(true);
        _click = 0;
        SoundManager.Instance.PlaySound(_buttonClick);
        SceneManager.LoadScene(_sceneName);
    }
}
