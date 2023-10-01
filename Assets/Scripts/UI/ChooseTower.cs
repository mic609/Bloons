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
            // Move the tower
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(_moveableTower != null)
                _moveableTower.transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);

            // Check what the tower collides with right now
            for (int i = 0; i < _layersNotToPlaceTower.Count; i++)
            {
                _colliders = Physics2D.OverlapCircleAll(_mousePosition, 0.7f, _layersNotToPlaceTower[i]);

                if (_colliders.Length > 0)
                    break;
            }

            // Change range color if neccesary
            ChangeRangeColor();

            // Place tower when the players clicks the mouse button
            if (Input.GetMouseButtonDown(0))
            {
                if(TowerCanBePlaced())
                    PlaceTower();
            }
        }
    }

    // Change the tower range color based on the info if the tower can be placed somewhere or not
    private void ChangeRangeColor()
    {
        if(_towerRange != null)
        {
            if (_colliders.Length > 0 || EventSystem.current.IsPointerOverGameObject())
            {
                _towerRange.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, _towerColor.a);
            }
            else
            {
                _towerRange.GetComponent<SpriteRenderer>().color = _towerColor;
            }
        }
    }

    // on button click
    public void SelectTower()
    {
        // I want to buy the tower! :>
        if (_moveableTower == null)
        {
            var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
            var towerPrice = _towerToPlace.GetComponent<ManageTower>().GetTowerInfo().standardPrice;

            // If the player has enough money to buy the tower
            if (PlayerStats.Instance.GetMoneyAmount() >= Mathf.RoundToInt(towerPrice + towerPrice * levelDifficulty.upgradeCost))
            {
                _moveableTower = Instantiate(_towerToPlace, gameObject.transform.position, gameObject.transform.rotation);

                // While deciding where to place the tower, the tower should not attack, detect etc.
                _moveableTower.GetComponent<TowerAttack>().enabled = false;
                _moveableTower.GetComponent<RangeCollider>().enabled = false;
                _moveableTower.GetComponent<CircleCollider2D>().enabled = false;

                // Set tower range to visible
                _towerRange = _moveableTower.transform.Find("Range");
                _towerRange.GetComponent<SpriteRenderer>().enabled = true;
                _towerColor = _towerRange.GetComponent<SpriteRenderer>().color;

                _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _isTowerMoving = true;
            }
            else
            {
                // not enough money
            }
        }
        // I don't wanna buy this tower ;((((
        else
        {
            Destroy(_moveableTower);
        }
    }

    // Check if the tower is not going to be placed on path or other monkey or interface
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }
        return true;
    }

    private void PlaceTower()
    {
        _isTowerMoving = false;

        // Activate Tower Components
        _moveableTower.GetComponent<TowerAttack>().enabled = true;
        _moveableTower.GetComponent<RangeCollider>().enabled = true;
        _moveableTower.GetComponent<CircleCollider2D>().enabled = true;
        
        // Set Range to invisible
        _towerRange = _moveableTower.transform.Find("Range");
        _towerRange.GetComponent<SpriteRenderer>().enabled = false;

        // Add new tower to the global list of current towers
        PlayerStats.Instance.AddInstantiatedTower(ref _moveableTower);

        // Set position of the placed tower
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _moveableTower.transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);

        // Check the tower price and decrease player's money
        var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
        var towerPrice = _towerToPlace.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
        PlayerStats.Instance.DecreaseMoneyForBoughtTower(Mathf.RoundToInt(towerPrice + towerPrice * levelDifficulty.upgradeCost));

        _moveableTower = null;
    }

    public int GetTowerCost()
    {
        return _towerToPlace.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
    }
}
