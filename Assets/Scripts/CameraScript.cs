using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;
    [SerializeField] Transform playerTransform;

    [Header("Settings")]
    [SerializeField] private bool doPixelPerfect = true;
    [Min(1)]
    [SerializeField] private int assetPixelPerUnit = 128;

    [Header("Debugging")]
    [SerializeField] private int camera_height;

    void Start()
    {
        camera_height = Mathf.RoundToInt(gameConfig.camera_width / gameConfig.camera_aspect);
        GetComponent<Camera>().orthographicSize = camera_height / 2f;
        GetComponent<Camera>().aspect = gameConfig.camera_aspect;

    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        float x = playerTransform.position.x;

        float y = playerTransform.position.y;

        if (doPixelPerfect)
        {
            x = Mathf.Round(x * (float)assetPixelPerUnit) / (float)assetPixelPerUnit;
            y = Mathf.Round(y * (float)assetPixelPerUnit) / (float)assetPixelPerUnit;
        }
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
