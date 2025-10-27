using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private RectTransform _winPanel;
    [SerializeField] private RectTransform _loosePanel;
    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CrowdManager _crowdManager;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _ballsText;

    private float _oldSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _coinsText.text = $"{WalletController.Instance.Coins}";
        _ballsText.text = $"{_crowdManager.GetTotalBallCount()}";
    }

    public void HandleLoose()
    {
        _playerController.forwardSpeed = 0;
    }
    
    public void HandleLevelsButton()
    {
        SceneManager.LoadScene("Levels");
    }
    
    public void HandleMenuButton()
    {
        _oldSpeed = _playerController.forwardSpeed;
        _playerController.forwardSpeed = 0;
    }

    public void HandleCloseButton()
    {
        _playerController.forwardSpeed = _oldSpeed;
    }
}
