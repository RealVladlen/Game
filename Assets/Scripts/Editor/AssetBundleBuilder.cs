using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Tools/Build Asset Bundles")]
    public static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles/Output";
    
        if (!System.IO.Directory.Exists(assetBundleDirectory))
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
    
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Debug.Log("AssetBundles создан.");
    }
}


