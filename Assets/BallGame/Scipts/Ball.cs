using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public GameObject owner;
    [HideInInspector] public float kickSpeed = 2f;
    [HideInInspector] public float kickDistance = 0.3f;
    [HideInInspector] public CrowdManager manager;
    
    private float kickTimer = 0f;
    private bool movingForward = true;
    private Vector3 basePosition;
    private Rigidbody rb;
    private float ballRadius;
    
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
            ballRadius = 0.5f;
        }
    }
    
    void Update()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }
        
        basePosition = owner.transform.position + owner.transform.forward * 1.5f;
        basePosition.x = owner.transform.position.x;
        basePosition.y = owner.transform.position.y + ballRadius;
        
        kickTimer += Time.deltaTime * kickSpeed;
        
        float kickOffset = Mathf.Sin(kickTimer) * kickDistance;
        Vector3 targetPosition = basePosition + Vector3.back * kickOffset;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        
        float rotationSpeed = 200f;
        transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0);
    }
    
    public IEnumerator FlyAway()
    {
        float randomDirection = Random.Range(-1f, 1f);
        Vector3 flyDirection = new Vector3(randomDirection, 0.2f, 0.5f).normalized;
        float flySpeed = 3f;
        float flyDuration = 1.5f;
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