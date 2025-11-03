using UnityEngine;

public class ShooterBlockHolder : MonoBehaviour
{
    private ShooterBlockManager shooterBlockManager;
    private bool shooterBlockHolderStay = true;
    public bool ShooterBlockHolderStay
    {
        get => shooterBlockHolderStay;
        set => shooterBlockHolderStay = value;
    }
    void Start()
    {
        shooterBlockHolderStay = true;
        shooterBlockManager = GetComponent<ShooterBlockManager>();
    }


    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Shooter")
        {
            Debug.Log("entered");
            shooterBlockHolderStay = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Shooter")
        {
            shooterBlockHolderStay = true;
            Debug.Log("exited");
        }
    }
}
