using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;

    [SerializeField] Transform playerTransform;

    [SerializeField] PixelPerfectCamera pixelPerfectCamera;

    [SerializeField] TileMapManager tileMapManager;


    public int camera_height;

    private (int xMin, int xMax, int yMin, int yMax) mapBounds;

    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

        camera_height = Mathf.RoundToInt(gameConfig.camera_width / gameConfig.camera_aspect);
        GetComponent<Camera>().orthographicSize = camera_height / 2f;
        GetComponent<Camera>().aspect = gameConfig.camera_aspect;

        mapBounds = tileMapManager.GetBounds();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        mapBounds = tileMapManager.GetBounds();
        float halfHeight = camera_height / 2f;
        float halfWidth = camera_height * gameConfig.camera_aspect / 2f;

        float x = playerTransform.position.x;
        x = Mathf.Min(x, mapBounds.xMax - 1 - halfWidth);
        x = Mathf.Max(x, mapBounds.xMin + 1 + halfWidth);

        float y = playerTransform.position.y;
        y = Mathf.Min(y, mapBounds.yMax - 1 - halfHeight);
        y = Mathf.Max(y, mapBounds.yMin + 1 + halfHeight);

        float ppu = pixelPerfectCamera.assetsPPU;
        x = Mathf.Round(x * (float)ppu) / (float)ppu;
        y = Mathf.Round(y * (float)ppu) / (float)ppu;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
