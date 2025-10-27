using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    
    [Header("Trap Settings")]
    public bool canKillMainPlayer = true;
    
    private CrowdManager crowdManager;
    
    void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
    }
    
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (crowdManager == null) return;
        
        if (other.CompareTag("Player"))
        {
            if (canKillMainPlayer)
            {
                // Здесь можно добавить логику смерти главного игрока
                // Например, показать экран Game Over
                Debug.Log("Main Player Hit Trap - Game Over!");
                UIController.Instance.HandleLoose();
                // Можно добавить: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        
        var crowdMembers = crowdManager.GetCrowdMembers();
        foreach (var member in crowdMembers)
        {
            if (member == other.gameObject)
            {
                crowdManager.RemoveSpecificCrowdMember(member);
                break;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}