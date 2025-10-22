using System.Collections;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [Header("Animation Settings")]
    public float closeDuration = 0.15f;
    
    private bool isTriggered = false;
    private CrowdManager crowdManager;
    private Transform trapFlapA;
    private Transform trapFlapB;
    
    void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
        
        trapFlapA = transform.Find("Trap Flap A");
        trapFlapB = transform.Find("Trap Flap B");
        
        if (trapFlapA == null)
        {
            Debug.LogWarning("Trap Flap A not found!");
        }
        
        if (trapFlapB == null)
        {
            Debug.LogWarning("Trap Flap B not found!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            
            bool isMainPlayer = false;
            if (crowdManager != null)
            {
                isMainPlayer = (other.gameObject == crowdManager.playerTransform.gameObject);
            }
            
            StartCoroutine(CloseTrap(other.gameObject, isMainPlayer));
        }
    }
    
    IEnumerator CloseTrap(GameObject victim, bool isMainPlayer)
    {
        Quaternion flapAStartRotation = trapFlapA != null ? trapFlapA.localRotation : Quaternion.identity;
        Quaternion flapBStartRotation = trapFlapB != null ? trapFlapB.localRotation : Quaternion.identity;
        
        Quaternion flapATargetRotation = flapAStartRotation * Quaternion.Euler(-90, 0, 0);
        Quaternion flapBTargetRotation = flapBStartRotation * Quaternion.Euler(90, 0, 0);
        
        float elapsed = 0f;
        
        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / closeDuration;
            
            if (trapFlapA != null)
            {
                trapFlapA.localRotation = Quaternion.Lerp(flapAStartRotation, flapATargetRotation, t);
            }
            
            if (trapFlapB != null)
            {
                trapFlapB.localRotation = Quaternion.Lerp(flapBStartRotation, flapBTargetRotation, t);
            }
            
            yield return null;
        }
        
        if (trapFlapA != null)
        {
            trapFlapA.localRotation = flapATargetRotation;
        }
        
        if (trapFlapB != null)
        {
            trapFlapB.localRotation = flapBTargetRotation;
        }
        
        if (victim != null)
        {
            if (isMainPlayer)
            {
                Debug.Log("Main Player caught in Bear Trap - Game Over!");
            }
            
            if (crowdManager != null)
            {
                if (isMainPlayer)
                {
                    // Можно добавить логику Game Over здесь
                }
                else
                {
                    crowdManager.RemoveSpecificCrowdMember(victim);
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}