using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private LevelDifficultyData _standardStats;
    [SerializeField] private TextMeshProUGUI _lifesText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _levelText;
    private Level _level;
    private int _lifesAmount;
    private int _moneyAmount;
    private int _levelNumber;

    public static PlayerStats Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _lifesAmount = _standardStats.numberOfLifes;
        _moneyAmount = _standardStats.moneyAtTheBeginning;
        _levelNumber = 1;
        _level = GameObject.Find("Map").GetComponent<Level>();

        _levelText.text = _levelNumber.ToString();
        _lifesText.text = _lifesAmount.ToString();
        _moneyText.text = _moneyAmount.ToString();
    }

    private void Update()
    {
        ChangeLevelInInterface();
    }

    private void ChangeLevelInInterface()
    {
        _levelNumber = _level.GetLevel().levelNumber;
        _levelText.text = _levelNumber.ToString();
    }

    public void AddMoneyForBloonPop()
    {
        _moneyAmount += 1;
        _moneyText.text = _moneyAmount.ToString();
    }

    public void AddMoneyForSoldTower(int moneyAmount)
    {
        _moneyAmount += moneyAmount;
        _moneyText.text = _moneyAmount.ToString();
    }

    public void DecreaseMoneyForBoughtTower(int moneyAmount)
    {
        _moneyAmount -= moneyAmount;
        _moneyText.text = _moneyAmount.ToString();
    }

    public void DecreaseLifeAmount(int rbe)
    {
        if(_lifesAmount > 0)
        {
            _lifesAmount -= rbe;
            _lifesText.text = _lifesAmount.ToString();
        }
    }
}
