using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTrap : MonoBehaviour
{
    [Header("Wind Settings")]
    public float windWidth = 5f;
    public float windHeight = 2f;
    public float onDuration = 1.5f;
    public float offDuration = 3f;
    
    [Header("Visual Settings")]
    public int lineCount = 5;
    public float waveSpeed = 2f;
    public float waveAmplitude = 0.3f;
    public Color windColor = Color.cyan;
    
    [Header("Ball Force")]
    public float ballForce = 10f;
    
    private bool isActive = false;
    private List<LineRenderer> windLines = new List<LineRenderer>();
    private BoxCollider windTrigger;
    private CrowdManager crowdManager;
    private List<Ball> processedBalls = new List<Ball>();
    
    void Start()
    {
        crowdManager = FindObjectOfType<CrowdManager>();
        
        CreateWindTrigger();
        CreateWindLines();
        
        StartCoroutine(WindCycle());
    }
    
    void CreateWindTrigger()
    {
        GameObject triggerObj = new GameObject("WindTrigger");
        triggerObj.transform.parent = transform;
    
        Vector3 worldPos = transform.position + new Vector3(-windWidth / 2f, windHeight / 2f, 0);
        triggerObj.transform.position = worldPos;
    
        windTrigger = triggerObj.AddComponent<BoxCollider>();
        windTrigger.isTrigger = true;
        windTrigger.size = new Vector3(windWidth, windHeight, 1f);
    
        WindTriggerHandler handler = triggerObj.AddComponent<WindTriggerHandler>();
        handler.windTrap = this;
    
        windTrigger.enabled = false;
    }
    
    void CreateWindLines()
    {
        for (int i = 0; i < lineCount; i++)
        {
            GameObject lineObj = new GameObject($"WindLine_{i}");
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = Vector3.zero;
            
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = windColor;
            line.endColor = new Color(windColor.r, windColor.g, windColor.b, 0.2f);
            line.positionCount = 20;
            line.useWorldSpace = true;
            
            windLines.Add(line);
            lineObj.SetActive(false);
        }
    }
    
    IEnumerator WindCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(offDuration);
            
            ActivateWind();
            yield return new WaitForSeconds(onDuration);
            
            DeactivateWind();
        }
    }
    
    void ActivateWind()
    {
        isActive = true;
        windTrigger.enabled = true;
        processedBalls.Clear();
        
        foreach (var line in windLines)
        {
            line.gameObject.SetActive(true);
        }
    }
    
    void DeactivateWind()
    {
        isActive = false;
        windTrigger.enabled = false;
        
        foreach (var line in windLines)
        {
            line.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isActive)
        {
            UpdateWindLines();
        }
    }
    
    void UpdateWindLines()
    {
        for (int i = 0; i < windLines.Count; i++)
        {
            LineRenderer line = windLines[i];
            float yOffset = (windHeight / lineCount) * i;
            
            for (int j = 0; j < line.positionCount; j++)
            {
                float t = (float)j / (line.positionCount - 1);
                float x = -t * windWidth;
                float y = yOffset + Mathf.Sin(Time.time * waveSpeed + i + j * 0.5f) * waveAmplitude;
                
                Vector3 worldPos = transform.position + new Vector3(x, y, 0);
                line.SetPosition(j, worldPos);
            }
        }
    }
    
    public void OnBallEnter(Ball ball)
    {
        if (!isActive || ball == null || processedBalls.Contains(ball)) return;
        
        processedBalls.Add(ball);
        StartCoroutine(BlowAwayBall(ball));
    }
    
    IEnumerator BlowAwayBall(Ball ball)
    {
        if (ball == null || ball.gameObject == null)
        {
            yield break;
        }
        
        ball.owner = null;
        
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.left * ballForce + Vector3.up * (ballForce * 0.3f), ForceMode.Impulse);
        }
        
        float duration = 2f;
        float elapsed = 0f;
        
        while (elapsed < duration && ball != null)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (ball != null && ball.gameObject != null)
        {
            Destroy(ball.gameObject);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + new Vector3(-windWidth / 2f, windHeight / 2f, 0);
        Gizmos.DrawWireCube(center, new Vector3(windWidth, windHeight, 1f));
    }
}

public class WindTriggerHandler : MonoBehaviour
{
    public WindTrap windTrap;
    
    void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null && windTrap != null)
        {
            windTrap.OnBallEnter(ball);
        }
    }
}