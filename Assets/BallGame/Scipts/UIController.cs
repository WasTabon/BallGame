using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CrowdManager _crowdManager;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _ballsText;

    private float _oldSpeed;

    private void Update()
    {
        _coinsText.text = $"{WalletController.Instance.Coins}";
        _ballsText.text = $"{_crowdManager.GetTotalBallCount()}";
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
