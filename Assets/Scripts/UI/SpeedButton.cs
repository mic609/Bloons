using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedButton : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _buttonClick;

    public void OnButtonClick()
    {
        SoundManager.Instance.PlaySound(_buttonClick);

        if(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Slow")
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fast";
        else
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Slow";
    }
}
