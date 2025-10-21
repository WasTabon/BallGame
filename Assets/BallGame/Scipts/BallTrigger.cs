using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public int ballChange = 1;
    
    private bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            CrowdManager crowdManager = FindObjectOfType<CrowdManager>();
            
            if (crowdManager != null)
            {
                if (ballChange > 0)
                {
                    for (int i = 0; i < ballChange; i++)
                    {
                        crowdManager.AddBall();
                    }
                }
                else if (ballChange < 0)
                {
                    for (int i = 0; i < Mathf.Abs(ballChange); i++)
                    {
                        crowdManager.RemoveBall();
                    }
                }
                
                hasBeenTriggered = true;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = ballChange > 0 ? Color.cyan : Color.magenta;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}