using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Header("Debugging")]
    [SerializeField] private Light2D light2D;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        var worldGen = FindAnyObjectByType<WorldGeneration>();
        if (worldGen == null || worldGen.BaseLevel == null)
        {
            Debug.LogError("No worldGeneration script or base level found in LightScript");
            return;
        }
        light2D.pointLightOuterRadius = worldGen.BaseLevel.viewDistance;
        light2D.lightType = Light2D.LightType.Point;
    }

    private void OnEnable()
    {
        PortalManager.OnPlayerTeleport += HandleTeleport;
    }

    private void OnDisable()
    {
        PortalManager.OnPlayerTeleport -= HandleTeleport;
    }

    private void HandleTeleport((Level level, Vector3 destination) data)
    {
        light2D.pointLightOuterRadius = data.level.viewDistance;
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                transform.position.z
            );
        }
    }
}
