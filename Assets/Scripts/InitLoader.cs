using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UIAnimations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitLoader : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private List<string> jsonFileNames;
    [SerializeField] private string assetBundleDirectory = "AssetBundles/Output";

    private void Start()
    {
        StartCoroutine(LoadResources());
    }

    private IEnumerator LoadResources()
    {
        Debug.Log("Загрузка ресурсов началась...");

        yield return LoadJsonFiles();
        yield return LoadAssetBundles();
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
        float progressStep = 20f / jsonFileNames.Count;
        float currentProgress = 0f;

        foreach (var t in jsonFileNames)
        {
            string fileName = $"{t}.json";
            string settingsPath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                Debug.Log($"Файл найден: {fileName}, Содержимое: {json}");
            }
            else
                Debug.LogError($"Файл {fileName} не найден в {Application.streamingAssetsPath}.");

            currentProgress += progressStep;
            progressBar.value = currentProgress / 100f;

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("JSON-файлы загружены.");
    }

    private IEnumerator LoadAssetBundles()
    {
        float currentProgress = 20f;

        string fullPath = Path.Combine(Application.dataPath, assetBundleDirectory);

        if (Directory.Exists(fullPath))
        {
            string[] files = Directory.GetFiles(fullPath);

            if (files.Length > 0)
            {
                Debug.Log($"Найдено {files.Length} Asset Bundle файлов в {fullPath}: ");
                foreach (string file in files)
                
                    Debug.Log($"- {Path.GetFileName(file)}");
            }
            else
                Debug.LogWarning($"Директория {fullPath} существует, но в ней нет файлов.");
        }
        else
            Debug.LogError($"Директория {fullPath} не найдена. Убедитесь, что Asset Bundles созданы.");

        currentProgress += 20f;
        progressBar.value = currentProgress / 100f;

        yield return new WaitForSeconds(1);

        Debug.Log("Asset Bundles загружены.");
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

            simulatedProgress = Mathf.MoveTowards(simulatedProgress, realProgress, Time.deltaTime * 10f);
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
    
    private IEnumerator LoadMainSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = 40f + Mathf.Clamp01(asyncLoad.progress / 0.9f) * 60f;
            progressBar.value = progress / 100f;

            yield return new WaitForSeconds(0.1f);
            
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Основная сцена почти загружена.");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
    }
}
