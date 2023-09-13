using UnityEngine;

public class TowerRotation : MonoBehaviour
{
    public void Rotate(Transform enemy)
    {
        var dir = enemy.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90; // The sprite is facing down that's way there is +90
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
