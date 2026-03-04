using UnityEngine;
using UnityEngine.UI;

public class EnvironmentSwitcher : MonoBehaviour
{
    [SerializeField] private Dropdown environmentDropdown;
    [SerializeField] private Text statusText;

    void Start()
    {
        if (environmentDropdown != null)
        {
            environmentDropdown.ClearOptions();
            environmentDropdown.AddOptions(new System.Collections.Generic.List<string> { "development", "staging", "production" });
            
            string current = ConfigLoader.Instance.GetCurrentEnvironment();
            environmentDropdown.value = environmentDropdown.options.FindIndex(o => o.text == current);
            
            environmentDropdown.onValueChanged.AddListener(OnEnvironmentChanged);
        }

        UpdateStatus();
    }

    void OnEnvironmentChanged(int index)
    {
        string selected = environmentDropdown.options[index].text;
        ConfigLoader.Instance.SetEnvironment(selected);
        UpdateStatus();
        Debug.Log($"Environment changed to {selected}. Restart app to apply fully.");
    }

    void UpdateStatus()
    {
        if (statusText != null)
        {
            string env = ConfigLoader.Instance.GetCurrentEnvironment();
            string url = Config.Instance.wsServerUrl;
            statusText.text = $"Environment: {env}\nServer: {url}";
        }
    }
}
