// The script describes the movement of the bloon

using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] _points;

    [Header("Enemy Details")]
    [SerializeField] private float _movementSpeed;

    private Vector3 _currentPosition; // current point
    private int _pointsIndex = 0;

    private float _currentDistance = 0f; // auxiliary variable
    private float _progress = 0f; // percentage of the path completed by a bloon
    private float _pathLength = 0f;

    private void Start()
    {
        _currentPosition = transform.position; // position of the bloon
        CalculatePathLength();
    }

    private void Update()
    {
        if (_pointsIndex < _points.Length)
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

        float movement = _movementSpeed * Time.deltaTime;
        _currentDistance += movement;

        _progress = _currentDistance / _pathLength;
    }

    private void CalculatePathLength()
    {
        var firstCycle = true;
        for(int i = 0; i < (_points.Length - 1); i++)
        {
            if(firstCycle)
            {
                _pathLength += Vector3.Distance(_currentPosition, _points[i].position);
                firstCycle = false;
                i--;
            }
            else
            {
                _pathLength += Vector3.Distance(_points[i].position, _points[i + 1].position);
            }
        }
    }

    public float GetProgress()
    {
        return _progress;
    }
}
