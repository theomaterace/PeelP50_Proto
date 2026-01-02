using UnityEngine;

public class NSU_AutoDrive : MonoBehaviour
{
    [Header("Odniesienia")]
    public PeelController_Lite controller; // kontroler auta
    public RaceManager raceManager;        // licznik czasu / start-meta

    [Header("Parametry NSU Prinz 1000")]
    [Tooltip("Prędkość maksymalna katalogowo ~131–135 km/h.")]
    public float nsuVmaxKmh = 135f;

    [Tooltip("Początkowe przyspieszenie (m/s²), policzone z 0–100 km/h ≈ 20 s.")]
    public float accelFwd0 = 2.5f;

    [Tooltip("Naturalne wytracanie prędkości bez hamulca.")]
    public float decelFree = 1.2f;

    [Tooltip("Hamowanie przy wciśniętym STOP / mocnym skręcie.")]
    public float decelBrake = 4.5f;

    [Header("Zachowanie w zakręcie")]
    [Tooltip("Do tej wartości skrętu: pełen gaz.")]
    [Range(0f, 1f)] public float gentleTurn = 0.15f;

    [Tooltip("Powyżej tej wartości skrętu: hamowanie.")]
    [Range(0f, 1f)] public float brakeTurn = 0.6f;

    void Reset()
    {
        controller = GetComponent<PeelController_Lite>();
        if (raceManager == null)
            raceManager = FindObjectOfType<RaceManager>();
    }

    void Start()
    {
        if (controller == null)
            controller = GetComponent<PeelController_Lite>();

        if (controller != null)
        {
            controller.vmaxKmh = nsuVmaxKmh;
            controller.accel_fwd_0 = accelFwd0;
            controller.decel_free = decelFree;
            controller.decel_brake = decelBrake;
        }

        // Na starcie nie wciskamy pedałów – RaceManager ustawi start
        MobileInput.GasHeld = false;
        MobileInput.BrakeHeld = false;
    }

    void Update()
    {
        if (controller == null)
            return;

        // podczas resetu / countdownu nic nie robimy
        if (raceManager != null && raceManager.IsResetting)
        {
            MobileInput.GasHeld = false;
            MobileInput.BrakeHeld = false;
            return;
        }

        // sterowanie kierowcy (A/D + przyciski ekranowe)
        float steer = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        steer += Input.GetAxis("Horizontal");
#endif
        steer += MobileInput.SteerAxis;
        steer = Mathf.Clamp(steer, -1f, 1f);

        float steerAbs = Mathf.Abs(steer);

        // 1) Prosto / lekki łuk → pełen gaz
        if (steerAbs < gentleTurn)
        {
            MobileInput.GasHeld = true;
            MobileInput.BrakeHeld = false;
        }
        // 2) Ostry zakręt → hamulec
        else if (steerAbs > brakeTurn)
        {
            MobileInput.GasHeld = false;
            MobileInput.BrakeHeld = true;
        }
        // 3) Średni zakręt → puszczony gaz, bez hamulca
        else
        {
            MobileInput.GasHeld = false;
            MobileInput.BrakeHeld = false;
        }
    }
}
