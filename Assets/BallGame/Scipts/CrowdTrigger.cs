using UnityEngine;

public class CrowdTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public int crowdChange = 1;
    
    private bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            CrowdManager crowdManager = FindObjectOfType<CrowdManager>();
            
            if (crowdManager != null)
            {
                if (crowdChange > 0)
                {
                    for (int i = 0; i < crowdChange; i++)
                    {
                        crowdManager.AddCrowdMember();
                    }
                }
                else if (crowdChange < 0)
                {
                    for (int i = 0; i < Mathf.Abs(crowdChange); i++)
                    {
                        crowdManager.RemoveCrowdMember();
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
            Gizmos.color = crowdChange > 0 ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}