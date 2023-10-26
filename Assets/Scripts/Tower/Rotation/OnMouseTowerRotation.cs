using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OnMouseTowerRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var direction = mousePosition - transform.position;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90.0f;

        var currentAngle = transform.eulerAngles.z;
        var angleDifference = Mathf.DeltaAngle(currentAngle, angle);
        var rotationAmount = Mathf.Clamp(angleDifference, -_rotationSpeed * Time.deltaTime, _rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle + rotationAmount));
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        _rotationSpeed = rotationSpeed;
    }
}
