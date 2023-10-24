using UnityEngine;

public class Vectors : MonoBehaviour
{
    [SerializeField]
    private Vector3 positionVector;

    [SerializeField]
    private Vector3 directionVector;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + positionVector);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, directionVector);
    }
}