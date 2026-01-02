using UnityEngine;
using TMPro;

public class GearIndicatorUI : MonoBehaviour
{
    public PeelController_Lite controller;   // przeci¹gnij PeelRoot (ze skryptem)
    public TextMeshProUGUI gearText;         // przeci¹gnij TMP na HUD-zie

    void Reset()
    {
        // spróbuj automatycznie znaleŸæ TextMeshPro na tym samym obiekcie
        gearText = GetComponent<TextMeshProUGUI>();

        // NOWY sposób szukania kontrolera w scenie (Unity 6)
        if (controller == null)
        {
            controller = FindFirstObjectByType<PeelController_Lite>();
        }
    }

    void Update()
    {
        if (controller == null || gearText == null) return;

        if (controller.IsReverse)
        {
            gearText.text = "R";
        }
        else
        {
            // 1..3
            gearText.text = controller.CurrentGear.ToString();
        }
    }
}
