using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int world_width;
    public int world_height;

    public int camera_width;
    public float camera_aspect;

    public float player_speed;
}
