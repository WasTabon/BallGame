using System.Collections;
using UnityEngine;

public class CoinCollectable : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public GameObject coinPrefab;
    
    [Header("Particle Settings")]
    public int particleCount = 10;
    public float explosionForce = 5f;
    public float particleLifetime = 1f;
    public float particleScale = 0.3f;
    
    private bool isCollected = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            CollectCoin();
        }
    }
    
    void CollectCoin()
    {
        if (WalletController.Instance != null)
        {
            WalletController.Instance.AddCoins(coinValue);
        }
        
        SpawnParticles();
        
        gameObject.SetActive(false);
    }
    
    void SpawnParticles()
    {
        if (coinPrefab == null) return;
        
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = Instantiate(coinPrefab, transform.position, Random.rotation);
            
            particle.transform.localScale = Vector3.one * particleScale;
            
            Collider col = particle.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
            
            CoinCollectable script = particle.GetComponent<CoinCollectable>();
            if (script != null)
            {
                Destroy(script);
            }
            
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = particle.AddComponent<Rigidbody>();
            }
            
            rb.useGravity = true;
            
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            
            rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            
            Destroy(particle, particleLifetime);
        }
    }
}