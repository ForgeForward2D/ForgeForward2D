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


    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

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

        float ppu = pixelPerfectCamera.assetsPPU;
        x = Mathf.Round(x * (float)ppu) / (float)ppu;
        y = Mathf.Round(y * (float)ppu) / (float)ppu;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
