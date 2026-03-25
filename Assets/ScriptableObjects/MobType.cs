using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/MobType")]
public class MobType : ScriptableObject
{
    public string displayName;
    public GameObject prefab;

    public float moveSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float aggroRange = 5.0f;
    public bool stealsResources = false;
    public float directionCheckDistance = 0.45f;
    public Vector2 idleDurationRange = new Vector2(0.75f, 2f);
    public Vector2 walkDurationRange = new Vector2(1.5f, 4f);
}