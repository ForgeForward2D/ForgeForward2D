using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightScript : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    void Start()
    {
        var worldGen = FindAnyObjectByType<WorldGeneration>();
        if (worldGen == null || worldGen.BaseLevel == null)
        {
            Debug.LogError("No worldGeneration script or base level found in LightScript");
            return;
        }
        light2D.pointLightOuterRadius = worldGen.BaseLevel.viewDistance;
    }

    void OnEnable()
    {
        PortalManager.OnPlayerTeleport += HandleTeleport;
    }

    void OnDisable()
    {
        PortalManager.OnPlayerTeleport -= HandleTeleport;
    }

    void HandleTeleport((Level level, Vector3 destination) data)
    {
        light2D.pointLightOuterRadius = data.level.viewDistance;
    }

    void LateUpdate()
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
