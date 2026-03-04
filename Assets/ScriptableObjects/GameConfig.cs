using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    [System.NonSerialized] public int world_width;
    [System.NonSerialized] public int world_height;
    [System.NonSerialized] public int yMin;
    [System.NonSerialized] public int yMax;

    [System.NonSerialized] public int xMin;
    [System.NonSerialized] public int xMax;

    public int camera_width;
    public float camera_aspect;

    public float player_speed;
}
