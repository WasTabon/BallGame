using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public GameObject owner;
    [HideInInspector] public float kickSpeed = 2f;
    [HideInInspector] public float kickDistance = 0.3f;
    [HideInInspector] public CrowdManager manager;
    [HideInInspector] public int ballIndex = 0;
    [HideInInspector] public int totalBallsForOwner = 1;
    
    private float kickTimer = 0f;
    private Vector3 basePosition;
    private Rigidbody rb;
    private float ballRadius;
    private float baseDistance = 1.5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            ballRadius = collider.bounds.extents.y;
        }
        else
        {
            ballRadius = 0.428f;
        }
    }
    
    void Update()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }
        
        float horizontalOffset = 0f;
        if (totalBallsForOwner > 1)
        {
            float spacing = ballRadius * 2.5f;
            float totalWidth = (totalBallsForOwner - 1) * spacing;
            horizontalOffset = -totalWidth / 2f + ballIndex * spacing;
        }
        
        if (manager != null)
        {
            baseDistance = manager.ballDistance;
        }
        
        basePosition = owner.transform.position + owner.transform.forward * baseDistance;
        basePosition.x = owner.transform.position.x + horizontalOffset;
        basePosition.y = owner.transform.position.y + ballRadius;
        
        kickTimer += Time.deltaTime * kickSpeed;
        float normalizedKick = (Mathf.Sin(kickTimer) + 1f) / 2f;
        
        Vector3 closestPosition = owner.transform.position + owner.transform.forward * (ballRadius * 3f) + new Vector3(horizontalOffset, ballRadius, 0);
        
        Vector3 targetPosition = Vector3.Lerp(closestPosition, basePosition, normalizedKick);
        
        float lerpSpeed = normalizedKick > 0.5f ? 8f : 3f;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        
        float rotationSpeed = 200f;
        transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0);
    }
    
    public IEnumerator FlyAway()
    {
        float randomDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
        Vector3 flyDirection = new Vector3(randomDirection * 0.8f, 0.1f, 0.3f).normalized;
        float flySpeed = 5f;
        float flyDuration = 2f;
        float elapsed = 0f;
        
        while (elapsed < flyDuration)
        {
            transform.position += flyDirection * flySpeed * Time.deltaTime;
            transform.Rotate(-200f * Time.deltaTime, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}