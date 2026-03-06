using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int camera_width;
    public float camera_aspect;

    public float player_speed;
    public float player_breaking_speed;
    public float min_destruction_animation_interval;
    public float player_breaking_animation_interval;
}
