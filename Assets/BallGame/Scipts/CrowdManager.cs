using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("Crowd Settings")]
    public int maxCrowdSize = 20;
    public float crowdRadius = 3f;
    public float minDistanceBetweenMembers = 1.5f;
    
    [Header("References")]
    public Transform playerTransform;
    public GameObject playerPrefab;
    
    [Header("Boundaries")]
    public float leftBoundary = -3f;
    public float rightBoundary = 3f;
    
    private List<GameObject> crowdMembers = new List<GameObject>();
    private PlayerController mainPlayerController;
    private float memberColliderSize;
    
    void Start()
    {
        mainPlayerController = playerTransform.GetComponent<PlayerController>();
        
        if (playerPrefab != null)
        {
            Collider collider = playerPrefab.GetComponent<Collider>();
            if (collider != null)
            {
                memberColliderSize = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z) * 1.1f;
                minDistanceBetweenMembers = memberColliderSize;
            }
        }
        
        if (mainPlayerController != null)
        {
            leftBoundary = mainPlayerController.leftBoundary;
            rightBoundary = mainPlayerController.rightBoundary;
        }
    }
    
    void Update()
    {
        UpdateCrowdPositions();
    }
    
    public void AddCrowdMember()
    {
        if (crowdMembers.Count >= maxCrowdSize) return;
        
        Vector3 spawnPosition = GetRandomPositionInCrowd();
        GameObject newMember = Instantiate(playerPrefab, spawnPosition, playerTransform.rotation);
        
        PlayerController controller = newMember.GetComponent<PlayerController>();
        newMember.GetComponent<Animator>().SetTrigger("Run");
        if (controller != null)
        {
            Destroy(controller);
        }
        
        crowdMembers.Add(newMember);
    }
    
    public void RemoveCrowdMember()
    {
        if (crowdMembers.Count == 0) return;
        
        int randomIndex = Random.Range(0, crowdMembers.Count);
        GameObject memberToRemove = crowdMembers[randomIndex];
        crowdMembers.RemoveAt(randomIndex);
        Destroy(memberToRemove);
    }
    
    Vector3 GetRandomPositionInCrowd()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minDistanceBetweenMembers, crowdRadius);
        
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            0f,
            Mathf.Sin(angle) * distance
        );
        
        return playerTransform.position + offset;
    }
    
    void UpdateCrowdPositions()
    {
        if (mainPlayerController == null) return;
        
        for (int i = 0; i < crowdMembers.Count; i++)
        {
            if (crowdMembers[i] == null) continue;
            
            float angle = (360f / Mathf.Max(crowdMembers.Count, 1)) * i * Mathf.Deg2Rad;
            float distance = crowdRadius * 0.7f;
            
            Vector3 targetOffset = new Vector3(
                Mathf.Cos(angle) * distance,
                0f,
                Mathf.Sin(angle) * distance
            );
            
            Vector3 targetPosition = playerTransform.position + targetOffset;
            
            float clampedX = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);
            
            if (targetPosition.x != clampedX)
            {
                float deltaX = targetPosition.x - clampedX;
                targetPosition.x = clampedX;
                targetPosition.z += Mathf.Abs(deltaX) * 0.5f;
            }
            
            crowdMembers[i].transform.position = Vector3.Lerp(
                crowdMembers[i].transform.position,
                targetPosition,
                Time.deltaTime * 5f
            );
        }
    }
    
    void OnDrawGizmos()
    {
        if (playerTransform == null) return;
        
        Gizmos.color = Color.green;
        DrawCircle(playerTransform.position, crowdRadius, 32);
        
        Gizmos.color = Color.yellow;
        DrawCircle(playerTransform.position, minDistanceBetweenMembers, 16);
        
        Gizmos.color = Color.red;
        Vector3 leftPos = new Vector3(leftBoundary, playerTransform.position.y, playerTransform.position.z);
        Vector3 rightPos = new Vector3(rightBoundary, playerTransform.position.y, playerTransform.position.z);
        
        Gizmos.DrawLine(leftPos + Vector3.forward * 10, leftPos - Vector3.forward * 10);
        Gizmos.DrawLine(rightPos + Vector3.forward * 10, rightPos - Vector3.forward * 10);
    }
    
    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    
    public int GetCrowdCount()
    {
        return crowdMembers.Count;
    }
}