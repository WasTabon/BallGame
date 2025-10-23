using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("Finish Requirements")]
    public int requiredBalls = 10;
    
    private CrowdManager crowdManager;
    private bool hasFinished = false;
    
    void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return;
        
        if (other.CompareTag("Player"))
        {
            if (crowdManager != null)
            {
                bool isMainPlayer = (other.gameObject == crowdManager.playerTransform.gameObject);
                
                if (isMainPlayer)
                {
                    hasFinished = true;
                    CheckWinCondition();
                }
            }
        }
    }
    
    void CheckWinCondition()
    {
        if (crowdManager == null) return;
        
        int totalBalls = crowdManager.GetTotalBallCount();
        
        if (totalBalls >= requiredBalls)
        {
            Debug.Log($"VICTORY! You finished with {totalBalls} balls (Required: {requiredBalls})");
        }
        else
        {
            Debug.Log($"DEFEAT! You finished with {totalBalls} balls (Required: {requiredBalls})");
        }
    }
    
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}