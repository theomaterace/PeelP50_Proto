using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown carDropdown;
    public Button btnStartCar;
    public Button btnVirtualTour;

    [Header("Panele")]
    public GameObject menuHub;          // panel z przyciskami (MenuHub)
    public GameObject webViewRunner;    // obiekt z WenaWebView (WebViewRunner)

    [Header("Sceny aut")]
    public string[] carScenes = { "Menu_PeelP50" };

    void Awake()
    {
        if (btnStartCar) btnStartCar.onClick.AddListener(StartCarFlow);
        if (btnVirtualTour) btnVirtualTour.onClick.AddListener(OpenVirtualTour);

        // startowo: menu widoczne, webview ukryte
        if (menuHub) menuHub.SetActive(true);
        if (webViewRunner) webViewRunner.SetActive(false);
    }

    void StartCarFlow()
    {
        int idx = (carDropdown != null)
            ? Mathf.Clamp(carDropdown.value, 0, Mathf.Max(0, carScenes.Length - 1))
            : 0;

        if (carScenes.Length == 0) return;

        var sceneName = carScenes[idx];
        if (Application.CanStreamedLevelBeLoaded(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogError($"Scena '{sceneName}' nie jest w Build Settings.");
    }

    void OpenVirtualTour()
    {
        // Pokaż WebViewRunner, schowaj menu
        if (menuHub) menuHub.SetActive(false);
        if (webViewRunner) webViewRunner.SetActive(true);
    }
}
