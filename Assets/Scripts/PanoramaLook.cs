using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplePanoramaLook : MonoBehaviour
{
    [Header("Czułość")]
    [Tooltip("Czułość w poziomie/pionie dla dotyku (piksel → stopnie).")]
    public float touchSensitivity = 0.10f;   // było 0.30f

    [Tooltip("Czułość dla myszy (stopnie na sekundę).")]
    public float mouseSensitivity = 80.0f;   // było 120

    [Header("Wygładzanie")]
    [Tooltip("Czas wygładzania ruchu (sekundy). 0.08–0.15 jest ok.")]
    public float smoothTime = 0.15f;         // było 0.10f

    [Header("Zakres")]
    [Tooltip("Maksymalne odchylenie kamery w górę/dół.")]
    public float pitchClamp = 80f;           // było 89

    [Header("Nawigacja")]
    [Tooltip("Nazwa sceny, do której ma wracać przycisk 'Wstecz'.")]
    public string backSceneName = ""; // np. "Menu_PeelP50"

    [Header("Żyroskop (opcjonalnie)")]
    public bool useGyro = false;
    public float gyroSensitivity = 1.0f;
    private bool gyroAvail;
    private Quaternion baseGyro;

    // wewnętrzne
    float targetYaw, targetPitch;
    float currYaw, currPitch;
    float velYaw, velPitch;

    // minimalny ruch palca, żeby w ogóle reagować (piksele / klatkę)
    const float dragMin = 0.8f;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        var e = transform.localEulerAngles;
        targetYaw = currYaw = e.y;
        targetPitch = currPitch = e.x;

        gyroAvail = SystemInfo.supportsGyroscope;
        if (useGyro && gyroAvail)
        {
            Input.gyro.enabled = true;
            baseGyro = Input.gyro.attitude;
        }
    }

    void Update()
    {
        // Android „Wstecz”
        if (Input.GetKeyDown(KeyCode.Escape))
            ReturnToMenu();

        if (useGyro && gyroAvail && Input.gyro.enabled)
            ReadGyro();
        else
            ReadPointer();

        // ogranicz odchylenie
        targetPitch = Mathf.Clamp(targetPitch, -pitchClamp, pitchClamp);

        // wygładzanie kątów
        currYaw = Mathf.SmoothDampAngle(currYaw, targetYaw, ref velYaw, smoothTime);
        currPitch = Mathf.SmoothDampAngle(currPitch, targetPitch, ref velPitch, smoothTime);

        transform.localRotation = Quaternion.Euler(currPitch, currYaw, 0f);
    }

    void ReadPointer()
    {
        // DOTYK
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                // ignoruj bardzo małe ruchy palca
                if (Mathf.Abs(t.deltaPosition.x) < dragMin &&
                    Mathf.Abs(t.deltaPosition.y) < dragMin)
                    return;

                targetYaw += t.deltaPosition.x * touchSensitivity;
                targetPitch -= t.deltaPosition.y * touchSensitivity;
            }
            return;
        }

        // MYSZ (PC) – przytrzymaj LPM
        if (Input.GetMouseButton(0))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            targetYaw += mx * mouseSensitivity * Time.deltaTime;
            targetPitch -= my * mouseSensitivity * Time.deltaTime;
        }
    }

    void ReadGyro()
    {
        var q = Input.gyro.attitude;
        var unity = new Quaternion(q.x, q.y, -q.z, -q.w);
        var rel = Quaternion.Inverse(baseGyro) * unity;
        var e = rel.eulerAngles;

        float Norm(float a) => (a > 180f) ? a - 360f : a;

        targetYaw = Norm(e.y) * gyroSensitivity;
        targetPitch = -Norm(e.x) * gyroSensitivity;
    }

    public void ReturnToMenu()
    {
        if (!string.IsNullOrEmpty(backSceneName))
            SceneManager.LoadScene(backSceneName);
    }

    public void CalibrateGyro()
    {
        if (gyroAvail)
            baseGyro = Input.gyro.attitude;
    }
}
