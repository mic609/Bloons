// Everything that the player sees in the upgrade panel is managed here

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManageTower : MonoBehaviour
{
    [SerializeField] private TowerData _towerInfo;
    [SerializeField] private float _sellDiscount;

    [SerializeField] private int _bloonsPoppedInt;
    // upgrade Panel
    private GameObject _upgradePanelToShow;
    private TextMeshProUGUI _towerName;
    private TextMeshProUGUI _bloonsPopped;
    private TextMeshProUGUI _sellText;

    private void Start()
    {
        _upgradePanelToShow = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel");
        _towerName = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "TowerName").GetComponent<TextMeshProUGUI>();
        _bloonsPopped = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Count").GetComponent<TextMeshProUGUI>();
        _sellText = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "SellButtonText").GetComponent<TextMeshProUGUI>();
        _bloonsPopped.text = "0";
        _bloonsPoppedInt = 0;
    }

    public void OnMouseDown()
    {
        if (!_upgradePanelToShow.activeSelf)
        {
            _upgradePanelToShow.SetActive(true);
            _bloonsPopped.text = _bloonsPoppedInt.ToString();
            _towerName.text = _towerInfo.towerName;
            _sellText.text = "Sell for: " + Mathf.RoundToInt(_towerInfo.standardPrice - _towerInfo.standardPrice * _sellDiscount).ToString();
        }
        else
        {
            _upgradePanelToShow.SetActive(false);
        }
    }

    public void BloonsPoppedUp()
    {
        _bloonsPoppedInt += 1;
    }

    public TowerData GetTowerInfo()
    {
        return _towerInfo;
    }

    public float GetSellDiscount()
    {
        return _sellDiscount;
    }
}
