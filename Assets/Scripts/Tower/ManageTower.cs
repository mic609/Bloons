// Everything that the player sees in the upgrade panel is managed here

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManageTower : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] private TowerData _towerInfo; // the tower cost, name etc.
    [SerializeField] private float _sellDiscount;

    [Header("SFX")]
    [SerializeField] private AudioClip _sellTowerSound;

    [Header("Upgrades")]
    [SerializeField] private List<UpgradeData> _upgrades;
    private int _upgradeIndex;

    private int _bloonsPoppedInt;

    private int _onClickCounter = 0;

    // upgrade Panel
    private GameObject _upgradePanelToShow;
    private TextMeshProUGUI _towerName;
    private TextMeshProUGUI _bloonsPopped;
    private TextMeshProUGUI _sellText;
    private GameObject _upgradeButton;

    private void Start()
    {
        _upgradePanelToShow = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel");
        _towerName = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "TowerName").GetComponent<TextMeshProUGUI>();
        _bloonsPopped = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "BloonsPoppedCount").GetComponent<TextMeshProUGUI>();
        _sellText = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "SellButtonText").GetComponent<TextMeshProUGUI>();
        _upgradeButton = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradeButton");
        _bloonsPopped.text = "0";
        _bloonsPoppedInt = 0;
        _upgradeIndex = 0;
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

            if(_upgradeIndex >= _upgrades.Count)
                _upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "MAX UPGRADES";
            else
            {
                _upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    _upgrades[_upgradeIndex].upgradeDescription + " (" + _upgrades[_upgradeIndex].upgradeNumber + "): "
                    + _upgrades[_upgradeIndex].upgradeCost + "$"; ;
            }

            var standardCost = _towerInfo.standardPrice;
            var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
            var towerCost = standardCost + standardCost * levelDifficulty.upgradeCost;

            _sellText.text = "Sell for: " + Mathf.RoundToInt(towerCost - towerCost * _sellDiscount).ToString();
        }
        else
        {
            var towerRange = gameObject.transform.Find("Range");
            towerRange.GetComponent<SpriteRenderer>().enabled = false;

            _upgradePanelToShow.SetActive(false);
        }
    }

    // Used by button
    public void SellTower()
    {
        var standardCost = PlayerStats.Instance.GetClickedTower().GetComponent<ManageTower>().GetTowerInfo().standardPrice;
        var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
        var towerCost = Mathf.RoundToInt(standardCost + standardCost * levelDifficulty.upgradeCost);

        var sellDiscount = _sellDiscount;
        var moneyToAdd = Mathf.RoundToInt(towerCost - towerCost * sellDiscount);

        // Play Sound
        SoundManager.Instance.PlaySound(_sellTowerSound);

        PlayerStats.Instance.AddMoneyForSoldTower(moneyToAdd);
        PlayerStats.Instance.DeleteInstantiatedTower();
        PlayerStats.Instance.ForgetClickedTower();

        Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel").SetActive(false);
    }

    // If the tower is chosen change bloons popped text in the interface while bloons are being popped
    private void UpdateBloonsPoppedText()
    {
        if(PlayerStats.Instance.GetClickedTower() != null)
        {
            _bloonsPopped.text = PlayerStats.Instance.GetClickedTower().GetComponent<ManageTower>()._bloonsPoppedInt.ToString();
        }
    }

    // button will use this method
    public void Upgrade()
    {
        var towerToUpgrade = PlayerStats.Instance.GetClickedTower();
        if (towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex < towerToUpgrade.GetComponent<ManageTower>()._upgrades.Count)
        {
            var bullets = towerToUpgrade.GetComponent<TowerAttack>().GetBullets();
            var upgrade = towerToUpgrade.GetComponent<ManageTower>()._upgrades[towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex];

            // upgrade bullets
            foreach (var bullet in bullets)
            {
                if(bullet.GetComponent<Projectile>() != null)
                {
                    var dart = bullet.GetComponent<Projectile>();
                    dart.UpgradeBullet(upgrade);
                }
                else if(bullet.GetComponent<BulletSet>() != null)
                {
                    var bulletSet = bullet.GetComponent<BulletSet>();
                    bulletSet.UpgradeBullet(upgrade);
                }
            }
            // upgrade tower data
            towerToUpgrade.GetComponent<TowerAttack>().SetDelay(upgrade.delay);
            if(towerToUpgrade.GetComponent<RangeCollider>() != null)
                towerToUpgrade.GetComponent<RangeCollider>().SetRadius(upgrade.radius);
            towerToUpgrade.GetComponent<TowerAttack>().SetDamage(upgrade.damage);
            if (towerToUpgrade.GetComponentInChildren<OnMouseTowerRotation>() != null)
                towerToUpgrade.GetComponentInChildren<OnMouseTowerRotation>().SetRotationSpeed(upgrade.rotationSpeed);

            PlayerStats.Instance.DecreaseMoneyForBoughtTower(upgrade.upgradeCost);

            towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex++;
            if (towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex >= towerToUpgrade.GetComponent<ManageTower>()._upgrades.Count)
                towerToUpgrade.GetComponent<ManageTower>()._upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
                    = "MAX UPGRADES";
            else
            {
                upgrade = towerToUpgrade.GetComponent<ManageTower>()._upgrades[towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex];
                towerToUpgrade.GetComponent<ManageTower>()._upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
                    = upgrade.upgradeDescription + " (" + upgrade.upgradeNumber + "): " + upgrade.upgradeCost + "$";
            }
        }
    }

    //////////////////////////////
    // Getters and setters
    //////////////////////////////

    public void BloonsPoppedUp(int bloonsAmount)
    {
        _bloonsPoppedInt += bloonsAmount;
    }

    public TowerData GetTowerInfo()
    {
        return _towerInfo;
    }

    public float GetSellDiscount()
    {
        return _sellDiscount;
    }

    public int GetNumberOfClicks()
    {
        return _onClickCounter;
    }

    public void Click()
    {
        _onClickCounter++;
    }
}
