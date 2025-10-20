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
    public float stopSpeed = 15f; // Скорость остановки
    
    private Rigidbody rb;
    private Animator animator;
    private bool isRunning = false;
    private float targetHorizontalVelocity = 0f;
    private float currentHorizontalVelocity = 0f;
    private Vector2 lastInputPosition;
    private bool isInputActive = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        GameEntryPoint.OnPlayerStart += StartRunning;
    }
    
    void StartRunning()
    {
        isRunning = true;
        animator.SetTrigger("Run");
    }
    
    void Update()
    {
        if (!isRunning) return;
        
        HandleInput();
        
        // Плавное сглаживание движения
        currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, Time.deltaTime * smoothSpeed);
        
        // Быстрая остановка когда нет ввода
        if (!isInputActive)
        {
            targetHorizontalVelocity = Mathf.Lerp(targetHorizontalVelocity, 0f, Time.deltaTime * stopSpeed);
            
            // Полная остановка при очень малых значениях
            if (Mathf.Abs(targetHorizontalVelocity) < 0.01f)
            {
                targetHorizontalVelocity = 0f;
            }
            if (Mathf.Abs(currentHorizontalVelocity) < 0.01f)
            {
                currentHorizontalVelocity = 0f;
            }
        }
    }
    
    void HandleInput()
    {
        // PC input
        if (Input.GetMouseButtonDown(0))
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
        
        // Mobile input
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
                targetHorizontalVelocity += touch.deltaPosition.x * swipeSensitivity;
                targetHorizontalVelocity = Mathf.Clamp(targetHorizontalVelocity, -1f, 1f);
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!isRunning) return;
        
        Vector3 movement = new Vector3(currentHorizontalVelocity * -sidewaysSpeed, 0, -forwardSpeed);
        rb.velocity = movement;
        
        float clampedX = Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
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