using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseTower : MonoBehaviour
{
    [SerializeField] private GameObject _towerToPlace;
    [SerializeField] private List<LayerMask> _layersNotToPlaceTower;
    private GameObject _moveableTower;
    private Transform _towerRange;
    private Color _towerColor;
    private Vector3 _mousePosition;
    private bool _isTowerMoving;
    private Collider2D[] _colliders; 

    private void Start()
    {
        _colliders = new Collider2D[_layersNotToPlaceTower.Count];
        _isTowerMoving = false;
    }

    private void Update()
    {
        if(_isTowerMoving)
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _moveableTower.transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);

            for (int i = 0; i < _layersNotToPlaceTower.Count; i++)
            {
                _colliders = Physics2D.OverlapCircleAll(_mousePosition, 0.7f, _layersNotToPlaceTower[i]);

                if (_colliders.Length > 0)
                    break;
            }

            ChangeRangeColor();

            if (Input.GetMouseButtonDown(0))
            {
                if(TowerCanBePlaced())
                    PlaceTower();
            }
        }
    }

    private void ChangeRangeColor()
    {
        if (_colliders.Length > 0)
        {
           _towerRange.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, _towerColor.a);
        }
        else
        {
            _towerRange.GetComponent<SpriteRenderer>().color = _towerColor;
        }
    }

    // on button click
    public void SelectTower()
    {
        _moveableTower = Instantiate(_towerToPlace, gameObject.transform.position, gameObject.transform.rotation);

        _moveableTower.GetComponent<TowerAttack>().enabled = false;
        _moveableTower.GetComponent<RangeCollider>().enabled = false;
        _moveableTower.GetComponent<CircleCollider2D>().enabled = false;

        _towerRange = _moveableTower.transform.Find("Range");
        _towerRange.GetComponent<SpriteRenderer>().enabled = true;
        _towerColor = _towerRange.GetComponent<SpriteRenderer>().color;

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _isTowerMoving = true;
    }

    // Check if the tower is not going to be placed on path or other monkey
    private bool TowerCanBePlaced()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for(int i = 0; i <_layersNotToPlaceTower.Count; i++)
        {
            if(_colliders.Length > 0)
            {
                return _colliders.Length == 0;
            }
        }
        return true;
    }

    public void SellTower()
    {
        var towerCost = _moveableTower.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
        var sellDiscount = _moveableTower.GetComponent<ManageTower>().GetSellDiscount();
        var moneyToAdd = Mathf.RoundToInt(towerCost - towerCost * sellDiscount);

        PlayerStats.Instance.AddMoneyForSoldTower(moneyToAdd);

        Destroy(_moveableTower);
        Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "UpgradePanel").SetActive(false);
    }

    private void PlaceTower()
    {
        _isTowerMoving = false;

        _moveableTower.GetComponent<TowerAttack>().enabled = true;
        _moveableTower.GetComponent<RangeCollider>().enabled = true;
        _moveableTower.GetComponent<CircleCollider2D>().enabled = true;
        _towerRange = _moveableTower.transform.Find("Range");
        _towerRange.GetComponent<SpriteRenderer>().enabled = false;

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _moveableTower.transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);

        var towerCost = _moveableTower.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
        PlayerStats.Instance.DecreaseMoneyForBoughtTower(towerCost);
    }
}
