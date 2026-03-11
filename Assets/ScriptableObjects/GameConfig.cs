using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int camera_width;
    public float camera_aspect;

    public float player_speed;
    public float player_breaking_speed;
    public float block_breaking_animation_min_update_interval;
}
