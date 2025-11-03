using UnityEngine;

[CreateAssetMenu(fileName = "LevelCounter", menuName = "Scriptable Objects/LevelCounter")]
public class LevelCounter : ScriptableObject
{
    public int level = 1;
    public int score = 10;
}

