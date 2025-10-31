using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private RectTransform _winPanel;
    [SerializeField] private RectTransform _loosePanel;
    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CrowdManager _crowdManager;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _ballsText;
    [SerializeField] private TextMeshProUGUI _coinsText1;
    [SerializeField] private TextMeshProUGUI _ballsText1;
    [SerializeField] private TextMeshProUGUI _coinsText2;
    [SerializeField] private TextMeshProUGUI _ballsText2;

    private bool isPanel;

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

    [ContextMenu("Loose")]
    public void HandleLoose()
    {
        if (isPanel) return;
        isPanel = true;
        
        _playerController.forwardSpeed = 0;
        _coinsText1.text = $"{WalletController.Instance.Coins}";
        _ballsText1.text = $"{_crowdManager.GetTotalBallCount()}";
        _coinsText2.text = $"{WalletController.Instance.Coins}";
        _ballsText2.text = $"{_crowdManager.GetTotalBallCount()}";
        ShowPanel(_loosePanel);
    }

    [ContextMenu("Win")]
    public void HandleWin()
    {
        Debug.Log("Win start");
        
        if (isPanel)
        {
            Debug.Log("Return");
            return;
        }
        
        Debug.Log("Win continue");
        isPanel = true;
        
        _playerController.forwardSpeed = 0;
        _coinsText1.text = $"{WalletController.Instance.Coins}";
        _ballsText1.text = $"{_crowdManager.GetTotalBallCount()}";
        _coinsText2.text = $"{WalletController.Instance.Coins}";
        _ballsText2.text = $"{_crowdManager.GetTotalBallCount()}";
        ShowPanel(_winPanel);
    }

    private void ShowPanel(RectTransform panel)
    {
        panel.gameObject.SetActive(true);
        
        CanvasGroup bgCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (bgCanvasGroup == null)
            bgCanvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        
        bgCanvasGroup.alpha = 0;
        bgCanvasGroup.DOFade(1f, 0.3f);
        
        if (panel.childCount > 0)
        {
            RectTransform mainPanel = panel.GetChild(0) as RectTransform;
            
            CanvasGroup mainPanelCanvasGroup = mainPanel.GetComponent<CanvasGroup>();
            if (mainPanelCanvasGroup == null)
                mainPanelCanvasGroup = mainPanel.gameObject.AddComponent<CanvasGroup>();
            
            mainPanelCanvasGroup.alpha = 0;
            mainPanel.localScale = Vector3.zero;
            
            Sequence mainPanelSequence = DOTween.Sequence();
            mainPanelSequence.Append(mainPanel.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f));
            mainPanelSequence.Join(mainPanelCanvasGroup.DOFade(1f, 0.3f));
            mainPanelSequence.Append(mainPanel.DOScale(1f, 0.15f).SetEase(Ease.InOutSine));
            
            for (int i = 0; i < mainPanel.childCount; i++)
            {
                RectTransform child = mainPanel.GetChild(i) as RectTransform;
                
                CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                if (childCanvasGroup == null)
                    childCanvasGroup = child.gameObject.AddComponent<CanvasGroup>();
                
                childCanvasGroup.alpha = 0;
                child.localScale = Vector3.zero;
                Vector3 originalPosition = child.anchoredPosition;
                child.anchoredPosition = new Vector3(originalPosition.x, originalPosition.y - 50f, 0);
                
                float delay = 0.6f + (i * 0.1f);
                
                Sequence childSequence = DOTween.Sequence();
                childSequence.Append(child.DOScale(1f, 0.5f).SetEase(Ease.OutElastic, 1.2f, 0.5f).SetDelay(delay));
                childSequence.Join(childCanvasGroup.DOFade(1f, 0.4f).SetDelay(delay));
                childSequence.Join(child.DOAnchorPos(originalPosition, 0.5f).SetEase(Ease.OutBack).SetDelay(delay));
            }
        }
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