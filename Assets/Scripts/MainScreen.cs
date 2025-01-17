using System.Collections;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    public Button incrementButton;
    public Button updateContentButton;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI welcomeMessageText;
    public Image buttonBackground;

    private int _counter;
    private string _settingsPath;
    private string _messagePath;
    private AssetBundle _assetBundle;
    private Transform _transformCountText;
    private Tween _countTextAnimation;
    
    void Start()
    {
        Init();

        _transformCountText = counterText.transform;
        
        incrementButton.onClick.AddListener(IncrementCounter);
        updateContentButton.onClick.AddListener(UpdateContent);
    }

    private void Init()
    {
        _settingsPath = InitLoader.Instance.GetSettingsJson();
        _messagePath = InitLoader.Instance.GetMessageJson();
        _assetBundle = InitLoader.Instance.GetAssetBundle();
        
        LoadCounter();
        LoadWelcomeMessage();

        if (_assetBundle != null)
        {
            Sprite backgroundSprite = _assetBundle.LoadAsset<Sprite>("Button");
            buttonBackground.sprite = backgroundSprite;
        }
    }
    
    private void LoadCounter()
    {
        string counterPath = Path.Combine(Application.persistentDataPath, "counter.json");
        string json = "";
        
        if (File.Exists(counterPath))
        {
            json = File.ReadAllText(counterPath);
            _counter = JsonUtility.FromJson<CounterData>(json).value;
        }
        else
        {
            _counter = JsonUtility.FromJson<SettingsData>(_settingsPath).startingNumber;
        }

        UpdateCounterUI();
    }

    private void IncrementCounter()
    {
        _counter++;
        SaveCounter();
        UpdateCounterUI();
        ScaleCountText();
    }

    private void SaveCounter()
    {
        string counterPath = Path.Combine(Application.persistentDataPath, "counter.json");
        CounterData data = new CounterData { value = _counter };
        File.WriteAllText(counterPath, JsonUtility.ToJson(data));
    }

    private void UpdateCounterUI()
    {
        counterText.text = _counter.ToString();
    }

    private void ScaleCountText()
    {
        _countTextAnimation = _transformCountText.DOScale(1.5f, 0.15f).OnComplete(() =>
        {
            _countTextAnimation = _transformCountText.DOScale(1f, 0.15f);
        });
    }
    
    private void LoadWelcomeMessage()
    {
        WelcomeMessageData messageData = JsonUtility.FromJson<WelcomeMessageData>(_messagePath);
        welcomeMessageText.text = messageData.message;
    }

    private void UpdateContent()
    {
        StartCoroutine(UpdateContentCoroutine());
    }
    
    private IEnumerator UpdateContentCoroutine()
    {
        yield return InitLoader.Instance.UpdateResources();
        
        _settingsPath = InitLoader.Instance.GetSettingsJson();
        _messagePath = InitLoader.Instance.GetMessageJson();
        _assetBundle = InitLoader.Instance.GetAssetBundle();

        string filePath = Path.Combine(Application.persistentDataPath, "counter.json");
        
        if (File.Exists(filePath))
            File.Delete(filePath);
        
        if (_assetBundle != null)
        {
            Sprite backgroundSprite = _assetBundle.LoadAsset<Sprite>("Button");
            buttonBackground.sprite = backgroundSprite;
        }
        
        LoadCounter();
        LoadWelcomeMessage();
    }

    private void OnDestroy()
    {
        _countTextAnimation?.Kill();
    }

    [System.Serializable]
    public class CounterData
    {
        public int value;
    }

    [System.Serializable]
    public class SettingsData
    {
        public int startingNumber;
    }

    [System.Serializable]
    public class WelcomeMessageData
    {
        public string message;
    }
}