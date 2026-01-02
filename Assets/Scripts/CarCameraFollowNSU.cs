using UnityEngine;

public class CarCameraFollowNSU : MonoBehaviour
{
    [Header("Obiekt do œledzenia")]
    public Transform target;                // np. PeelRoot / NSU root
    public PeelController_Lite controller;  // ten sam obiekt co „target”

    [Header("Pozycja kamery")]
    public Vector3 localOffset = new Vector3(0f, 2.0f, -6.0f);
    public float followLerp = 6f;          // jak szybko kamera dogania

    [Header("Pochylenie jak w NFS")]
    public float maxRollAngle = 12f;       // przechy³ na boki (Z)
    public float maxPitchAngle = 4f;       // lekkie „wgryzanie siê” w zakrêt (X)
    public float minSpeedForTiltKmh = 10f; // od jakiej prêdkoœci zaczyna siê efekt

    public float tiltLerp = 8f;            // jak szybko kamera reaguje na skrêt

    void Reset()
    {
        // Spróbuj automatycznie znaleŸæ target i kontroler
        if (target == null)
        {
            var ctrl = FindObjectOfType<PeelController_Lite>();
            if (ctrl != null)
            {
                controller = ctrl;
                target = ctrl.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1) Œledzenie pozycji za autem
        Vector3 desiredPos = target.TransformPoint(localOffset);
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followLerp * Time.deltaTime
        );

        // 2) Bazowa rotacja: patrzymy na auto
        Vector3 toTarget = target.position - transform.position;
        if (toTarget.sqrMagnitude < 0.001f) return;

        Quaternion lookRot = Quaternion.LookRotation(toTarget, Vector3.up);

        // 3) Obliczenie pochylenia na podstawie skrêtu i prêdkoœci
        float steer = (controller != null) ? controller.SteerVisual : 0f;
        float speedKmh = (controller != null) ? controller.PlanarSpeedKmh : 0f;

        // 0–1, jak bardzo zbli¿amy siê do maksymalnego efektu
        float speed01 = 0f;
        if (controller != null && controller.vmaxKmh > 0f)
        {
            float s = Mathf.Abs(speedKmh);
            speed01 = Mathf.InverseLerp(minSpeedForTiltKmh, controller.vmaxKmh, s);
        }

        // roll: przechy³ na boki (Z), im wiêkszy skrêt tym wiêkszy przechy³
        float roll = -steer * maxRollAngle * speed01;

        // pitch: delikatne „pochylenie do œrodka zakrêtu” (X)
        float pitch = -Mathf.Abs(steer) * maxPitchAngle * speed01;

        Quaternion tiltRot = lookRot * Quaternion.Euler(pitch, 0f, roll);

        // 4) Wyg³adzenie rotacji kamery
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            tiltRot,
            tiltLerp * Time.deltaTime
        );
    }
}
