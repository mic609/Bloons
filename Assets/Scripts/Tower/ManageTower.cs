// Everything that the player sees in the upgrade panel is managed here

using System.Linq;
using TMPro;
using UnityEngine;

public class ManageTower : MonoBehaviour
{
    [SerializeField] private TowerData _towerInfo;
    [SerializeField] private float _sellDiscount;
    
    private int _bloonsPoppedInt;

    // upgrade Panel
    private GameObject _upgradePanelToShow;
    private TextMeshProUGUI _towerName;
    private TextMeshProUGUI _bloonsPopped;
    private TextMeshProUGUI _sellText;

    private void Start()
    {
        _upgradePanelToShow = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel");
        _towerName = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "TowerName").GetComponent<TextMeshProUGUI>();
        _bloonsPopped = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "BloonsPoppedCount").GetComponent<TextMeshProUGUI>();
        _sellText = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "SellButtonText").GetComponent<TextMeshProUGUI>();
        _bloonsPopped.text = "0";
        _bloonsPoppedInt = 0;
    }

    private void Update()
    {
        UpdateBloonsPoppedText();
    }

    public void ShowUpgradePanel()
    {
        if (!_upgradePanelToShow.activeSelf)
        {
            var _towerRange = gameObject.transform.Find("Range");
            _towerRange.GetComponent<SpriteRenderer>().enabled = true;

            _upgradePanelToShow.SetActive(true);
            _bloonsPopped.text = _bloonsPoppedInt.ToString();
            _towerName.text = _towerInfo.towerName;
            _sellText.text = "Sell for: " + Mathf.RoundToInt(_towerInfo.standardPrice - _towerInfo.standardPrice * _sellDiscount).ToString();
        }
        else
        {
            var _towerRange = gameObject.transform.Find("Range");
            _towerRange.GetComponent<SpriteRenderer>().enabled = false;

            _upgradePanelToShow.SetActive(false);
        }
    }

    // Used by button
    public void SellTower()
    {
        var towerCost = _towerInfo.standardPrice;
        var sellDiscount = _sellDiscount;
        var moneyToAdd = Mathf.RoundToInt(towerCost - towerCost * sellDiscount);

        PlayerStats.Instance.AddMoneyForSoldTower(moneyToAdd);
        PlayerStats.Instance.DeleteInstantiatedTower();
        PlayerStats.Instance.ForgetClickedTower();

        Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel").SetActive(false);
    }

    // If the tower is chosen change bloons popped text in the interface while bloons are being popped
    public void UpdateBloonsPoppedText()
    {
        if(PlayerStats.Instance.GetClickedTower() != null)
        {
            _bloonsPopped.text = PlayerStats.Instance.GetClickedTower().GetComponent<ManageTower>()._bloonsPoppedInt.ToString();
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
