using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public string sceneName;
    public int cost = 30;
    public bool isFirstLevel = false;
    
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI levelText;
    private GameObject lockIcon;
    private bool isUnlocked;
    
    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        lockIcon = transform.Find("LockIcon").gameObject;
        
        CheckUnlockStatus();
        UpdateButtonState();
        
        button.onClick.AddListener(OnButtonClick);
    }
    
    void CheckUnlockStatus()
    {
        if (isFirstLevel)
        {
            isUnlocked = true;
        }
        else
        {
            isUnlocked = PlayerPrefs.GetInt($"Level_{levelIndex}_Unlocked", 0) == 1;
        }
    }
    
    void UpdateButtonState()
    {
        if (isUnlocked)
        {
            buttonImage.color = Color.white;
            lockIcon.SetActive(false);
            levelText.text = $"Level {levelIndex}";
        }
        else
        {
            buttonImage.color = Color.gray;
            lockIcon.SetActive(true);
            levelText.text = $"Level {levelIndex}\n{cost} coins";
        }
    }
    
    void OnButtonClick()
    {
        if (isUnlocked)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (WalletController.Instance != null && WalletController.Instance.Coins >= cost)
            {
                WalletController.Instance.Coins -= cost;
                PlayerPrefs.SetInt($"Level_{levelIndex}_Unlocked", 1);
                PlayerPrefs.Save();
                isUnlocked = true;
                UpdateButtonState();
            }
            else
            {
                LevelSelectionUI levelUI = FindObjectOfType<LevelSelectionUI>();
                if (levelUI != null)
                {
                    levelUI.ShowInsufficientCoinsPanel();
                }
            }
        }
    }
}