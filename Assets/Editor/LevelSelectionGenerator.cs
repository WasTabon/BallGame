#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;

public class LevelSelectionGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Level Selection UI")]
    static void GenerateLevelSelection()
    {
        CreateLevelButtonPrefab();
        CreateLevelSelectionCanvas();
    }
    
    static void CreateLevelButtonPrefab()
    {
        string prefabPath = "Assets/Prefabs/UI";
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
        }
        
        GameObject buttonPrefab = new GameObject("LevelButtonPrefab");
        
        Image buttonImage = buttonPrefab.AddComponent<Image>();
        buttonImage.color = Color.white;
        
        Button button = buttonPrefab.AddComponent<Button>();
        
        RectTransform buttonRect = buttonPrefab.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 200);
        
        GameObject textObj = new GameObject("LevelText");
        textObj.transform.SetParent(buttonPrefab.transform);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Level X";
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        GameObject lockObj = new GameObject("LockIcon");
        lockObj.transform.SetParent(buttonPrefab.transform);
        Image lockImage = lockObj.AddComponent<Image>();
        lockImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform lockRect = lockObj.GetComponent<RectTransform>();
        lockRect.sizeDelta = new Vector2(80, 80);
        lockRect.anchoredPosition = Vector2.zero;
        
        buttonPrefab.AddComponent<LevelButton>();
        
        string fullPath = $"{prefabPath}/LevelButtonPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(buttonPrefab, fullPath);
        DestroyImmediate(buttonPrefab);
        
        Debug.Log($"Level Button Prefab created at: {fullPath}");
    }
    
    static void CreateLevelSelectionCanvas()
    {
        GameObject canvasObj = new GameObject("LevelSelectionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 2340);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject coinsDisplayObj = new GameObject("CoinsDisplay");
        coinsDisplayObj.transform.SetParent(canvasObj.transform);
        TextMeshProUGUI coinsText = coinsDisplayObj.AddComponent<TextMeshProUGUI>();
        coinsText.text = "Coins: 0";
        coinsText.fontSize = 48;
        coinsText.alignment = TextAlignmentOptions.Center;
        coinsText.color = Color.yellow;
        
        RectTransform coinsRect = coinsDisplayObj.GetComponent<RectTransform>();
        coinsRect.anchorMin = new Vector2(0.5f, 1f);
        coinsRect.anchorMax = new Vector2(0.5f, 1f);
        coinsRect.pivot = new Vector2(0.5f, 1f);
        coinsRect.anchoredPosition = new Vector2(0, -50);
        coinsRect.sizeDelta = new Vector2(400, 80);
        
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(canvasObj.transform);
        
        RectTransform scrollRect = scrollViewObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(0, 0);
        scrollRect.offsetMax = new Vector2(0, -150);
        
        ScrollRect scroll = scrollViewObj.AddComponent<ScrollRect>();
        
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewObj.transform);
        
        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;
        
        viewportObj.AddComponent<Image>().color = Color.clear;
        viewportObj.AddComponent<Mask>().showMaskGraphic = false;
        
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform);
        
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 1f);
        contentRect.anchorMax = new Vector2(0.5f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(1000, 0);
        
        GridLayoutGroup gridLayout = contentObj.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(200, 200);
        gridLayout.spacing = new Vector2(30, 30);
        gridLayout.padding = new RectOffset(40, 40, 40, 40);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 4;
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        
        ContentSizeFitter fitter = contentObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        scroll.vertical = true;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/LevelButtonPrefab.prefab");
        
        string[] sceneNames = { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5" };
        
        for (int i = 1; i <= 30; i++)
        {
            GameObject buttonInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            buttonInstance.transform.SetParent(contentObj.transform);
            buttonInstance.name = $"LevelButton_{i}";
            
            LevelButton levelButton = buttonInstance.GetComponent<LevelButton>();
            levelButton.levelIndex = i;
            levelButton.cost = 30;
            levelButton.isFirstLevel = (i == 1);
            
            if (i <= 5)
            {
                levelButton.sceneName = sceneNames[i - 1];
            }
            else
            {
                int randomIndex = Random.Range(0, sceneNames.Length);
                levelButton.sceneName = sceneNames[randomIndex];
            }
        }
        
        GameObject insufficientPanel = new GameObject("InsufficientCoinsPanel");
        insufficientPanel.transform.SetParent(canvasObj.transform);
        
        RectTransform panelRect = insufficientPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelBg = insufficientPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);
        
        GameObject messageBox = new GameObject("MessageBox");
        messageBox.transform.SetParent(insufficientPanel.transform);
        
        RectTransform msgRect = messageBox.AddComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.pivot = new Vector2(0.5f, 0.5f);
        msgRect.sizeDelta = new Vector2(600, 400);
        msgRect.anchoredPosition = Vector2.zero;
        
        Image msgBg = messageBox.AddComponent<Image>();
        msgBg.color = Color.white;
        
        GameObject msgTextObj = new GameObject("MessageText");
        msgTextObj.transform.SetParent(messageBox.transform);
        TextMeshProUGUI msgText = msgTextObj.AddComponent<TextMeshProUGUI>();
        msgText.text = "Недостаточно монет!";
        msgText.fontSize = 42;
        msgText.alignment = TextAlignmentOptions.Center;
        msgText.color = Color.black;
        
        RectTransform msgTextRect = msgTextObj.GetComponent<RectTransform>();
        msgTextRect.anchorMin = new Vector2(0, 0.5f);
        msgTextRect.anchorMax = new Vector2(1, 1);
        msgTextRect.sizeDelta = Vector2.zero;
        msgTextRect.anchoredPosition = Vector2.zero;
        
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(messageBox.transform);
        
        RectTransform closeBtnRect = closeButton.AddComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.5f, 0);
        closeBtnRect.anchorMax = new Vector2(0.5f, 0);
        closeBtnRect.pivot = new Vector2(0.5f, 0);
        closeBtnRect.sizeDelta = new Vector2(200, 80);
        closeBtnRect.anchoredPosition = new Vector2(0, 50);
        
        Image closeBtnImage = closeButton.AddComponent<Image>();
        closeBtnImage.color = Color.green;
        
        Button closeBtn = closeButton.AddComponent<Button>();
        
        GameObject closeBtnTextObj = new GameObject("Text");
        closeBtnTextObj.transform.SetParent(closeButton.transform);
        TextMeshProUGUI closeBtnText = closeBtnTextObj.AddComponent<TextMeshProUGUI>();
        closeBtnText.text = "OK";
        closeBtnText.fontSize = 36;
        closeBtnText.alignment = TextAlignmentOptions.Center;
        closeBtnText.color = Color.white;
        
        RectTransform closeBtnTextRect = closeBtnTextObj.GetComponent<RectTransform>();
        closeBtnTextRect.anchorMin = Vector2.zero;
        closeBtnTextRect.anchorMax = Vector2.one;
        closeBtnTextRect.sizeDelta = Vector2.zero;
        closeBtnTextRect.anchoredPosition = Vector2.zero;
        
        LevelSelectionUI levelUI = canvasObj.AddComponent<LevelSelectionUI>();
        levelUI.insufficientCoinsPanel = insufficientPanel;
        levelUI.coinsText = coinsText;
        
        closeBtn.onClick.AddListener(() => levelUI.CloseInsufficientCoinsPanel());
        
        insufficientPanel.SetActive(false);
        
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("Level Selection Canvas created successfully!");
    }
}
#endif