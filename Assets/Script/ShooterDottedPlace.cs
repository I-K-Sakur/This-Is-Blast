using System;
using UnityEngine;

public class ShooterDottedPlace : MonoBehaviour
{
    public bool staying;
    private Vector3 position;

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
    }
    

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Shooter"))
        {
            staying = true;
        }
    }
}