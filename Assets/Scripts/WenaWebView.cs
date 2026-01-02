using System.Collections;
using UnityEngine;

public class WenaWebView : MonoBehaviour
{
    [Header("Ustawienia")]
    [Tooltip("URL wirtualnego zwiedzania")]
    public string url = "https://wena-360.netlify.app";

    [Tooltip("Canvas główny (do przeliczenia pikseli)")]
    public Canvas rootCanvas;

    [Tooltip("Pasek u góry z przyciskiem WSTECZ (RectTransform paska)")]
    public RectTransform topBar; // ten, który rozciągnęliśmy na pełną szerokość

    [Tooltip("Panel z menu głównym do pokazania po zamknięciu webview")]
    public GameObject menuHub; // np. Twój obiekt "MenuHub"

    WebViewObject web;
    bool isOpen;
    bool isClosing;

    void Start()
    {
        // Nic nie otwieramy automatycznie – przycisk w menu wywoła Open()
        HideTopBar(false); // na starcie pasek można ukryć (opcjonalnie)
    }

    void Update()
    {
        // Android "wstecz"
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackPressed();
        }
    }

    // === API dla przycisków ===
    public void Open()
    {
        if (isOpen) return;
        StartCoroutine(OpenRoutine());
    }

    public void OnBackPressed()
    {
        if (!isOpen || web == null) return;

        // jeśli strona ma historię – cofaj w niej; jeśli nie – zamknij webview
        if (web.CanGoBack())
            web.GoBack();
        else
            Close();
    }

    public void Close()
    {
        if (!isOpen || isClosing) return;
        StartCoroutine(CloseRoutine());
    }

    // === Rzeczy wewnętrzne ===
    IEnumerator OpenRoutine()
    {
        isClosing = false;

        // Schowaj menu na czas webview (jeśli masz je w tej samej scenie)
        if (menuHub != null) menuHub.SetActive(false);

        // Pokaż pasek u góry
        HideTopBar(true);

        // Tworzymy WebView
        web = new GameObject("WebView").AddComponent<WebViewObject>();
        web.Init(
            cb: (msg) => { /* debug js->unity */ },
            err: (msg) => { Debug.LogWarning("WebView error: " + msg); },
            started: (msg) => { /* strona zaczęła się ładować */ },
            hooked: (msg) => { /* link hook */ },
            ld: (msg) => { /* załadowano */ }
        );

        // Oblicz top margin: SafeArea + wysokość paska
        int topMarginPx = CalcTopMarginPixels();

        // Zostaw górny margines na pasek, reszta pełny ekran
        web.SetMargins(0, topMarginPx, 0, 0);
        web.SetVisibility(true);

        // Uwaga: na Androidzie używaj http/https w pełnym URL
        web.LoadURL(url);

        isOpen = true;
        yield break;
    }

    IEnumerator CloseRoutine()
    {
        isClosing = true;

        // Schowaj webview natychmiast
        if (web != null)
        {
            web.SetVisibility(false);
            Destroy(web.gameObject);
            web = null;
        }

        isOpen = false;

        // Daj jedną klatkę, żeby natywne okno się domknęło (eliminuje „biały ekran”)
        yield return null;

        // Schowaj pasek
        HideTopBar(false);

        // Przywróć menu
        if (menuHub != null) menuHub.SetActive(true);

        isClosing = false;
    }

    int CalcTopMarginPixels()
    {
        // bez CanvasScaler: scaleFactor ~ 1
        float scale = (rootCanvas != null) ? rootCanvas.scaleFactor : 1f;

        // wysokość paska w pikselach
        int barPx = 0;
        if (topBar != null)
            barPx = Mathf.RoundToInt(topBar.rect.height * scale);

        // uwzględnij notche / status bar
        var safe = Screen.safeArea;
        int safeTopPx = Mathf.RoundToInt(Screen.height - safe.yMax);

        return safeTopPx + barPx;
    }

    void HideTopBar(bool show)
    {
        if (topBar != null)
            topBar.gameObject.SetActive(show);
    }
}
