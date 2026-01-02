using UnityEngine;
using System.Collections;

public class WenaWebViewController : MonoBehaviour
{
    public string url = "https://wena-360.netlify.app";
    private WebViewObject webView;

    IEnumerator Start()
    {
        // 🔹 Poczekaj jedną klatkę, żeby Android poprawnie zainicjował WebView
        yield return new WaitForEndOfFrame();

        webView = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webView.Init(
            cb: msg => Debug.Log($"CallFromJS: {msg}"),
            err: msg => Debug.LogError($"WebView error: {msg}"),
            started: msg => Debug.Log($"Started: {msg}"),
            hooked: msg => Debug.Log($"Hooked: {msg}")
        );

        webView.SetMargins(0, 0, 0, 80);  // miejsce na przycisk Wstecz
        webView.SetVisibility(true);
        webView.LoadURL(url);
    }

    public void GoBack()
    {
        if (webView != null)
        {
            if (webView.CanGoBack())
                webView.GoBack();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomeMenu");
        }
    }
}
