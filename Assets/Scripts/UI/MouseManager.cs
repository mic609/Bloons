using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    [SerializeField] private Camera gameCamera;


    private void Awake()
    {
        // keep this object when we go to new scene
        if (Instance == null)
        {
            Instance = this;
        }
        // destroy duplicate gameobjects
        else if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    public bool IsMouseOutsideGame()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = gameCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));

        float cameraWidth = gameCamera.orthographicSize * gameCamera.aspect;
        float cameraHeight = gameCamera.orthographicSize;

        Rect cameraBounds = new Rect(
            gameCamera.transform.position.x - cameraWidth,
            gameCamera.transform.position.y - cameraHeight,
            2 * cameraWidth,
            2 * cameraHeight
        );

        return !cameraBounds.Contains(new Vector2(worldMousePosition.x, worldMousePosition.y));
    }
}
