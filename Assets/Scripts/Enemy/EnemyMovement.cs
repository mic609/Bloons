using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _movementSpeed;
    private Vector3 _currentPosition;
    private int _pointsIndex = 0;

    void Start()
    {
        _currentPosition = transform.position; // position of the bloon
    }

    void Update()
    {
        if(_pointsIndex < _points.Length)
        {
            MoveToPoint(_currentPosition, _points[_pointsIndex]); // move

            if (_currentPosition == _points[_pointsIndex].transform.position)
            {
                _pointsIndex++;
            }
        }
    }

    void MoveToPoint(Vector2 lastPoint, Transform nextPoint)
    {
        _currentPosition = Vector3.MoveTowards(_currentPosition, nextPoint.transform.position, _movementSpeed * Time.deltaTime); // smooth move from point to another
        transform.position = _currentPosition;
    }
}
