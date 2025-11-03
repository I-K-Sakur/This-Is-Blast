using SmallHedge.SoundManager;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShooterProjectile : MonoBehaviour
{
    public ColorType colorType;
    public float speed = 8f;
    private GameObject targetBlock;
    private bool hasTarget = false;
    private LevelLoader levelLoader;
    private Vector3 targetPos;
    private ShooterBlock shooterBlock;
    private float lifetime = 5f;
    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        shooterBlock = FindObjectOfType<ShooterBlock>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.03f)
            {
                HitTarget();
            }
        }
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    public void SetTarget(Vector3 pos)
    {
        targetPos = pos;
        hasTarget = true;
    }

    private void HitTarget()
    {
        if (targetBlock != null)
        {
           
            Tile tile = targetBlock.GetComponent<Tile>();
            if (tile != null && tile.colorType == colorType)
            {
                shooterBlock.Shoot = false;
                levelLoader.RemoveBlock(tile.row, tile.col);
                Destroy(targetBlock);
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Tile tile = other.GetComponent<Tile>();
        if (tile != null)
        {
            if (tile.colorType == colorType)
            {
                SoundManager.PlaySound(SoundType.TARGETHIT);
                if (levelLoader != null)
                {
                    levelLoader.RemoveBlock(tile.row, tile.col);
                }
                Destroy(tile.gameObject);
                if(shooterBlock != null)
                    shooterBlock.Shoot = false;
                Destroy(gameObject);
            }
        }
    }
}