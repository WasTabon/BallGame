using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    
    [Header("Follow Settings")]
    public float smoothSpeed = 5f;
    
    private Vector3 offset;
    private bool offsetCalculated = false;
    
    void LateUpdate()
    {
        if (player == null) return;
        
        if (!offsetCalculated)
        {
            offset = transform.position - player.position;
            offsetCalculated = true;
        }
        
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, player.position.z + offset.z);
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}