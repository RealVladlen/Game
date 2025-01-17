using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UIAnimations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitLoader : MonoBehaviour
{
    public static InitLoader Instance;
    
    [SerializeField] private Slider progressBar;

    private string _settingsJson;
    private string _messageJson;
    private AssetBundle _assetBundle;

    public string GetSettingsJson() => _settingsJson;
    public string GetMessageJson() => _messageJson;
    public AssetBundle GetAssetBundle() => _assetBundle;
    
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        StartCoroutine(LoadResources());
    }

    public IEnumerator UpdateResources()
    {
        Debug.Log("Загрузка ресурсов началась...");

        yield return LoadJsonFiles();
        yield return LoadAssetBundles();

        Debug.Log("Все ресурсы обновлены.");
    }
    
    private IEnumerator LoadResources()
    {
        Debug.Log("Загрузка ресурсов началась...");

        yield return new WaitForSeconds(0.5f);
        yield return LoadJsonFiles();
        yield return new WaitForSeconds(1f);
        yield return LoadAssetBundles();
        yield return new WaitForSeconds(1f);
        yield return LoadMainSceneAdditiveAsync();

        Debug.Log("Все ресурсы загружены. Переход на основную сцену завершён.");
        
        progressBar.GetComponent<CanvasGroup>().DOFade(0,1f).OnComplete(()=>
        {
            progressBar.gameObject.SetActive(false);
            Fader.Instance.HideFade(1.5f);
        }) ;
    }

    private IEnumerator LoadJsonFiles()
    {
        _settingsJson = CheckJsonFile("Settings.json");
        progressBar.value = 0.1f;
        
        _messageJson = CheckJsonFile("WelcomeMessage.json");
        progressBar.value = 0.2f;

        Debug.Log("JSON-файлы загружены.");
        yield return null;
    }

    private string CheckJsonFile(string nameFile)
    {
        string path = Path.Combine(Application.streamingAssetsPath, nameFile);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"Файл найден: {nameFile}, Содержимое: {json}");
            
            return json;
        }

        Debug.LogError($"Файл {nameFile} не найден в {Application.streamingAssetsPath}.");
        return null;
    }
    
    private IEnumerator LoadAssetBundles()
    {
        float currentProgress = 20f;
        
        if (_assetBundle != null)
            _assetBundle.Unload(false);
        
        _assetBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/Output/buttonbundle");
        
        currentProgress += 20f;
        progressBar.value = currentProgress / 100f;

        if (_assetBundle != null)
            Debug.Log("Asset Bundles загружены.");
        else
            Debug.LogError("Не удалось загрузить AssetBundle!");
        yield return null;
    }
    
    private IEnumerator LoadMainSceneAdditiveAsync()
    {
        string mainSceneName = "MainScene";
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainSceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        float simulatedProgress = 40f;
        while (!asyncLoad.isDone)
        {
            float realProgress = 40f + Mathf.Clamp01(asyncLoad.progress / 0.9f) * 60f;

            simulatedProgress = Mathf.MoveTowards(simulatedProgress, realProgress, Time.deltaTime * UnityEngine.Random.Range(0.1f,20f));
            progressBar.value = simulatedProgress / 100f;

            if (asyncLoad.progress >= 0.9f && simulatedProgress >= 100f)
            {
                Debug.Log("Основная сцена почти загружена.");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Основная сцена загружена в режиме Additive.");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainSceneName));
    }

    private void OnDestroy()
    {
        if (_assetBundle != null)
            _assetBundle.Unload(false);
    }
}
