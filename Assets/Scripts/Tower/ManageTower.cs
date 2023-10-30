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
    [SerializeField] private AudioClip _upgradeTowerSound;

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

    // sell
    private int _moneyToAdd;
    private float _towerValue;
    private float _towerCost;

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

        var standardCost = _towerInfo.standardPrice;
        var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
        _towerCost = Mathf.RoundToInt(standardCost + standardCost * levelDifficulty.upgradeCost);
        _towerValue = _towerCost;
        _moneyToAdd = Mathf.RoundToInt(_towerValue - _towerValue * _sellDiscount);
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
                var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
                _upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = " (" + _upgrades[_upgradeIndex].upgradeNumber + ") " +
                    Mathf.RoundToInt(_upgrades[_upgradeIndex].upgradeCost +
                    _upgrades[_upgradeIndex].upgradeCost * levelDifficulty.upgradeCost) + "$" + ":\n" +
                    _upgrades[_upgradeIndex].upgradeDescription;
            }
            
            _sellText.text = "Sell for: " + _moneyToAdd.ToString();
        }
        else
        {
            var towerRange = gameObject.transform.Find("Range");
            towerRange.GetComponent<SpriteRenderer>().enabled = false;

            _upgradePanelToShow.SetActive(false);
        }
    }

    private void ChangeSellCost(GameObject towerToUpgrade, UpgradeData upgrade)
    {
        towerToUpgrade.GetComponent<ManageTower>()._towerValue += upgrade.upgradeCost;
        towerToUpgrade.GetComponent<ManageTower>()._moneyToAdd =
            Mathf.RoundToInt(towerToUpgrade.GetComponent<ManageTower>()._towerValue - towerToUpgrade.GetComponent<ManageTower>()._towerValue * _sellDiscount);

        towerToUpgrade.GetComponent<ManageTower>()._sellText.text = "Sell for: " + towerToUpgrade.GetComponent<ManageTower>()._moneyToAdd.ToString();
    }

    // Used by button
    public void SellTower()
    {
        var clickedTower = PlayerStats.Instance.GetClickedTower().GetComponent<ManageTower>();

        clickedTower._moneyToAdd = Mathf.RoundToInt(clickedTower._towerValue - clickedTower._towerValue * _sellDiscount);

        // Play Sound
        SoundManager.Instance.PlaySound(_sellTowerSound);

        PlayerStats.Instance.AddMoneyForSoldTower(clickedTower._moneyToAdd);
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
        var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();

        var towerToUpgrade = PlayerStats.Instance.GetClickedTower();
        if (towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex < towerToUpgrade.GetComponent<ManageTower>()._upgrades.Count)
        {
            var bullets = towerToUpgrade.GetComponent<TowerAttack>().GetBullets();
            var upgrade = towerToUpgrade.GetComponent<ManageTower>()._upgrades[towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex];

            if (PlayerStats.Instance.GetMoneyAmount() >= Mathf.RoundToInt(upgrade.upgradeCost + upgrade.upgradeCost * levelDifficulty.upgradeCost))
            {
                // upgrade bullets
                foreach (var bullet in bullets)
                {
                    if (bullet.GetComponent<Projectile>() != null)
                    {
                        var dart = bullet.GetComponent<Projectile>();
                        dart.UpgradeBullet(upgrade);
                    }
                    else if (bullet.GetComponent<BulletSet>() != null)
                    {
                        var bulletSet = bullet.GetComponent<BulletSet>();
                        bulletSet.UpgradeBullet(upgrade);
                    }
                }

                // upgrade tower data
                towerToUpgrade.GetComponent<TowerAttack>().SetDelay(upgrade.delay);

                if(towerToUpgrade.GetComponent<MonkeyAceAttack>() != null)
                    towerToUpgrade.transform.Find("MonkeyAceSprite").GetComponent<SpriteRenderer>().sprite = upgrade.upgradeSprite;
                else
                    towerToUpgrade.GetComponentInChildren<SpriteRenderer>().sprite = upgrade.upgradeSprite;

                if (towerToUpgrade.GetComponent<RangeCollider>() != null)
                    towerToUpgrade.GetComponent<RangeCollider>().SetRadius(upgrade.radius);
                towerToUpgrade.GetComponent<TowerAttack>().SetDamage(upgrade.damage);
                if (towerToUpgrade.GetComponentInChildren<OnMouseTowerRotation>() != null)
                    towerToUpgrade.GetComponentInChildren<OnMouseTowerRotation>().SetRotationSpeed(upgrade.rotationSpeed);
                if (towerToUpgrade.GetComponent<SniperMonkeyAttack>() != null)
                    towerToUpgrade.GetComponent<SniperMonkeyAttack>().SetCannotPopLead(upgrade.cannotPopLead);

                PlayerStats.Instance.DecreaseMoneyForBoughtTower(Mathf.RoundToInt(upgrade.upgradeCost + upgrade.upgradeCost * levelDifficulty.upgradeCost));
                ChangeSellCost(towerToUpgrade, upgrade);

                towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex++;
                if (towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex >= towerToUpgrade.GetComponent<ManageTower>()._upgrades.Count)
                    towerToUpgrade.GetComponent<ManageTower>()._upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
                        = "MAX UPGRADES";
                else
                {
                    upgrade = towerToUpgrade.GetComponent<ManageTower>()._upgrades[towerToUpgrade.GetComponent<ManageTower>()._upgradeIndex];
                    towerToUpgrade.GetComponent<ManageTower>()._upgradeButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text
                        = " (" + upgrade.upgradeNumber + ") " + Mathf.RoundToInt(upgrade.upgradeCost + upgrade.upgradeCost * levelDifficulty.upgradeCost) + "$" + ":\n" + upgrade.upgradeDescription;
                }

                SoundManager.Instance.PlaySound(_upgradeTowerSound);
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
