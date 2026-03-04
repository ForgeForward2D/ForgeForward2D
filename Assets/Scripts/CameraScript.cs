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
        // Automatically get the required components
        pixelPerfectCam = GetComponent<PixelPerfectCamera>();
        camera_height = Mathf.RoundToInt(gameConfig.camera_width / gameConfig.camera_aspect); 
        
        if (pixelPerfectCam == null)
            Debug.LogError("PixelPerfectCamera component missing!");

        GetComponent<Camera>().orthographicSize = camera_height / 2f;
        GetComponent<Camera>().aspect = gameConfig.camera_aspect;

        if (pixelPerfectCamera == null)
        {
            pixelPerfectCamera = GetComponent<UnityEngine.U2D.PixelPerfectCamera>();
            Debug.Log("Resolving Pixel Perfect Camera component from the same GameObject.");
        }

        if (pixelPerfectCamera == null) {
            Debug.Log("Pixel Perfect Camera is not assigned!");
        } else {
            Debug.Log("Pixel Perfect Camera is assigned.");
            int ppu = 16; 
            pixelPerfectCamera.assetsPPU = ppu;
            pixelPerfectCamera.refResolutionX = gameConfig.camera_width * ppu;
            pixelPerfectCamera.refResolutionY =camera_height * ppu;
        }
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
        x = Mathf.Min(x, gameConfig.world_width - halfWidth);
        x = Mathf.Max(x, halfWidth);

        float y = playerTransform.position.y;
        y = Mathf.Min(y, gameConfig.world_height - halfHeight);
        y = Mathf.Max(y, halfHeight); 

        x = Mathf.Round(x * 16f) / 16f;
        y = Mathf.Round(y * 16f) / 16f;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
