using System.Collections;
using TMPro;
using UnityEngine;

public class MessageField : MonoBehaviour
{
    [SerializeField] private GameObject _level;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _messageTime;

    private void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ActivateMessage()
    {
        gameObject.SetActive(true);
        StartCoroutine("ActivateMessageCourutine");
    }

    private IEnumerator ActivateMessageCourutine()
    {
        _textMeshPro.text = _level.GetComponent<Level>().GetCurrentLevel().levelDescription;
        yield return new WaitForSeconds(_messageTime);
        gameObject.SetActive(false);
    }
}
