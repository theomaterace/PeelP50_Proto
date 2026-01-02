using UnityEngine;

public class TapToStartController : MonoBehaviour
{
    [Header("Panele")]
    public GameObject tapPanel;    // Panel_TapToStart (logo + napis)
    public GameObject mainMenu;    // Panel_MainMenu (dropdown + przyciski)

    bool started = false;

    // wywo³ywane z przycisku full-screen
    public void OnTap()
    {
        if (started) return;
        started = true;

        if (tapPanel != null)
            tapPanel.SetActive(false);

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }
}
