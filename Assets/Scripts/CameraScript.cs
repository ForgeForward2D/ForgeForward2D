using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;

    [SerializeField] Transform playerTransform;

    [SerializeField] UnityEngine.U2D.PixelPerfectCamera pixelPerfectCamera;

    private PixelPerfectCamera pixelPerfectCam;

    public int camera_height;

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
        // float halfHeight = GetComponent<Camera>().orthographicSize;
        // float halfWidth = halfHeight * GetComponent<Camera>().aspect;
        float halfHeight = camera_height / 2f;
        float halfWidth = (camera_height * gameConfig.camera_aspect) / 2f;

        float x = playerTransform.position.x;
        x = Mathf.Min(x, gameConfig.xMax - halfWidth);
        x = Mathf.Max(x, gameConfig.xMin + halfWidth);

        float y = playerTransform.position.y;
        y = Mathf.Min(y, gameConfig.yMax - halfHeight);
        y = Mathf.Max(y, gameConfig.yMin + halfHeight);

        // x = Mathf.Round(x * (float)ppu) / (float)ppu;
        // y = Mathf.Round(y * (float)ppu) / (float)ppu;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
