// All of the player game stats

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    // list of towers
    private List<GameObject> _towers;

    // clicked tower by user
    private GameObject _clickedTower;

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
        _towers = new List<GameObject>();
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
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedTower();
        }
    }

    public void SetGameSpeed()
    {
        if (Time.timeScale == 1.0f)
            Time.timeScale = 2.5f;
        else
            Time.timeScale = 1.0f;
    }

    public void DetectClickedTower()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction);

        // We just want to click somewhere else to close tower interface
        if (_clickedTower != null)
        {
            // "Somewhere else" is not the tower interface
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _clickedTower.GetComponent<ManageTower>().ShowUpgradePanel();
                _clickedTower = null;
            }
        }
        // The tower is being chosen already by the player
        else if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tower"))
            {
                _clickedTower = hit.collider.gameObject;
                _clickedTower.GetComponent<ManageTower>().ShowUpgradePanel();
            }
        }
    }

    public void AddInstantiatedTower(ref GameObject newTower)
    {
        _towers.Add(newTower);
    }

    public void DeleteInstantiatedTower()
    {
        _towers.Remove(_clickedTower);
        Destroy(_clickedTower);
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
            if (_lifesAmount < 0)
                _lifesAmount = 0;
            _lifesText.text = _lifesAmount.ToString();
        }
    }

    public GameObject GetClickedTower()
    {
        return _clickedTower;
    }

    public void ForgetClickedTower()
    {
        _clickedTower = null;
    }
}
