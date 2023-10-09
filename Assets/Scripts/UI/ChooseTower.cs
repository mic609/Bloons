using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseTower : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip _chooseTowerSound;
    [SerializeField] private AudioClip _placeTowerSound;

    [Header("Tower")]
    [SerializeField] private GameObject _towerToPlace;
    [SerializeField] private List<LayerMask> _layersNotToPlaceTower;

    private static GameObject _moveableTower; // tower itself

    private Transform _towerRange; // range of the tower
    private Color _towerColor; // color of the tower range
    private Vector3 _mousePosition;
    [SerializeField] private bool _isTowerMoving;
    private Collider2D[] _colliders; // what the moving tower collides with

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

            if (_moveableTower != null)
                _moveableTower.transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);
            else if (_moveableTower == null)
                _isTowerMoving = false;

            // Check what the tower collides with right now
            for (int i = 0; i < _layersNotToPlaceTower.Count; i++)
            {
                _colliders = Physics2D.OverlapCircleAll(_mousePosition, 0.7f, _layersNotToPlaceTower[i]);

                if (_colliders.Length > 0)
                    break;
            }

            if(_isTowerMoving) // just to be sure
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
        if (_moveableTower == null || _moveableTower.GetComponent<ManageTower>().name != (_towerToPlace.name + "(Clone)"))
        {
            var levelDifficulty = PlayerStats.Instance.GetLevelDifficulty();
            var towerPrice = _towerToPlace.GetComponent<ManageTower>().GetTowerInfo().standardPrice;

            // If the player has enough money to buy the tower
            if (PlayerStats.Instance.GetMoneyAmount() >= Mathf.RoundToInt(towerPrice + towerPrice * levelDifficulty.upgradeCost))
            {
                if(_moveableTower != null)
                {
                    Destroy(_moveableTower);
                    _moveableTower = Instantiate(_towerToPlace, gameObject.transform.position, gameObject.transform.rotation);
                }
                else
                    _moveableTower = Instantiate(_towerToPlace, gameObject.transform.position, gameObject.transform.rotation);

                // sound
                SoundManager.Instance.PlaySound(_chooseTowerSound);

                // Change radius size
                if(_moveableTower.GetComponent<RangeCollider>() != null)
                    _moveableTower.GetComponent<RangeCollider>().ChangeRadiusSize();

                // While deciding where to place the tower, the tower should not attack, detect etc.
                if (_moveableTower.GetComponent<SniperMonkeyAttack>() != null)
                    _moveableTower.GetComponent<SniperMonkeyAttack>().enabled = false;
                if (_moveableTower.GetComponent<TowerAttack>() != null)
                    _moveableTower.GetComponent<TowerAttack>().enabled = false;
                if (_moveableTower.GetComponent<RangeCollider>() != null)
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
            _isTowerMoving = false;
            Destroy(_moveableTower);
            _moveableTower = null;
        }
    }

    // Check if the tower is not going to be placed on path or other monkey or interface
    private bool TowerCanBePlaced()
    {
        if (_moveableTower != null)
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            for (int i = 0; i < _layersNotToPlaceTower.Count; i++)
            {
                if (_colliders.Length > 0)
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
        return false;
    }

    private void PlaceTower()
    {
        _isTowerMoving = false;

        // Play sound
        SoundManager.Instance.PlaySound(_placeTowerSound);

        // Activate Tower Components
        if (_moveableTower.GetComponent<SniperMonkeyAttack>() != null)
            _moveableTower.GetComponent<SniperMonkeyAttack>().enabled = true;
        if (_moveableTower.GetComponent<TowerAttack>() != null)
            _moveableTower.GetComponent<TowerAttack>().enabled = true;
        if (_moveableTower.GetComponent<RangeCollider>() != null)
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
        var towerPrice = _moveableTower.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
        PlayerStats.Instance.DecreaseMoneyForBoughtTower(Mathf.RoundToInt(towerPrice + towerPrice * levelDifficulty.upgradeCost));

        _moveableTower = null;
    }

    public int GetTowerCost()
    {
        return _towerToPlace.GetComponent<ManageTower>().GetTowerInfo().standardPrice;
    }
}
