using System;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    public float leftBoundary = -3f;
    public float rightBoundary = 3f;
    public float moveSpeed = 3f;
    
    private CrowdManager crowdManager;
    private float direction = 1f;

    private void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
    }

    void Update()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;
        
        if (transform.position.x >= rightBoundary)
        {
            direction = -1f;
            transform.position = new Vector3(rightBoundary, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= leftBoundary)
        {
            direction = 1f;
            transform.position = new Vector3(leftBoundary, transform.position.y, transform.position.z);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool isMainPlayer = false;
            if (crowdManager != null)
            {
                isMainPlayer = (other.gameObject.TryGetComponent(out PlayerController _));
            }
            
            if (isMainPlayer)
            {
                Debug.Log("Main Player hit by Moving Trap - Game Over!");
                UIController.Instance.HandleLoose();
            }
            else if (crowdManager != null)
            {
                crowdManager.RemoveSpecificCrowdMember(other.gameObject);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Vector3 leftPos = new Vector3(leftBoundary, transform.position.y, transform.position.z);
        Vector3 rightPos = new Vector3(rightBoundary, transform.position.y, transform.position.z);
        
        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawWireSphere(leftPos, 0.2f);
        Gizmos.DrawWireSphere(rightPos, 0.2f);
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}