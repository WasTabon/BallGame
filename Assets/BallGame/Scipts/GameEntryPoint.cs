using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    public static GameEntryPoint Instance { get; private set; }
    
    public static System.Action OnPlayerStart;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartCoroutine(StartGameDelayed());
    }
    
    IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
    }
    
    public void StartGame()
    {
        StartPlayer();
    }
    
    public void StartPlayer()
    {
        OnPlayerStart?.Invoke();
    }
}