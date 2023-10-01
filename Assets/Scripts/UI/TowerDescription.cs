using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _fieldToShow;
    private int _counter;

    private void Start()
    {
        _counter = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _fieldToShow.SetActive(true);
        var towerCost = gameObject.GetComponent<ChooseTower>().GetTowerCost() + gameObject.GetComponent<ChooseTower>().GetTowerCost() * PlayerStats.Instance.GetLevelDifficulty().upgradeCost;

        if(_counter == 0)
        {
            _fieldToShow.GetComponentInChildren<TextMeshProUGUI>().text = "Cost: " + towerCost.ToString() + "$\n" + _fieldToShow.GetComponentInChildren<TextMeshProUGUI>().text;
            _counter++;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _fieldToShow.SetActive(false);
    }
}
