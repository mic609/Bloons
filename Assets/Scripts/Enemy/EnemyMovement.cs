// The script describes the movement of the bloon

using TMPro;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Details")]
    [SerializeField] private float _movementSpeed; // this is original bloon speed given in the inspector
    private float _currentMovementSpeed; // this is a value that can change in certain situations

    // Level Object contains path points
    [Header("Level")]
    [SerializeField] private GameObject _levelObject;
    private Level _level;

    // Every path has points that bloons approach
    private Vector3 _currentPosition; // current point that bloon passed
    private int _pointsIndex = 0;

    private float _currentDistance = 0f; // auxiliary variable
    private float _progress = 0f; // percentage of the path completed by a bloon

    private void Start()
    {
        _level = _levelObject.GetComponent<Level>();

        _currentPosition = transform.position; // position of the bloon
        gameObject.GetComponent<BloonController>().RotateMoabClassBloon(); // rotate moab bloon if neccesary (based on the initial movement direction)
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
            MoveToPoint(points[_pointsIndex]); // move to the next point

            if (_currentPosition == points[_pointsIndex].transform.position)
            {
                // approach new point
                _pointsIndex++;

                // after passing the point the rotation of the moab class bloon needs to change as the direction is being changed
                transform.gameObject.GetComponent<BloonController>().RotateMoabClassBloon();
            }
        }
    }

    private void MoveToPoint(Transform nextPoint)
    {
        // smooth move from one point to another
        _currentPosition = Vector3.MoveTowards(_currentPosition, nextPoint.transform.position, _currentMovementSpeed * Time.deltaTime);
        transform.position = _currentPosition;

        // Every frame we measure how much the bloon went through
        float movement = _currentMovementSpeed * Time.deltaTime;
        _currentDistance += movement;
        _progress = _currentDistance / _level.GetPathLength();
    }

    //////////////////////////////////////
    // Geters and Setters
    //////////////////////////////////////

    public Vector3 GetMovementDirection()
    {
        var points = _level.GetPoints();
        if (_pointsIndex < points.Length)
        {
            var movementDirection = (_currentPosition - points[_pointsIndex].position).normalized;
            return movementDirection;
        }
        else
        {
            return Vector3.zero;
        }
    }

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
