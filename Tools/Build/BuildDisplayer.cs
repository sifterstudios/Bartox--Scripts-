using Bartox.Tools.Build;
using TMPro;
using UnityEngine;

namespace Bartox.Editor
{
    public class BuildDisplayer : MonoBehaviour
    {
        string _buildNumber = "1";
        TMP_Text _text;

        void Awake()
        {
            _text = GetComponent<TMP_Text>();
            var request = Resources.LoadAsync("Build",
                typeof(BuildScriptableObject));
            request.completed += Request_completed;
        }

        void Request_completed(AsyncOperation obj)
        {
            var buildScriptableObject = ((ResourceRequest) obj).asset as BuildScriptableObject;
            if (buildScriptableObject == null)
                Debug.LogError("Build Scriptable Object not found in resources directory. Check build log for errors!");
            else
                _text.SetText($"Build: (Application.version) {buildScriptableObject.BuildNumber}");
        }
    }
}