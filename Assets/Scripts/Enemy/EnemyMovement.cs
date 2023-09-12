// The script describes the movement of the bloon

using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] _points;

    [Header("Enemy Details")]
    [SerializeField] private float _movementSpeed;

    private Vector3 _currentPosition;
    private int _pointsIndex = 0;

    private void Start()
    {
        _currentPosition = transform.position; // position of the bloon
    }

    private void Update()
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

    private void MoveToPoint(Vector2 lastPoint, Transform nextPoint)
    {
        _currentPosition = Vector3.MoveTowards(_currentPosition, nextPoint.transform.position, _movementSpeed * Time.deltaTime); // smooth move from point to another
        transform.position = _currentPosition;
    }
}
