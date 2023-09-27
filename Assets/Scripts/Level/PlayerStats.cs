using TMPro;
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

    private void Start()
    {
        _lifesAmount = _standardStats.numberOfLifes;
        _moneyAmount = _standardStats.moneyAtTheBeginning;
        _levelNumber = 1;
        _level = GameObject.Find("Map").GetComponent<Level>();

        Debug.Log("Start: " + _level);

        _levelText.text = _levelNumber.ToString();
        _lifesText.text = _lifesAmount.ToString();
        _moneyText.text = _moneyAmount.ToString();
    }

    private void Update()
    {
        ChangeLevelInInterface();
    }

    public void ChangeLevelInInterface()
    {
        _levelNumber = _level.GetLevel().levelNumber;
        _levelText.text = _levelNumber.ToString();
    }

    public void AddMoneyForBloonPop()
    {
        _moneyAmount += 1;
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
