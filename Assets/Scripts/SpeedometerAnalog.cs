using UnityEngine;
using TMPro;

public class SpeedometerAnalog : MonoBehaviour
{
    [Header("Źródło prędkości")]
    public PeelController_Lite controller;   // przeciągnij PeelRoot ze skryptem

    [Header("Wskazówka")]
    public RectTransform needle;            // przeciągnij obiekt wskazówki (RectTransform)
    [Tooltip("Kąt, gdy prędkość = 0 km/h")]
    public float zeroAngle = -120f;
    [Tooltip("Kąt, gdy prędkość = maxDisplayedKmh")]
    public float maxAngle = 120f;
    [Tooltip("Odwrócenie kierunku obrotu (gdy grafika jest „w drugą stronę”)")]
    public bool invertDirection = false;

    [Header("Skala")]
    [Tooltip("Prędkość odpowiadająca końcowi skali (maxAngle).")]
    public float maxDisplayedKmh = 140f;

    [Header("Cyfra km/h (opcjonalnie)")]
    public TextMeshProUGUI speedText;       // tekst z liczbą
    public bool showUnits = true;           // czy dopisywać „km/h”

    void Reset()
    {
        // spróbuj sam znaleźć referencje
        if (controller == null)
            controller = FindObjectOfType<PeelController_Lite>();

        if (needle == null)
            needle = GetComponent<RectTransform>();

        if (speedText == null)
            speedText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (controller == null || needle == null)
            return;

        // bierzemy prędkość z kontrolera (bez żadnych „magicznych” 73 / 74)
        float kmh = Mathf.Abs(controller.PlanarSpeedKmh);

        // Normalizacja do 0–1 względem maxDisplayedKmh
        float t = 0f;
        if (maxDisplayedKmh > 0f)
            t = Mathf.Clamp01(kmh / maxDisplayedKmh);

        // ewentualne odwrócenie kierunku
        if (invertDirection)
            t = 1f - t;

        // wyliczenie kąta wskazówki
        float angle = Mathf.Lerp(zeroAngle, maxAngle, t);
        needle.localEulerAngles = new Vector3(0f, 0f, angle);

        // aktualizacja tekstu
        if (speedText != null)
        {
            if (showUnits)
                speedText.text = $"{kmh:0} km/h";
            else
                speedText.text = $"{kmh:0}";
        }
    }
}
