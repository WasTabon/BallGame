using UnityEngine;

public class SpeedZoneTrigger : MonoBehaviour
{
    [Header("Speed Zone Settings")]
    public float speedMultiplier = 0.5f;
    
    private CrowdManager crowdManager;
    
    void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && crowdManager != null)
        {
            crowdManager.SetZoneSpeedMultiplier(speedMultiplier);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && crowdManager != null)
        {
            crowdManager.ResetZoneSpeedMultiplier();
        }
    }
    
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = speedMultiplier < 1f ? Color.blue : Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}