using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int rows;
    public int columns;
    public ColorType[] layout;
}

public enum ColorType
{
    Red,
    Green,
    Yellow,
    Blue,
}