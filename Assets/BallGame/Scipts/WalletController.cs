using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance { get; private set; }
    
    private int coins = 0;
    
    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value;
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.Save();
        }
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
    }
    
    public void AddCoins(int amount)
    {
        Coins += amount;
    }
}