using UnityEngine;

public class Tile : MonoBehaviour
{
    private LevelLoader levelLoader;
    public int row, col;
    public ColorType colorType;
    [SerializeField]private int identity;

    public int Identity
    {
        get => identity;
        set => identity = value;
    }
    public void Initialize(LevelLoader loader, int r, int c, ColorType color)
    {
        levelLoader = loader;
        row = r;
        col = c;
        colorType = color;
    }

    public void UpdateRow(int newRow, int sameColumn)
    {
        row = newRow;
        col = sameColumn;
    }

    public void UpdateColumn(int newCol)
    {
        col = newCol;
    }

    public void DestroyBlock()
    {
        if (levelLoader != null)
            levelLoader.RemoveBlock(row, col);
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        DestroyBlock();
    }
}