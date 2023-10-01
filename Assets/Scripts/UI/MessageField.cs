// Message at the end of the level logic

using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MessageField : MonoBehaviour
{
    [SerializeField] private GameObject _level;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _messageTime;
    private string _messageType;

    private void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ActivateMessage(string messageType)
    {
        _messageType = messageType;
        gameObject.SetActive(true);
        StartCoroutine("ActivateMessageCourutine");
    }

    private IEnumerator ActivateMessageCourutine()
    {
        if(_messageType == "GameBegin")
        {
            _textMeshPro.text = PlayerStats.Instance.GetLevelDifficulty().difficultyDescription;
        }
        else if(_messageType == "LevelEnd")
        {
            _textMeshPro.text = _level.GetComponent<Level>().GetCurrentLevel().levelDescription;
        }

        yield return new WaitForSeconds(_messageTime);
        gameObject.SetActive(false);
    }
}
