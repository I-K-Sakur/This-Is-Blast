using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections;
using SmallHedge.SoundManager;
using Random = System.Random;
[RequireComponent(typeof(Collider2D))]
public class ShooterBlock : MonoBehaviour
{
    public ColorType colorType;
    public GameObject projectilePrefab;
    public float shootCooldown = 1f;
    private List<Tuple<ColorType, Vector3,GameObject>> targetPositions;
    private ShooterBlockManager shooterBlockManager;
    private float lastShotTime=1f;
    private LevelLoader loader;
    public TextMeshPro countText;
    private bool shoot,justOnce,onlyThisShooter;
    [SerializeField] private GameObject shooterBlockPrefab;
    private GameManager gameManager;
    private Vector3 newPosition;
    private int shootingProjectileNumber;
    [SerializeField]private LevelCounter levelCounter;
    private int uniQueID;
    private bool extraBlock=false;
    [SerializeField] private AudioSource shootingAudioSource;
     private SpriteRenderer spriteRenderer;
     public Sprite selectedSprite;
     public Sprite normalSprite;
     private int temptime = 1;
     private bool nowClickOn = false;

 public bool NowClickOn
 {
     get => nowClickOn;
     set => nowClickOn = value;
 }

 public int UniQueID
 {
     get => uniQueID;
     set => uniQueID = value;
 }
 public int ShootingProjectileNumber
    {
        get => shootingProjectileNumber;
        set => shootingProjectileNumber = value;
    }
    public bool Shoot
    {
        get => shoot;
        set => shoot = value;
    }
    public Vector3 NewPosition
    {
        get => newPosition;
        set => newPosition = value;
    }


    private void Start()
    {
        gameManager=FindObjectOfType<GameManager>();
        loader = FindObjectOfType<LevelLoader>();
        shooterBlockManager = FindObjectOfType<ShooterBlockManager>();
        if (countText == null)
            countText = GetComponentInChildren<TextMeshPro>();
        if (loader != null && loader.colorCount.ContainsKey(colorType))
        {
            foreach (var VARIABLE in shooterBlockManager.ShooterCounts)
            {
                if (VARIABLE.Key == uniQueID)
                {
                    UpdateCountDisplay(VARIABLE.Value);
                }
            }
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (levelCounter.level >= 8)
        {
            spriteRenderer.sprite = normalSprite;
        }
        int temptime = 1;
        
    }

private void Update()
{
    CheckUpdatedLastRow();
    int destroyNum = shootingProjectileNumber;
    if (destroyNum <= 0)
    {
        shooterBlockManager.OutofSpace = true;
        BlockDestroyAnim();
    }
    if (shoot )
    {
        FireProjectileAtColorBlocks();
    }

    if (shootingProjectileNumber <= 0)
    {
        if (shooterBlockManager.ShooterQueue.Contains(gameObject))
            shooterBlockManager.ShooterQueue = new Queue<GameObject>(
                shooterBlockManager.ShooterQueue.Where(x => x != gameObject)
            );
        gameObject.SetActive(false);
    }
}

    public void UniQueIDSetter(int id)
    {
        uniQueID = id;
    }
    void BlockDestroyAnim()
    {
        float speed = 1f;
        gameObject.transform.Translate(transform.position.x - 10f*Time.time*speed, transform.position.y, transform.position.z);
        Destroy(gameObject);
    }
    void BlockChanger()
    {
        if (spriteRenderer != null && selectedSprite != null )
        {
            spriteRenderer.sprite = selectedSprite;
        }
    }
    void OnMouseDown()
    {
        if ( (nowClickOn) || shooterBlockManager.ShooterQueue.Count <4 || shooterBlockManager.OutofSpace )
        {
            SoundManager.PlaySound(SoundType.CLICKSOUND);
            nowClickOn = false;
            CLickingOnBlock();
        }
    }
    void CLickingOnBlock()
    {
        if (shooterBlockManager.OutofSpace)
        {
            shooterBlockManager.OutofSpaceUi.SetActive(false);
            Debug.Log("Clicked Successfully");
            var queueList = shooterBlockManager.ShooterQueue.ToList();
            Vector3 pos;
            if (queueList.Count >= 4)
            {
                pos = new Vector3(queueList[3].transform.position.x+2 , transform.position.y, transform.position.z);
                transform.position = pos;
            }
            else
            {
                UpdatedPositionAndShoot(); 
            }
            shooterBlockManager.OutofSpace = false;
        }
        else
        {
            UpdatedPositionAndShoot();
        }
        ShooterBlock[] allShooters = FindObjectsOfType<ShooterBlock>();
        foreach (ShooterBlock s in allShooters)
        {
            if (s != this)
                s.shoot = false;
        }
        
        shooterBlockManager.OrderOfShooterBlocks.Add(gameObject);
        onlyThisShooter = true;

        if (!shooterBlockManager.ShooterQueue.Contains(gameObject))
        {
            shooterBlockManager.ShooterQueue.Enqueue(gameObject);
        }

        if (levelCounter.level >= 8)
        {
            BlockChanger();
        }
        int listpos = shooterBlockManager.ShooterQueue.Count;
        shootCooldown = Mathf.Max(0.1f, listpos); 
        uniQueID = listpos;
        lastShotTime = Time.time;
        FireProjectileAtColorBlocks();
    }
    
    void UpdatedPositionAndShoot()
    {
        if (!justOnce && levelCounter.level <= 3)
        {
            var queueList = shooterBlockManager.ShooterQueue.ToList();
            Vector3 spawnPos = new Vector3(transform.position.x, -6f, transform.position.z);
            bool realPos = queueList.All(b=>b.gameObject.transform.position == spawnPos);
            Vector3 newPos = transform.position;
            if (realPos)
            {
                 newPos = new Vector3(transform.position.x, -5f, transform.position.z);
                transform.position = newPos;
            }
        
            else
            {
                transform.position = spawnPos;
            }
        
            if (shooterBlockPrefab != null)
                shooterBlockPrefab.transform.position = spawnPos;
            justOnce = true;
        }
        if(!justOnce && levelCounter.level >= 4)
        {
            foreach (var VARIABLE in shooterBlockManager.ShooterBlockHolderList)
            {
                ShooterBlockHolder holder = VARIABLE.GetComponent<ShooterBlockHolder>();

                if (holder.ShooterBlockHolderStay)
                {
                    holder.ShooterBlockHolderStay = false;
                    Vector3 holderPos = holder.transform.position;
                    transform.position = new Vector3(holderPos.x, holderPos.y, -1f); 

                    justOnce = true;
                    break; 
                }
            }
        }
    }


    public void UpdateCountDisplay(int countNumber)
    {   if (loader == null || countText == null) return;
            shootingProjectileNumber = countNumber;
            countText.text = shootingProjectileNumber.ToString();
    }

    void CheckUpdatedLastRow()
    {
        if (onlyThisShooter)
        {
            if (this == null || gameObject == null || loader == null) return;
            targetPositions = loader.GetPositionsOfColorBlocks(colorType,this.gameObject);
            shoot = true;
        }
    
    }
    public void FireProjectileAtColorBlocks()
    {
        if (projectilePrefab == null || loader == null)
        {
            shoot = false; 
            return;
        }
        if (targetPositions == null || targetPositions.Count == 0)
        {
            targetPositions = loader.GetPositionsOfColorBlocks(colorType, this.gameObject);
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            shoot = false;
            return;
        }

        foreach (var target in targetPositions.ToList()) 
        {
            int iden = -1;
            var blockObj = target.Item3;
            if (blockObj != null)
            {
                Tile t = blockObj.GetComponent<Tile>();
                if (t != null) iden = t.Identity;
            }

            if (iden != -1 && shooterBlockManager.ShooterAimingListone.Contains(iden))
            {
                continue;
            }
            if (shootingProjectileNumber <= 0)
                break;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            ShooterProjectile shooterProjectile = projectile.GetComponent<ShooterProjectile>();
            shootingProjectileNumber--;
            UpdateCountDisplay(shootingProjectileNumber);
            if (shooterProjectile != null && target.Item1 == colorType)
            {
                shooterProjectile.colorType = colorType;
                shooterProjectile.SetTarget(target.Item2);
            }
            
            if (iden != -1)
                shooterBlockManager.ShooterAimingListone.Add(iden);
            targetPositions.Remove(target);
        }
        shoot = false;
    }
    
private IEnumerator RemoveTarget(float delay,Tuple<ColorType, Vector3,GameObject> target)
{
    yield return new WaitForSeconds(delay);
    targetPositions.Remove(target);
}
}
