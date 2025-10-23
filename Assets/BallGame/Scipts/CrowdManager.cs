using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("Crowd Settings")]
    public int maxCrowdSize = 20;
    public float crowdRadius = 3f;
    public float minDistanceBetweenMembers = 1.5f;
    public float speedChangePerMember = 0.05f;
    
    [Header("References")]
    public Transform playerTransform;
    public GameObject playerPrefab;
    public GameObject ballPrefab;
    
    [Header("Effects")]
    public GameObject playerDestroyEffectPrefab;
    
    [Header("Boundaries")]
    public float leftBoundary = -3f;
    public float rightBoundary = 3f;
    
    [Header("Ball Settings")]
    public float ballDistance = 1.5f;
    public float kickSpeed = 2f;
    public float kickDistance = 0.3f;
    
    private List<GameObject> crowdMembers = new List<GameObject>();
    private List<Ball> allBalls = new List<Ball>();
    private Dictionary<Ball, Coroutine> ballRemovalCoroutines = new Dictionary<Ball, Coroutine>();
    private PlayerController mainPlayerController;
    private float memberColliderSize;
    private float baseSpeedMultiplier = 1f;
    private float zoneSpeedMultiplier = 1f;
    
    void Start()
    {
        mainPlayerController = playerTransform.GetComponent<PlayerController>();
        
        if (playerPrefab != null)
        {
            Collider collider = playerPrefab.GetComponent<Collider>();
            if (collider != null)
            {
                memberColliderSize = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z) * 1.1f;
                minDistanceBetweenMembers = memberColliderSize;
            }
        }
        
        if (mainPlayerController != null)
        {
            leftBoundary = mainPlayerController.leftBoundary;
            rightBoundary = mainPlayerController.rightBoundary;
        }
        
        AddBallToPlayer(playerTransform.gameObject);
        UpdateSpeed();
        
        // зробити анімації партікли, рівні і ui
    }
    
    void Update()
    {
        UpdateCrowdPositions();
        UpdateBalls();
        UpdateBallIndices();
    }
    
    public void AddCrowdMember()
    {
        if (crowdMembers.Count >= maxCrowdSize) return;
        
        Vector3 spawnPosition = GetRandomPositionInCrowd();
        GameObject newMember = Instantiate(playerPrefab, spawnPosition, playerTransform.rotation);
        
        PlayerController controller = newMember.GetComponent<PlayerController>();
        newMember.GetComponent<Animator>().SetTrigger("Run");
        if (controller != null)
        {
            Destroy(controller);
        }
        
        StartCoroutine(SpawnMemberAnimation(newMember));
        
        crowdMembers.Add(newMember);
        
        RedistributeBalls();
        UpdateSpeed();
    }
    
    IEnumerator SpawnMemberAnimation(GameObject member)
    {
        Vector3 targetScale = member.transform.localScale;
        member.transform.localScale = Vector3.zero;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration && member != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            member.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        
        if (member != null)
        {
            member.transform.localScale = targetScale;
        }
    }
    
    public void RemoveCrowdMember()
    {
        if (crowdMembers.Count == 0) return;
        
        int randomIndex = Random.Range(0, crowdMembers.Count);
        GameObject memberToRemove = crowdMembers[randomIndex];
        
        RemoveAllBallsFromPlayer(memberToRemove);
        SpawnPlayerDestroyEffect(memberToRemove.transform.position);
        
        crowdMembers.RemoveAt(randomIndex);
        Destroy(memberToRemove);
        UpdateSpeed();
    }
    
    public void RemoveSpecificCrowdMember(GameObject member)
    {
        if (crowdMembers.Contains(member))
        {
            RemoveAllBallsFromPlayer(member);
            SpawnPlayerDestroyEffect(member.transform.position);
            crowdMembers.Remove(member);
            Destroy(member);
            UpdateSpeed();
        }
    }
    
    void SpawnPlayerDestroyEffect(Vector3 position)
    {
        if (playerDestroyEffectPrefab != null)
        {
            GameObject effect = Instantiate(playerDestroyEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    
    void UpdateSpeed()
    {
        baseSpeedMultiplier = 1f + (crowdMembers.Count * speedChangePerMember);
        ApplySpeed();
    }
    
    void ApplySpeed()
    {
        if (mainPlayerController != null)
        {
            mainPlayerController.SetSpeedMultiplier(baseSpeedMultiplier * zoneSpeedMultiplier);
        }
    }
    
    public void SetZoneSpeedMultiplier(float multiplier)
    {
        zoneSpeedMultiplier = multiplier;
        ApplySpeed();
    }
    
    public void ResetZoneSpeedMultiplier()
    {
        zoneSpeedMultiplier = 1f;
        ApplySpeed();
    }
    
    public List<GameObject> GetCrowdMembers()
    {
        return new List<GameObject>(crowdMembers);
    }
    
    void RedistributeBalls()
    {
        List<GameObject> allPlayers = GetAllPlayers();
        List<Ball> ballsToRedistribute = new List<Ball>();
        
        foreach (var ball in allBalls)
        {
            if (ball != null && GetBallCountForPlayer(ball.owner) > 1)
            {
                ballsToRedistribute.Add(ball);
            }
        }
        
        foreach (var ball in ballsToRedistribute)
        {
            GameObject playerWithoutBall = null;
            foreach (var player in allPlayers)
            {
                if (GetBallCountForPlayer(player) == 0)
                {
                    playerWithoutBall = player;
                    break;
                }
            }
            
            if (playerWithoutBall != null)
            {
                if (ballRemovalCoroutines.ContainsKey(ball))
                {
                    StopCoroutine(ballRemovalCoroutines[ball]);
                    ballRemovalCoroutines.Remove(ball);
                }
                
                ball.owner = playerWithoutBall;
            }
        }
    }
    
    public void AddBall()
    {
        List<GameObject> allPlayers = GetAllPlayers();
        if (allPlayers.Count == 0) return;
        
        Dictionary<GameObject, int> ballCounts = new Dictionary<GameObject, int>();
        foreach (var player in allPlayers)
        {
            ballCounts[player] = GetBallCountForPlayer(player);
        }
        
        GameObject targetPlayer = null;
        int minBalls = int.MaxValue;
        
        foreach (var kvp in ballCounts)
        {
            if (kvp.Value < minBalls)
            {
                minBalls = kvp.Value;
                targetPlayer = kvp.Key;
            }
        }
        
        if (targetPlayer != null)
        {
            AddBallToPlayer(targetPlayer);
        }
    }
    
    public void RemoveBall()
    {
        if (allBalls.Count == 0) return;
        
        List<GameObject> playersWithBalls = new List<GameObject>();
        foreach (var ball in allBalls)
        {
            if (ball != null && ball.owner != null && !playersWithBalls.Contains(ball.owner))
            {
                playersWithBalls.Add(ball.owner);
            }
        }
        
        if (playersWithBalls.Count == 0) return;
        
        GameObject randomPlayer = playersWithBalls[Random.Range(0, playersWithBalls.Count)];
        RemoveBallFromPlayer(randomPlayer);
    }
    
    void AddBallToPlayer(GameObject player)
    {
        if (ballPrefab == null) return;
        
        Vector3 spawnPos = player.transform.position + player.transform.forward * ballDistance;
        GameObject ballObj = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        
        Ball ball = ballObj.GetComponent<Ball>();
        if (ball == null)
        {
            ball = ballObj.AddComponent<Ball>();
        }
        
        ball.owner = player;
        ball.kickSpeed = kickSpeed;
        ball.kickDistance = kickDistance;
        ball.manager = this;
        
        allBalls.Add(ball);
        
        int ballCount = GetBallCountForPlayer(player);
        if (ballCount > 1)
        {
            Coroutine removalCoroutine = StartCoroutine(RemoveBallAfterDelay(ball, 8f));
            ballRemovalCoroutines[ball] = removalCoroutine;
        }
    }
    
    void RemoveBallFromPlayer(GameObject player)
    {
        for (int i = allBalls.Count - 1; i >= 0; i--)
        {
            if (allBalls[i] != null && allBalls[i].owner == player)
            {
                Ball ballToRemove = allBalls[i];
                
                if (ballRemovalCoroutines.ContainsKey(ballToRemove))
                {
                    StopCoroutine(ballRemovalCoroutines[ballToRemove]);
                    ballRemovalCoroutines.Remove(ballToRemove);
                }
                
                allBalls.RemoveAt(i);
                Destroy(ballToRemove.gameObject);
                return;
            }
        }
    }
    
    void RemoveAllBallsFromPlayer(GameObject player)
    {
        for (int i = allBalls.Count - 1; i >= 0; i--)
        {
            if (allBalls[i] != null && allBalls[i].owner == player)
            {
                Ball ballToRemove = allBalls[i];
                
                if (ballRemovalCoroutines.ContainsKey(ballToRemove))
                {
                    StopCoroutine(ballRemovalCoroutines[ballToRemove]);
                    ballRemovalCoroutines.Remove(ballToRemove);
                }
                
                allBalls.RemoveAt(i);
                Destroy(ballToRemove.gameObject);
            }
        }
    }
    
    IEnumerator RemoveBallAfterDelay(Ball ball, float delay)
    {
        yield return new WaitForSeconds(delay);
    
        if (ball != null && allBalls.Contains(ball) && GetBallCountForPlayer(ball.owner) > 1)
        {
            StartCoroutine(ball.FlyAway());
            allBalls.Remove(ball);
        
            if (ballRemovalCoroutines.ContainsKey(ball))
            {
                ballRemovalCoroutines.Remove(ball);
            }
        }
    }
    
    void UpdateBallIndices()
    {
        Dictionary<GameObject, List<Ball>> ballsByOwner = new Dictionary<GameObject, List<Ball>>();
        
        foreach (var ball in allBalls)
        {
            if (ball != null && ball.owner != null)
            {
                if (!ballsByOwner.ContainsKey(ball.owner))
                {
                    ballsByOwner[ball.owner] = new List<Ball>();
                }
                ballsByOwner[ball.owner].Add(ball);
            }
        }
        
        foreach (var kvp in ballsByOwner)
        {
            int totalBalls = kvp.Value.Count;
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                kvp.Value[i].ballIndex = i;
                kvp.Value[i].totalBallsForOwner = totalBalls;
            }
        }
    }
    
    int GetBallCountForPlayer(GameObject player)
    {
        int count = 0;
        foreach (var ball in allBalls)
        {
            if (ball != null && ball.owner == player)
            {
                count++;
            }
        }
        return count;
    }
    
    public int GetTotalBallCount()
    {
        return allBalls.Count;
    }
    
    int GetTotalPlayerCount()
    {
        return crowdMembers.Count + 1;
    }
    
    List<GameObject> GetAllPlayers()
    {
        List<GameObject> players = new List<GameObject> { playerTransform.gameObject };
        players.AddRange(crowdMembers);
        return players;
    }
    
    void UpdateBalls()
    {
        for (int i = allBalls.Count - 1; i >= 0; i--)
        {
            if (allBalls[i] == null)
            {
                allBalls.RemoveAt(i);
            }
        }
    }
    
    Vector3 GetRandomPositionInCrowd()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minDistanceBetweenMembers, crowdRadius);
        
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            0f,
            Mathf.Sin(angle) * distance
        );
        
        return playerTransform.position + offset;
    }
    
    void UpdateCrowdPositions()
    {
        if (mainPlayerController == null) return;
        
        for (int i = 0; i < crowdMembers.Count; i++)
        {
            if (crowdMembers[i] == null) continue;
            
            float angle = (360f / Mathf.Max(crowdMembers.Count, 1)) * i * Mathf.Deg2Rad;
            float distance = crowdRadius * 0.7f;
            
            Vector3 targetOffset = new Vector3(
                Mathf.Cos(angle) * distance,
                0f,
                Mathf.Sin(angle) * distance
            );
            
            Vector3 targetPosition = playerTransform.position + targetOffset;
            
            float clampedX = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);
            
            if (targetPosition.x != clampedX)
            {
                float deltaX = targetPosition.x - clampedX;
                targetPosition.x = clampedX;
                targetPosition.z += Mathf.Abs(deltaX) * 0.5f;
            }
            
            crowdMembers[i].transform.position = Vector3.Lerp(
                crowdMembers[i].transform.position,
                targetPosition,
                Time.deltaTime * 5f
            );
        }
    }
    
    void OnDrawGizmos()
    {
        if (playerTransform == null) return;
        
        Gizmos.color = Color.green;
        DrawCircle(playerTransform.position, crowdRadius, 32);
        
        Gizmos.color = Color.yellow;
        DrawCircle(playerTransform.position, minDistanceBetweenMembers, 16);
        
        Gizmos.color = Color.red;
        Vector3 leftPos = new Vector3(leftBoundary, playerTransform.position.y, playerTransform.position.z);
        Vector3 rightPos = new Vector3(rightBoundary, playerTransform.position.y, playerTransform.position.z);
        
        Gizmos.DrawLine(leftPos + Vector3.forward * 10, leftPos - Vector3.forward * 10);
        Gizmos.DrawLine(rightPos + Vector3.forward * 10, rightPos - Vector3.forward * 10);
    }
    
    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    
    public int GetCrowdCount()
    {
        return crowdMembers.Count;
    }
}