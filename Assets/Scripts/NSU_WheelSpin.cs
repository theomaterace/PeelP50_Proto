using UnityEngine;

public class NSU_WheelSpin : MonoBehaviour
{
    [Header("Odniesienia")]
    public PeelController_Lite controller;   // tu przeciągnij PeelRoot (ze skryptem)

    [Tooltip("Same meshe kół – po prostu Transformy.")]
    public Transform[] wheels;               // kolejno: LF, RF, LR, RR (jak wolisz)

    [Header("Parametry geometryczne")]
    [Tooltip("Promień koła w metrach (NSU ma ~13\" felgę, więc coś koło 0.29 m).")]
    public float wheelRadius = 0.29f;

    [Tooltip("Jeśli koła kręcą się \"do tyłu\" – zaznacz to.")]
    public bool invertRotation = false;

    void Reset()
    {
        if (controller == null)
            controller = GetComponent<PeelController_Lite>();
    }

    void Update()
    {
        if (controller == null || wheels == null || wheels.Length == 0)
            return;

        // prędkość liniowa auta [m/s]
        float v = Mathf.Abs(controller.PlanarSpeedMS);

        // omega = v / R [rad/s] → * Mathf.Rad2Deg żeby mieć stopnie na sekundę
        float omegaDeg = (wheelRadius > 0.001f)
            ? (v / wheelRadius) * Mathf.Rad2Deg
            : 0f;

        float delta = omegaDeg * Time.deltaTime;
        if (invertRotation) delta = -delta;

        // zakładam, że oś obrotu koła to lokalne X
        foreach (var w in wheels)
        {
            if (w == null) continue;
            w.Rotate(Vector3.right, delta, Space.Self);
        }
    }
}
