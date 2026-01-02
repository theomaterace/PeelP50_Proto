using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PeelController_Lite : MonoBehaviour
{
    [Header("Parametry docelowe")]
    public float vmaxKmh = 62f;        // prędkość maks. (km/h)
    public float reverseKmh = 15f;     // maks. na wstecznym (km/h)

    [Header("Sterowanie — czułość")]
    public float turnSpeedDeg = 85f;
    [Range(0.1f, 1f)] public float steerAtVmaxFactor = 0.35f;
    public float steerExponent = 1.3f;
    public float steerSmoothing = 6f;
    public float deadzone = 0.05f;     // martwa strefa dla gaz/hamulca

    [Header("Dynamika (m/s^2)")]
    public float accel_fwd_0 = 0.807f;
    public float accel_rev_0 = 0.807f;
    public float decel_free = 2.5f;
    public float decel_brake = 5.5f;

    [Header("Mnożnik globalny prędkości")]
    [Range(0.1f, 1f)] public float speedScale = 1f;

    [Header("Ślizg po bandach (opcjonalnie)")]
    public LayerMask barrierMask;
    public float wallCheckRadius = 0.35f;
    public float wallCheckDistance = 0.8f;
    [Range(0f, 1f)] public float scrapeDamping = 0.6f;

    // ───── Wskaźnik biegu (AUTO, 3-bieg) ─────
    [Header("Biegi (auto-wskaźnik)")]
    public bool autoGears = true;                 // tylko wskaźnik; nie wpływa na fizykę
    [Tooltip("Progi zmiany w górę (km/h): 1→2, 2→3")]
    public float[] upshiftKmh = new float[] { 15f, 30f };
    [Tooltip("Progi zmiany w dół (km/h): 2→1, 3→2")]
    public float[] downshiftKmh = new float[] { 12f, 24f };
    public float shiftMinInterval = 0.6f;
    public float reverseDetectKmh = 1.0f;

    public int CurrentGear { get; private set; } = 1; // 1..3; „R” pokazujesz osobno
    public bool IsReverse { get; private set; } = false;

    [Header("Wsteczny – zasada działania")]
    [Tooltip("Czas przytrzymania STOP w bezruchu, aby wejść w R.")]
    public float reverseHoldSeconds = 0.6f;

    // ───── wewnętrzne ─────
    Rigidbody rb;
    float steerRaw, steerSmoothed, throttle;

    // do kamery / wizualek
    public float SteerVisual => steerSmoothed;

    float currentSpeed; // [m/s] po płaszczyźnie, ze znakiem
    float lastShiftTime = -999f;

    bool brakeHeldNow;        // czy STOP jest trzymany (bieżąca klatka)
    bool gasHeldNow;          // czy GAS jest trzymany (bieżąca klatka)
    bool reverseMode = false; // aktywny tryb wsteczny (po przytrzymaniu STOP)
    float brakeHoldTimer = 0f;
    bool brakingNow = false;  // flaga do mocnego hamowania w FixedUpdate

    float Vmax => (vmaxKmh / 3.6f) * speedScale;
    float Vrev => (reverseKmh / 3.6f) * speedScale;

    public float PlanarSpeedMS => currentSpeed;
    public float PlanarSpeedKmh => currentSpeed * 3.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 pv = rb.linearVelocity;
        currentSpeed = new Vector3(pv.x, 0f, pv.z).magnitude;
    }

    void Update()
    {
        // ── SKRĘT ───────────────────────────────────────────────
        float keyTurn = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        keyTurn = Input.GetAxis("Horizontal");   // PC / Editor
#endif
        float uiTurn = MobileInput.SteerAxis;    // przyciski na ekranie

        float combinedTurn = Mathf.Clamp(keyTurn + uiTurn, -1f, 1f);

        steerRaw = combinedTurn;

        // wygładzanie skrętu
        steerSmoothed = Mathf.MoveTowards(
            steerSmoothed,
            steerRaw,
            steerSmoothing * Time.deltaTime
        );

        // ── GAZ / HAMULEC – odczyt wejść ────────────────────────
        gasHeldNow = false;
        brakeHeldNow = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        float vAxis = Input.GetAxis("Vertical");
        if (vAxis > deadzone) gasHeldNow = true;
        if (vAxis < -deadzone) brakeHeldNow = true;
#endif

        // Mobile przyciski / auto-drive nadpisują
        if (MobileInput.GasHeld) gasHeldNow = true;
        if (MobileInput.BrakeHeld) brakeHeldNow = true;

        // ── Logika R ────────────────────────────────────────────
        float kmhAbs = Mathf.Abs(PlanarSpeedKmh);

        if (gasHeldNow)
        {
            reverseMode = false;
            brakeHoldTimer = 0f;
        }
        else if (brakeHeldNow)
        {
            if (kmhAbs < reverseDetectKmh)   // praktycznie stoi
            {
                brakeHoldTimer += Time.unscaledDeltaTime;
                if (!reverseMode && brakeHoldTimer >= reverseHoldSeconds)
                    reverseMode = true;       // wejdź w R po przytrzymaniu
            }
            else
            {
                // jedziesz → STOP = hamulec
                brakeHoldTimer = 0f;
                reverseMode = false;
            }
        }
        else
        {
            // żaden pedał
            brakeHoldTimer = 0f;
        }

        // ── Throttle / hamowanie ────────────────────────────────
        brakingNow = false;
        throttle = 0f;

        if (gasHeldNow)
        {
            throttle = 1f; // jazda do przodu
        }
        else if (brakeHeldNow)
        {
            if (reverseMode)
            {
                throttle = -1f; // świadomy wsteczny
            }
            else
            {
                throttle = 0f;
                brakingNow = true;
            }
        }

        // wskaźnik biegów (tylko UI)
        if (autoGears) UpdateGearIndicator();
    }

    void FixedUpdate()
    {
        // === 1) SKRĘT z redukcją czułości przy prędkości ===
        float speedAbs = Mathf.Abs(currentSpeed);
        float ratio = (Vmax > 0f) ? Mathf.Clamp01(speedAbs / Vmax) : 0f;
        float steerFactor = Mathf.Lerp(1f, steerAtVmaxFactor, Mathf.Pow(ratio, steerExponent));
        float effectiveTurnDegPerSec = turnSpeedDeg * steerFactor;
        float turn = steerSmoothed * effectiveTurnDegPerSec * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        // === 2) PRĘDKOŚĆ docelowa i narastanie ===
        float targetSpeed;   // [m/s]
        float a;             // [m/s^2]

        if (gasHeldNow && throttle > 0f)
        {
            targetSpeed = Mathf.Lerp(0f, Vmax, throttle);
            float ar = (Vmax > 0f) ? Mathf.Clamp01(Mathf.Abs(currentSpeed) / Vmax) : 0f;
            a = accel_fwd_0 * (1f - ar);
        }
        else if (reverseMode && throttle < 0f)
        {
            targetSpeed = -Mathf.Lerp(0f, Vrev, -throttle);
            float ar = (Vrev > 0f) ? Mathf.Clamp01(Mathf.Abs(currentSpeed) / Vrev) : 0f;
            a = accel_rev_0 * (1f - ar);
        }
        else
        {
            // brak gazu / zwykłe STOP
            targetSpeed = 0f;
            a = brakingNow ? decel_brake : decel_free;
        }

        // ostrzejsze hamowanie przy zmianie kierunku
        if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) &&
            Mathf.Abs(currentSpeed) > 0.05f &&
            (gasHeldNow || reverseMode))
        {
            a = decel_brake;
        }

        float maxDelta = a * Time.fixedDeltaTime;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta);

        // „snap” blisko Vmax – żeby nie pływało na końcu
        if (gasHeldNow && Vmax > 0f && Vmax - Mathf.Abs(currentSpeed) < 0.10f)
            currentSpeed = Mathf.Sign(currentSpeed) * Vmax;

        // === 3) ŚLIZG po bandach (opcjonalnie) ===
        Vector3 planarDir = (currentSpeed >= 0f) ? transform.forward : -transform.forward;
        Vector3 move = planarDir * Mathf.Abs(currentSpeed);

        if (barrierMask.value != 0)
        {
            if (Physics.SphereCast(rb.worldCenterOfMass + Vector3.up * 0.2f,
                                   wallCheckRadius, transform.forward,
                                   out RaycastHit hit, wallCheckDistance,
                                   barrierMask, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(hit.normal.y) < 0.5f && Vector3.Dot(move, hit.normal) > 0f)
                {
                    Vector3 tangential = Vector3.ProjectOnPlane(move, hit.normal);
                    move = tangential * scrapeDamping;
                }
            }
        }

        // === 4) ustaw prędkość RB (zachowaj grawitację) ===
        Vector3 vel = rb.linearVelocity;
        vel.x = move.x;
        vel.z = move.z;
        rb.linearVelocity = vel;
    }

    // ───── Wskaźnik biegu ─────
    void UpdateGearIndicator()
    {
        float kmh = Mathf.Abs(PlanarSpeedKmh);

        // Reverse pokazujemy, gdy aktywny reverseMode lub faktycznie jedziesz wstecz
        IsReverse = reverseMode || (PlanarSpeedKmh < -reverseDetectKmh);

        if (IsReverse)
        {
            CurrentGear = 1; // UI wyświetli „R”
            return;
        }

        if (kmh < 0.5f)
        {
            CurrentGear = 1;
            return;
        }

        if (Time.time - lastShiftTime < shiftMinInterval) return;

        // w górę
        if (CurrentGear == 1 && kmh >= upshiftKmh[0]) { CurrentGear = 2; lastShiftTime = Time.time; return; }
        if (CurrentGear == 2 && kmh >= upshiftKmh[1]) { CurrentGear = 3; lastShiftTime = Time.time; return; }

        // w dół
        if (CurrentGear == 3 && kmh <= downshiftKmh[1]) { CurrentGear = 2; lastShiftTime = Time.time; return; }
        if (CurrentGear == 2 && kmh <= downshiftKmh[0]) { CurrentGear = 1; lastShiftTime = Time.time; return; }
    }

    // reset prędkości poziomej – woła to RaceManager
    public void ResetSpeed()
    {
        currentSpeed = 0f;
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            v.x = 0f; v.z = 0f;
            rb.linearVelocity = v;
        }
        IsReverse = false;
        CurrentGear = 1;
        lastShiftTime = -999f;

        reverseMode = false;
        brakeHoldTimer = 0f;
        brakingNow = false;
    }
}
