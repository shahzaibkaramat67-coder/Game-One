using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    [MenuItem("DuelRush/Build APK")]
    public static void BuildAPK()
    {
        // Define build path (output to parent directory)
        string buildPath = "../DuelRush.apk";

        // Define scenes
        string[] scenes = FindEnabledScenes();

        // Build settings
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        // Configure Android settings
        PlayerSettings.bundleIdentifier = "com.duelrush.game";
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;
        PlayerSettings.Android.useCustomKeystore = false;

        // Perform build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("✅ Build succeeded! APK: " + buildPath);
        }
        else if (summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.LogError("❌ Build failed!");
        }
    }

    static string[] FindEnabledScenes()
    {
        System.Collections.Generic.List<string> enabledScenes = new System.Collections.Generic.List<string>();
        
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }

        if (enabledScenes.Count == 0)
        {
            UnityEngine.Debug.LogError("No scenes enabled in Build Settings!");
        }

        return enabledScenes.ToArray();
    }
}
