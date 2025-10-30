using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float sidewaysSpeed = 3f;
    public float leftBoundary = -3f;
    public float rightBoundary = 3f;
    
    [Header("Swipe Settings")]
    public float swipeSensitivity = 0.01f;
    public float smoothSpeed = 10f;
    public float stopSpeed = 15f;
    
    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public float baseFootstepInterval = 0.4f;
    public int minCrowdSoundsPerStep = 1;
    public int maxCrowdSoundsPerStep = 5;
    public float crowdSoundRandomDelay = 0.05f;
    
    private Animator animator;
    private bool isRunning = false;
    private float targetHorizontalVelocity = 0f;
    private float currentHorizontalVelocity = 0f;
    private Vector2 lastInputPosition;
    private bool isInputActive = false;
    private float speedMultiplier = 1f;
    private float footstepTimer = 0f;
    private CrowdManager crowdManager;
    
    private Vector3 lastPosition;
    
    private Rigidbody rb;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        crowdManager = FindObjectOfType<CrowdManager>();
        rb = GetComponent<Rigidbody>();
    
        lastPosition = transform.position;
    }

    
    void StartRunning()
    {
        isRunning = true;
        animator.SetTrigger("Run");
    }
    
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    
    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }
    
    void Update()
    {
        HandleInput();
        HandleFootsteps();
    
        currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, Time.deltaTime * smoothSpeed);
    
        if (!isInputActive)
        {
            targetHorizontalVelocity = Mathf.Lerp(targetHorizontalVelocity, 0f, Time.deltaTime * stopSpeed);
        
            if (Mathf.Abs(targetHorizontalVelocity) < 0.01f)
            {
                targetHorizontalVelocity = 0f;
            }
            if (Mathf.Abs(currentHorizontalVelocity) < 0.01f)
            {
                currentHorizontalVelocity = 0f;
            }
        }
    
        Vector3 movement = new Vector3(currentHorizontalVelocity * -sidewaysSpeed, 0, -forwardSpeed * speedMultiplier) * Time.deltaTime;
        Vector3 newPosition = rb.position + movement; // ← ЗАМЕНИЛ transform.position на rb.position

        float clampedX = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        rb.MovePosition(new Vector3(clampedX, newPosition.y, newPosition.z)); // ← ЗАМЕНИЛ transform.position на rb.MovePosition
    }
    
    void HandleFootsteps()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        
        float adjustedInterval = baseFootstepInterval / speedMultiplier;
        footstepTimer += Time.deltaTime;
        
        if (footstepTimer >= adjustedInterval)
        {
            footstepTimer = 0f;
            PlayFootstepSounds();
        }
    }
    
    void PlayFootstepSounds()
    {
        int crowdCount = crowdManager != null ? crowdManager.GetTotalBallCount() : 1;
        int soundsToPlay = Mathf.Clamp(Mathf.CeilToInt(crowdCount / 10f), minCrowdSoundsPerStep, maxCrowdSoundsPerStep);
        
        for (int i = 0; i < soundsToPlay; i++)
        {
            AudioClip randomFootstep = footstepSounds[Random.Range(0, footstepSounds.Length)];
            float delay = Random.Range(0f, crowdSoundRandomDelay);
            
            if (delay > 0)
            {
                StartCoroutine(PlayFootstepWithDelay(randomFootstep, delay));
            }
            else
            {
                MusicController.Instance.PlaySpecificSound(randomFootstep);
            }
        }
    }
    
    IEnumerator PlayFootstepWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        MusicController.Instance.PlaySpecificSound(clip);
    }
    
    void HandleInput()
    {
        // Приоритет тачу на мобильных устройствах
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
        
            if (touch.phase == TouchPhase.Began)
            {
                isInputActive = true;
                lastInputPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isInputActive = false;
            }
            else if (touch.phase == TouchPhase.Moved && isInputActive)
            {
                targetHorizontalVelocity += touch.deltaPosition.x * swipeSensitivity * 0.1f;
                targetHorizontalVelocity = Mathf.Clamp(targetHorizontalVelocity, -1f, 1f);
            }
        }
        // Мышь только если нет тачей
        else if (Input.GetMouseButtonDown(0))
        {
            isInputActive = true;
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isInputActive = false;
        }
        else if (Input.GetMouseButton(0) && isInputActive)
        {
            Vector2 currentPosition = Input.mousePosition;
            float deltaX = currentPosition.x - lastInputPosition.x;
        
            targetHorizontalVelocity += deltaX * swipeSensitivity;
            targetHorizontalVelocity = Mathf.Clamp(targetHorizontalVelocity, -1f, 1f);
        
            lastInputPosition = currentPosition;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 leftPos = new Vector3(leftBoundary, transform.position.y, transform.position.z);
        Vector3 rightPos = new Vector3(rightBoundary, transform.position.y, transform.position.z);
        
        Gizmos.DrawLine(leftPos + Vector3.forward * 10, leftPos - Vector3.forward * 10);
        Gizmos.DrawLine(rightPos + Vector3.forward * 10, rightPos - Vector3.forward * 10);
        
        Gizmos.DrawLine(leftPos, rightPos);
    }
    
    void OnDestroy()
    {
        GameEntryPoint.OnPlayerStart -= StartRunning;
    }
}