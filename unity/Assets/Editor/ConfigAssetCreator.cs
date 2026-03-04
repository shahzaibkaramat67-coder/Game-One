using System.IO;
using UnityEditor;
using UnityEngine;

public static class ConfigAssetCreator
{
    [MenuItem("DuelRush/Create Config Asset")]
    public static void CreateConfigAsset()
    {
        const string resourcesDir = "Assets/Resources";
        const string assetPath = resourcesDir + "/Config.asset";

        if (!Directory.Exists(resourcesDir))
        {
            Directory.CreateDirectory(resourcesDir);
            AssetDatabase.Refresh();
        }

        var existing = AssetDatabase.LoadAssetAtPath<Config>(assetPath);
        if (existing != null)
        {
            Selection.activeObject = existing;
            Debug.Log("Config asset already exists: " + assetPath);
            return;
        }

        var config = ScriptableObject.CreateInstance<Config>();
        AssetDatabase.CreateAsset(config, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = config;
        Debug.Log("Created Config asset: " + assetPath);
    }
}

