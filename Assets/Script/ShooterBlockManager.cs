using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShooterBlockManager : MonoBehaviour
{
    [Header("Shooter Block Settings")]
    public GameObject[] shooterBlockPrefabs; 
    public Transform shooterParent;
    public float spacing = 1f;
    public float yOffset ;
    public List<GameObject> OrderOfShooterBlocks = new List<GameObject>();
    private LevelLoader levelLoader;
    private GameObject[] spawnedShooters;
    private int assignedCount,colCount=0,colorQueueCount=0,idCount=0,blockCount=0;
    private Queue<GameObject> shooterQueue=new Queue<GameObject>();
    private bool uniteBlocks=false;
    private ColorType color = ColorType.Blue;
    private List<Tuple<GameObject,Vector3,Transform>>shooterInstantiations = new List<Tuple<GameObject,Vector3,Transform>>();
    private Dictionary<int, int> shooterCounts = new(); 
    private bool outofSpace = false;
    private Vector3[] posArray;
    private List<GameObject> shooterRandomPosition= new List<GameObject>();
    [SerializeField] private GameObject outofSpaceUi;
    private List<Vector3> positions = new List<Vector3>();
    private List<int> ShooterAimingList = new List<int>();
    [SerializeField] private GameObject IdenticalShooterUi;
    private  List<GameObject> colorBlock = new List<GameObject>();
    private LevelCounter levelCounter;
    private bool islevelReady = false;
    private bool shooterblockHolderStay = false;
    [SerializeField] private List<GameObject> shooterBlockHolderList = new List<GameObject>();

    public List<GameObject> ShooterBlockHolderList
    {
        get => shooterBlockHolderList;
        set => shooterBlockHolderList = value;
    }
    public bool ShooterblockHolderStay
    {
        get => shooterblockHolderStay;
        set => shooterblockHolderStay = value;
    }
    public bool IslevelReady
    {
        get => islevelReady;
        set => islevelReady = value;
    }
    public List<Vector3> Positions
    {
        get => positions;
        set => positions = value;
    }
    public GameObject OutofSpaceUi
    {
        get => outofSpaceUi;
        set => outofSpaceUi = value;
    }
    public bool OutofSpace
    {
        get => outofSpace;
        set => outofSpace = value;
    }

    public int AssignedCount
    {
        get => assignedCount;
        set => assignedCount = value;
    }

    public Dictionary<int, int> ShooterCounts
    {
        get => shooterCounts;
        set => shooterCounts = value;
    }
    public int ColorQueueCount
    {
        get => colorQueueCount;
        set => colorQueueCount = value;
    }
    public Queue<GameObject> ShooterQueue
    {
        get => shooterQueue;
        set => shooterQueue = value;
    }

    public List<int> ShooterAimingListone
    {
        get => ShooterAimingList;
        set => ShooterAimingList = value;
    }
    void Awake()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        gridColors = levelLoader.spawnedColor.ToArray();
        SpawnShootersFromTopRow(gridColors);
        ShooterPositionRandomize();
        levelCounter= FindObjectOfType<LevelCounter>();
        IdenticalShooterUi.SetActive(false);
    }
    private void Update()
    {
        foreach (var VARIABLE in shooterQueue)
        {
            if (color == VARIABLE.gameObject.GetComponent<ShooterBlock>().colorType)
            {
                colorQueueCount++;
            }
            else
            {
                colorQueueCount = 0;
                color = VARIABLE.gameObject.GetComponent<ShooterBlock>().colorType;
            }

            if (levelLoader.TotalNumberOfBlocks <= 0)
            {
                VARIABLE.gameObject.GetComponent<ShooterBlock>().UpdateCountDisplay(0);
            }
        }

        blockCount = levelLoader.TotalNumberOfBlocks;
        ShooterUnite();
        GettingMultipleColor();
        MaintainShooterBlockHolder();
        OutOfSpace();
    }

    public int[] gridColors;


    public void SpawnShootersFromTopRow(int[] topRowColors)
    {
        if (topRowColors == null || topRowColors.Length == 0) return;

        ClearShooters();
        shooterInstantiations.Clear();
        int count = topRowColors.Length;
        spawnedShooters = new GameObject[count];

        float totalWidth = count + spacing * (count - 1);
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            
            yOffset -= 3.5f;
            int colorIndex = topRowColors[i];
            GameObject prefab = shooterBlockPrefabs[colorIndex];
            ColorType color = (ColorType)topRowColors[i];
            int rand = Random.Range(2, 4);
            int randNum = rand;
            int colorBlockCount = levelLoader.colorCount.ContainsKey(color) ? levelLoader.colorCount[color] : 0;
            int remainingBlocks = colorBlockCount;
            int baseCountPerShooter = colorBlockCount / rand;
            int remainder = colorBlockCount % rand; 
            
            for (int n = 0; n < rand; n++)
            {
                float xPos = startX + (i * (1f+spacing )) ;
                if (colCount > 3)
                {
                    yOffset += 3f;
                }
                Vector3 spawnPos = new Vector3(xPos, -(10f+n), 0f);
                shooterInstantiations.Add(new Tuple<GameObject, Vector3, Transform>(prefab, spawnPos, shooterParent));
              idCount++;
              GameObject shooterGO = Instantiate(prefab, spawnPos, Quaternion.identity, shooterParent);
              ShooterBlock shooterBlock = shooterGO.GetComponent<ShooterBlock>();
              colCount++;
              shooterRandomPosition.Add(shooterGO);
              if (shooterBlock != null)
              {
                  shooterBlock.UniQueIDSetter(idCount);
                  shooterBlock.colorType = color;
                  int assignedCount = baseCountPerShooter + (n < remainder ? remainder : 0);
                  shooterCounts[idCount] = assignedCount;
                  shooterBlock.UpdateCountDisplay(assignedCount);
                  remainingBlocks -= assignedCount;
              }

            }
            colCount++;
        }
        
    }
    public void ResetShooterTargets()
    {
        Positions.Clear();
    }

    void GettingMultipleColor()
    {
        bool anyShooterHasTargets = false;

        foreach (var VARIABLE in shooterQueue)
        {
            ShooterBlock shooterBlock = VARIABLE.GetComponent<ShooterBlock>();
            if (shooterBlock == null) continue;

            ColorType color = shooterBlock.colorType;
            var list = levelLoader.GetLastRowColors(color);

            if (list != null && list.Count > 0)
            {
                anyShooterHasTargets = true;
                break;
            }
        }

        outofSpace = !anyShooterHasTargets;
    }


    void ShooterPositionRandomize()
    {
        if (shooterRandomPosition == null || shooterRandomPosition.Count == 0)
        {
            return;
        }
        List<Vector3> posList = shooterRandomPosition
            .Select(s => s.transform.position)
            .ToList();
        for (int i = 0; i < posList.Count; i++)
        {
            int randomIndex = Random.Range(0, posList.Count);
            (posList[i], posList[randomIndex]) = (posList[randomIndex], posList[i]); // swap
        }

        for (int i = 0; i < shooterRandomPosition.Count; i++)
        {
            shooterRandomPosition[i].transform.position = posList[i];
        }
    }

    void ShooterUnite()
    {
        if (shooterQueue.Count < 3) return;
        var shooters = shooterQueue.ToList();
        var groupedByColor = shooters
            .GroupBy(s => s.GetComponent<ShooterBlock>().colorType)
            .Where(g => g.Count() >= 3)
            .ToList();

        foreach (var group in groupedByColor)
        {
            var mainShooter = group.First().GetComponent<ShooterBlock>();
            int totalCount = 0;

            foreach (var shooterObj in group)
            {
                var shooter = shooterObj.GetComponent<ShooterBlock>();
                totalCount += shooter.ShootingProjectileNumber;
                if (shooter != mainShooter)
                {
                    shooterQueue = new Queue<GameObject>(shooterQueue.Where(x => x != shooterObj));
                    OrderOfShooterBlocks.Remove(shooterObj);
                    Destroy(shooterObj);
                }
            }
            IdenticalShooterUi.SetActive(true);
            mainShooter.UpdateCountDisplay(totalCount);
            Invoke("IdenticalShooterUiSetActive",3f);
        }
    }
    void MaintainShooterBlockHolder()
    {
        foreach (var VARIABLE in shooterBlockHolderList)
        {
            bool empty = VARIABLE.GetComponent<ShooterBlockHolder>().ShooterBlockHolderStay;
            if (empty)
            {
                if (shooterQueue.Count > 0)
                {
                    GameObject shooter = shooterQueue.Peek();
                    ShooterBlock block = shooter.GetComponent<ShooterBlock>();
                    block.NowClickOn = true;
                }

            }
        }
        
    }
    void IdenticalShooterUiSetActive()
    {
        IdenticalShooterUi.SetActive(false);
    }
    void OutOfSpace()
    {
        {
           if (levelLoader.RealoutOfSpace && levelLoader.TotalNumberOfBlocks>0)
           {
               outofSpaceUi.SetActive(outofSpace);
           }
        }
    }

    public void OnCrossClick()
    {
        levelLoader.RealoutOfSpace = false;
        outofSpaceUi.SetActive(false);
    }
    IEnumerator LoadLevel()
    {
        islevelReady = false;
        yield return new WaitForSeconds(0.5f);
        islevelReady = true;
    }
    void ClearShooters()
    {
        if (shooterParent == null) return;
        foreach (Transform child in shooterParent)
            Destroy(child.gameObject);
    }

}
