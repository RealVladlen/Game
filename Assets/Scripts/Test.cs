using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private List<string> jsonFileNames;
    [SerializeField] private string assetBundleDirectory;
    [SerializeField] private string assetBundlePath;
    [SerializeField] private string assetBundleName;
    [SerializeField] private Image testSprite;
    
    private AssetBundle _assetBundle;

    private void Start()
    {
        CheckJsonFile();
        CheckAssetBundle();
        CheckSprite();
    }

    private void CheckSprite()
    {
        _assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        if (_assetBundle == null)
        {
            Debug.LogError("Не удалось загрузить AssetBundle!");
            return;
        }

        testSprite.sprite = _assetBundle.LoadAsset<Sprite>(assetBundleName);
        
        if (testSprite == null)
            Debug.LogError("Не удалось найти спрайт 'buttonbackground' в AssetBundle!");
        else
            Debug.Log("Спрайт успешно загружен!");
    }

    
    private void CheckAssetBundle()
    {
        string fullPath = Path.Combine(Application.dataPath, assetBundleDirectory);

        if (Directory.Exists(fullPath))
        {
            string[] files = Directory.GetFiles(fullPath);

            if (files.Length > 0)
            {
                Debug.Log($"Найдено {files.Length} Asset Bundle файлов в {fullPath}:");
                foreach (string file in files)
                    Debug.Log($"- {Path.GetFileName(file)}");
                
            }
            else
                Debug.LogWarning($"Директория {fullPath} существует, но в ней нет файлов.");
            
        }
        else
            Debug.LogError($"Директория {fullPath} не найдена. Убедитесь, что Asset Bundles созданы.");
    }
    
    private void CheckJsonFile()
    {
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
        }
    }
    
    private void OnDestroy()
    {
        if (_assetBundle != null)
            _assetBundle.Unload(false);
    }
}
