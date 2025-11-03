using System;
using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [Header("Block Settings")]
    public Transform blockParent;
    public GameObject greenBlockPrefab;
    public GameObject yellowBlockPrefab;
    public GameObject redBlockPrefab;
    public GameObject blueBlockPrefab;
   private  float totalWidth,totalHeight;
    [Header("Level Data")]
    public LevelData currentLevel;
    public Dictionary<ColorType,int> colorCount = new Dictionary<ColorType,int>();
    [Header("Spacing Settings")]
    private float blockSpacing = .1f;
    private float blockWidth = 0.5f;
    private float blockHeight = 0.5f;
    public HashSet<int> spawnedColor = new HashSet<int>();
    public GameObject[,] grid; 
    private LevelData currentLevelData;
    private Vector2 startPos; 
    private GameManager gameManager;
    public float verticalOffset = 2f; 
    private ShooterBlock shooterBlock;
    private ShooterBlockManager shooterBlockManager;
    private int totalNumberOfBlocks;
    [SerializeField]private LevelCounter levelCounter;
    private ColorType currentColorType;
    public List<GameObject> AllLastRowBlocks = new List<GameObject>();
    public Dictionary<Vector3, bool> claimedTargets = new Dictionary<Vector3, bool>();
    private bool realoutOfSpace = false;
    [SerializeField] private List<GameObject> higherBlocks = new List<GameObject>();
    
    public bool RealoutOfSpace
    {
        get => realoutOfSpace;
        set => realoutOfSpace = value;
    }
    public ColorType CurrentColorType
    {
        get => currentColorType;
        set => currentColorType = value;
    }
    public int TotalNumberOfBlocks
    {
        get => totalNumberOfBlocks;
        set => totalNumberOfBlocks = value;
    }
    void Awake()
    {
   
        gameManager = FindObjectOfType<GameManager>();
        if (currentLevel != null)
            LoadLevel(currentLevel,levelCounter.level);
        shooterBlock = FindObjectOfType<ShooterBlock>();
        shooterBlockManager = FindObjectOfType<ShooterBlockManager>();
       
    }
    

public void LoadLevel(LevelData levelData,int ind)
{
    if (levelData == null) return;
    ClearLevel();
    blockWidth = 1f;
    blockHeight = 1f;
    Renderer r = greenBlockPrefab.GetComponent<Renderer>();
    if (r != null)
    {
        blockWidth = r.bounds.size.x;
        blockHeight = r.bounds.size.y;
    }
    else
    {
        SpriteRenderer sr = greenBlockPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            blockWidth = sr.bounds.size.x;
            blockHeight = sr.bounds.size.y;
        }
    }

    foreach (ColorType color in Enum.GetValues(typeof(ColorType)))
    {
        colorCount[color] = 0;
    }
    float totalWidth = levelData.columns * (blockWidth + blockSpacing);
    float totalHeight = levelData.rows * (blockHeight + blockSpacing);
    float yOffset = 6f;
    startPos = new Vector2(-totalWidth / 2 + blockWidth / 2, totalHeight + yOffset);
    grid = new GameObject[levelData.rows, levelData.columns];
    for (int row = 0; row < levelData.rows; row++)
    {
        for (int col = 0; col < levelData.columns; col++)
        {
            int index = UnityEngine.Random.Range(0, Mathf.Min(ind, System.Enum.GetValues(typeof(ColorType)).Length));
            //Debug.Log(index);
            ColorType color = (ColorType)index;
            spawnedColor.Add(index);
            GameObject prefabToSpawn = (levelCounter.level<=10)? GetBlockPrefab(color):higherBlocks[index];
            colorCount[color]++;
            //Debug.Log( colorCount[color]);
            if (prefabToSpawn == null) continue;
            
            float x = startPos.x + col * (blockWidth + blockSpacing);
            float y = startPos.y - row * (blockHeight + blockSpacing);
            Vector2 spawnPos = new Vector2(x, y);

            GameObject block = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, blockParent);
            block.GetComponent<Tile>().Identity = totalNumberOfBlocks ;
            grid[row, col] = block;
            Tile blockBehavior = block.GetComponent<Tile>();
            if (blockBehavior == null) blockBehavior = block.AddComponent<Tile>();
            blockBehavior.Initialize(this, row, col, color);
           totalNumberOfBlocks++;
           Debug.Log(totalNumberOfBlocks);
        }
    }

    currentLevel = levelData;
}


public List<GameObject> GetLastRowColors(ColorType targetColor)
{
    List<GameObject> matchingBlocks = new List<GameObject>();
    matchingBlocks.Clear();
    if (currentLevel == null) return null;

    int lastRow = currentLevel.rows - 1;
    AllLastRowBlocks.Clear();

    for (int col = 0; col < currentLevel.columns; col++)
    {
        GameObject block = grid[lastRow, col];
        if (block == null) continue;
        
        AllLastRowBlocks.Add(block);

        Tile tile = block.GetComponent<Tile>();
        if (tile != null && tile.colorType == targetColor)
            matchingBlocks.Add(block);
    }
    
    if (AllLastRowBlocks.Count == 0)
    {
        if (shooterBlockManager != null) shooterBlockManager.OutofSpace = false;
        return null;
    }
    bool hasMatching = matchingBlocks.Count > 0;
    if (shooterBlockManager != null) shooterBlockManager.OutofSpace = !hasMatching;

    return matchingBlocks;
}



public List<Tuple<ColorType, Vector3, GameObject>> GetPositionsOfColorBlocks(ColorType targetColor, GameObject shooterInstance)
{
    List<Tuple<ColorType, Vector3, GameObject>> positions = new List<Tuple<ColorType, Vector3, GameObject>>();
     positions.Clear();
    List<GameObject> blocks = GetLastRowColors(targetColor);

    if (blocks == null || blocks.Count == 0)
        return positions;

    foreach (GameObject block in blocks)
    {
        if (block == null) continue;

        Tile tile = block.GetComponent<Tile>();
        if (tile == null) continue;
        
        ColorType blockColor = tile.colorType;
        positions.Add(new Tuple<ColorType, Vector3, GameObject>(blockColor, block.transform.position, block));
    }

    return positions;
}


public void RemoveBlock(int row, int col)
{
    if (grid == null) return;
    if (row < 0 || col < 0 || row >= currentLevel.rows || col >= currentLevel.columns) return;

    GameObject block = grid[row, col];
    if (block != null)
    {
        AllLastRowBlocks.Remove(block);

        Tile t = block.GetComponent<Tile>();
        if (t != null && shooterBlockManager != null)
        {
            shooterBlockManager.ShooterAimingListone.Remove(t.Identity);
        }

        realoutOfSpace = true;
        totalNumberOfBlocks--;
        gameManager.Slider.value++;
        Destroy(block);
    }


    grid[row, col] = null;
    DropBlocksDown(row, col);
    GetLastRowColors(currentColorType);
    
}

public void DropBlocksDown(int destroyedRow, int column)
{
    if (grid == null || currentLevel == null) return;
    for (int row = destroyedRow - 1; row >= 0; row--)
    {
        GameObject above = grid[row, column];
        if (above == null) continue;
        int targetRow = row;
        while (targetRow + 1 < currentLevel.rows && grid[targetRow + 1, column] == null)
            targetRow++;
        
        if (targetRow != row)
        {
            grid[targetRow, column] = above;
            grid[row, column] = null;
            float newY = startPos.y - targetRow * (blockHeight + blockSpacing);
            Vector3 newPos = new Vector3(above.transform.position.x, newY, above.transform.position.z);
            above.transform.position = newPos;
            Tile t = above.GetComponent<Tile>();
            if (t != null) t.UpdateRow(targetRow, column);
        }
    }
}

    
    void ClearLevel()
    {
        foreach (Transform child in blockParent)
            Destroy(child.gameObject);
    }

    GameObject GetBlockPrefab(ColorType color)
    {
        switch (color)
        {
            case ColorType.Green: return greenBlockPrefab;
            case ColorType.Yellow: return yellowBlockPrefab;
            case ColorType.Red: return redBlockPrefab;
            case ColorType.Blue: return blueBlockPrefab;  
            default: return null;
        }
    }
}
