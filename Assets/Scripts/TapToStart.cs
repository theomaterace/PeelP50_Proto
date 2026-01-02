using UnityEngine;

public class TapToStart : MonoBehaviour
{
    [Header("Panele")]
    public GameObject tapPanel;   // panel z logo + "Dotknij ekranu..."
    public GameObject mainPanel;  // panel z przyciskami (Wybierz samochód, Wirtualne zwiedzanie)

    bool started = false;

    // Funkcja podpinana pod przycisk
    public void OnTap()
    {
        if (started) return;   // zabezpieczenie przed drugim klikniêciem
        started = true;

        if (tapPanel != null)
            tapPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }
}
