// The script describes the movement of the bloon

using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Details")]
    [SerializeField] private float _movementSpeed; // this is original bloon speed given in the inspector (cannot change)
    private float _currentMovementSpeed; // this is a value that can change in certain situations

    [Header("Level")]
    [SerializeField] private GameObject _levelObject;
    private Level _level;

    private Vector3 _currentPosition; // current point
    private int _pointsIndex = 0;

    private float _currentDistance = 0f; // auxiliary variable
    [SerializeField] private float _progress = 0f; // percentage of the path completed by a bloon

    private void Start()
    {
        _currentPosition = transform.position; // position of the bloon
        _level = _levelObject.GetComponent<Level>();
    }

    private void Awake()
    {
        _currentMovementSpeed = _movementSpeed;
    }

    private void Update()
    {
        var points = _level.GetPoints();

        if (_pointsIndex < points.Length)
        {
            MoveToPoint(_currentPosition, points[_pointsIndex]); // move

            if (_currentPosition == points[_pointsIndex].transform.position)
            {
                _pointsIndex++;
            }
        }
    }

    private void MoveToPoint(Vector2 lastPoint, Transform nextPoint)
    {
        _currentPosition = Vector3.MoveTowards(_currentPosition, nextPoint.transform.position, _currentMovementSpeed * Time.deltaTime); // smooth move from point to another
        transform.position = _currentPosition;

        float movement = _currentMovementSpeed * Time.deltaTime;
        _currentDistance += movement;

        _progress = _currentDistance / _level.GetPathLength();
    }

    //////////////////////////////////////
    // Get and Set
    //////////////////////////////////////

    public void SetCurrentPosition(Vector3 currentPosition)
    {
        _currentPosition = currentPosition;
    }

    public Vector3 GetCurrentPosition()
    {
        return _currentPosition;
    }

    public void SetPointsIndex(int pointsIndex)
    {
        _pointsIndex = pointsIndex;
    }

    public int GetPointsIndex()
    {
        return _pointsIndex;
    }

    public void SetCurrentDistance(float currentDistance)
    {
        _currentDistance = currentDistance;
    }

    public float GetCurrentDistance()
    {
        return _currentDistance;
    }

    public void SetProgress(float progress)
    {
        _progress = progress;
    }

    public float GetProgress()
    {
        return _progress;
    }
}
