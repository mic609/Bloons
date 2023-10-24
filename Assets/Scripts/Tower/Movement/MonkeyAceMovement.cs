using UnityEngine;

public class MonkeyAceMovement : MonoBehaviour
{

    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _radius;

    private Vector3 _centre; // centre of the circle
    private float _angle;
    private float _angleInDegrees;
    private float _going; // define the circle which the plane moves around
    private Vector3 _startingPosition;
    private Vector3 _offset;

    private void Start()
    {
        _going = 1.0f;
        _centre = transform.position;
        _offset = new Vector3(Mathf.Sin(_angle), _going * Mathf.Cos(_angle), transform.position.z) * _radius;
        _startingPosition = transform.position - _offset;
    }

    private void Update()
    {

        _angle += _rotateSpeed * Time.deltaTime;
        _angleInDegrees = Mathf.Rad2Deg * _angle;

        _offset = new Vector3(Mathf.Sin(_angle), _going * Mathf.Cos(_angle), transform.position.z) * _radius;
        transform.position = _centre - _offset;

        if(transform.childCount == 0)
            transform.rotation = Quaternion.Euler(0, 0, (-1) * _going * _angleInDegrees);

        if (_angleInDegrees >= 360)
        {
            _centre = new Vector3(_startingPosition.x, _startingPosition.y - _offset.y, _startingPosition.z);
            _angle = 0.0f;
            _going *= -1.0f;
        }
    }
}