using UnityEngine;
using TMPro;

public class LevelSelectionUI : MonoBehaviour
{
    public GameObject insufficientCoinsPanel;
    public TextMeshProUGUI coinsText;
    
    void Start()
    {
        if (insufficientCoinsPanel != null)
        {
            insufficientCoinsPanel.SetActive(false);
        }
        
        UpdateCoinsDisplay();
    }
    
    void Update()
    {
        UpdateCoinsDisplay();
    }
    
    void UpdateCoinsDisplay()
    {
        if (WalletController.Instance != null && coinsText != null)
        {
            coinsText.text = $"Coins: {WalletController.Instance.Coins}";
        }
    }
    
    public void ShowInsufficientCoinsPanel()
    {
        if (insufficientCoinsPanel != null)
        {
            insufficientCoinsPanel.SetActive(true);
        }
    }
    
    public void CloseInsufficientCoinsPanel()
    {
        if (insufficientCoinsPanel != null)
        {
            insufficientCoinsPanel.SetActive(false);
        }
    }
}