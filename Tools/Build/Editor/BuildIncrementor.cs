using System;
using Bartox.Tools.Build;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Bartox.Editor
{
    public class BuildIncrementor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();
            switch (report.summary.platform)
            {
                case BuildTarget.StandaloneOSX:
                    PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
                    buildScriptableObject.BuildNumber = PlayerSettings.macOS.buildNumber;
                    break;
                case BuildTarget.iOS:
                    PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
                    buildScriptableObject.BuildNumber = PlayerSettings.iOS.buildNumber;
                    break;
                case BuildTarget.Android:
                    PlayerSettings.Android.bundleVersionCode++;
                    buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                    break;
                case BuildTarget.Switch:
                    PlayerSettings.Switch.displayVersion = IncrementBuildNumber(PlayerSettings.Switch.displayVersion);
                    PlayerSettings.Switch.releaseVersion = IncrementBuildNumber(PlayerSettings.Switch.releaseVersion);
                    buildScriptableObject.BuildNumber = PlayerSettings.Switch.displayVersion;
                    break;
                case BuildTarget.WSAPlayer:
                    PlayerSettings.WSA.packageVersion = new Version(PlayerSettings.WSA.packageVersion.Major,
                        PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);
                    buildScriptableObject.BuildNumber = PlayerSettings.WSA.packageVersion.Build.ToString();
                    break;
                case BuildTarget.StandaloneWindows64:
                    PlayerSettings.WSA.packageVersion = new Version(PlayerSettings.WSA.packageVersion.Major,
                        PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);
                    buildScriptableObject.BuildNumber = PlayerSettings.WSA.packageVersion.Build.ToString();
                    break;
            }

            AssetDatabase.DeleteAsset("Assets/Scripts/Tools/Build/Resources/Build.asset");
            AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Scripts/Tools/Build/Resources/Build.asset");
            AssetDatabase.SaveAssets();
        }

        string IncrementBuildNumber(string buildNumber)
        {
            int.TryParse(buildNumber, out var outputBuildNumber);
            return (outputBuildNumber + 1).ToString();
        }
    }
}