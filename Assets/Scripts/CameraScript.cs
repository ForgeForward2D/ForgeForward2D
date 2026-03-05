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

    private int cameraPixelHeight;
    private int cameraPixelWidth;
    private int ppu;

    private (int xMin, int xMax, int yMin, int yMax) mapBounds;

    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

        ppu = pixelPerfectCamera.assetsPPU;
        cameraPixelWidth = gameConfig.camera_width * ppu;
        cameraPixelHeight = Mathf.RoundToInt(cameraPixelWidth / gameConfig.camera_aspect);

        camera_height = Mathf.RoundToInt((float)cameraPixelHeight / ppu);

        GetComponent<Camera>().orthographicSize = cameraPixelHeight / (2f * ppu);
        GetComponent<Camera>().aspect = (float)cameraPixelWidth / cameraPixelHeight;

        mapBounds = tileMapManager.GetBounds();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        float halfHeight = cameraPixelHeight / (2f * ppu);
        float halfWidth = cameraPixelWidth / (2f * ppu);

        float x = playerTransform.position.x;
        x = Mathf.Min(x, mapBounds.xMax - halfWidth);
        x = Mathf.Max(x, mapBounds.xMin + halfWidth);

        float y = playerTransform.position.y;
        y = Mathf.Min(y, mapBounds.yMax - halfHeight);
        y = Mathf.Max(y, mapBounds.yMin + halfHeight);

        x = Mathf.Round(x * ppu) / ppu;
        y = Mathf.Round(y * ppu) / ppu;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
