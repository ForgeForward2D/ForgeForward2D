using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;
    [SerializeField] Transform playerTransform;
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;

    [Header("Debugging")]
    [SerializeField] private int camera_height;
    // [SerializeField] private (int xMin, int xMax, int yMin, int yMax) mapBounds;

    // TileMapManager tileMapManager;

    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

        camera_height = Mathf.RoundToInt(gameConfig.camera_width / gameConfig.camera_aspect);
        GetComponent<Camera>().orthographicSize = camera_height / 2f;
        GetComponent<Camera>().aspect = gameConfig.camera_aspect;

        // tileMapManager = TileMapManager.Instance;

        // mapBounds = tileMapManager.GetBounds();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // mapBounds = TileMapManager.Instance.GetBounds();
        float halfHeight = camera_height / 2f;
        float halfWidth = camera_height * gameConfig.camera_aspect / 2f;

        float x = playerTransform.position.x;
        // x = Mathf.Min(x, mapBounds.xMax - halfWidth);
        // x = Mathf.Max(x, mapBounds.xMin + halfWidth);

        float y = playerTransform.position.y;
        // y = Mathf.Min(y, mapBounds.yMax - halfHeight);
        // y = Mathf.Max(y, mapBounds.yMin + halfHeight);

        float ppu = pixelPerfectCamera.assetsPPU;
        x = Mathf.Round(x * (float)ppu) / (float)ppu;
        y = Mathf.Round(y * (float)ppu) / (float)ppu;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
