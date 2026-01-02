using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Podstawowe ustawienia")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -4.5f);
    public float followSmoothness = 5f;
    public float rotateSmoothness = 3f;

    [Header("Efekt prędkości")]
    public float speedZoomFactor = 0.2f; // ile kamera odjeżdża przy rosnącej prędkości
    public float maxZoomOut = 1.2f;      // mniejsze odjechanie, mniej "łódki"

    [Header("Dynamiczne bujanie")]
    public float bobAmount = 0.05f;      // amplituda bujania góra–dół
    public float bobSpeed = 2.5f;        // szybkość bujania

    [Header("Skrętowy efekt kamery")]
    public float yawTiltAngle = 8f;      // MAKS. przechył przy skręcie (deg) – było 12
    public float sideOffsetAmount = 0.35f; // MAKS. przesunięcie w bok – było 0.6
    public float tiltSmoothness = 5f;    // wygładzanie przechyłu

    [Header("Czułość i progi")]
    public float steerSmoothing = 10f;   // jak szybko wygładzamy wejście skrętu
    public float tiltSpeedThreshold = 0.5f; // poniżej tej prędkości tilt ~ 0 (m/s)
    public float tiltSpeedAtMax = 6f;    // przy tej prędkości tilt jest pełny (m/s ~ 21.6 km/h)

    Rigidbody carRb;
    float steerRaw;
    float steerSmoothed;

    void Start()
    {
        if (target)
            carRb = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // surowy input
        steerRaw = Input.GetAxis("Horizontal");
        // wygładzenie, żeby kamera nie "szarpała"
        steerSmoothed = Mathf.Lerp(steerSmoothed, steerRaw, Time.deltaTime * steerSmoothing);
    }

    void LateUpdate()
    {
        if (!target) return;

        // prędkość planarna (XZ)
        float speed = 0f;
        if (carRb)
            speed = new Vector3(carRb.linearVelocity.x, 0f, carRb.linearVelocity.z).magnitude;

        // normalizacja prędkości do 0..1 (poniżej progu – wygaszamy)
        float speedNorm = Mathf.InverseLerp(tiltSpeedThreshold, tiltSpeedAtMax, speed);
        // łagodne bujanie skalowane prędkością
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount * speedNorm;

        // zoom od prędkości
        float zoomOut = Mathf.Clamp(speed * speedZoomFactor, 0f, maxZoomOut);

        // przechył (roll) i przesunięcie boczne zależne od WYGŁADZONEGO skrętu i prędkości
        float tilt = -steerSmoothed * yawTiltAngle * speedNorm;
        Vector3 sideOffset = Vector3.right * (steerSmoothed * sideOffsetAmount * speedNorm);

        // pozycja kamery
        Vector3 dynamicOffset = offset + new Vector3(0f, bob, -zoomOut);
        Vector3 desiredPos = target.TransformPoint(dynamicOffset + sideOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmoothness);

        // rotacja – patrzymy lekko nad auto, z delikatnym przechyłem
        Quaternion baseRot = Quaternion.LookRotation(target.position + Vector3.up * 0.8f - transform.position, Vector3.up);
        Quaternion tiltRot = baseRot * Quaternion.Euler(0f, 0f, tilt);
        transform.rotation = Quaternion.Slerp(transform.rotation, tiltRot, Time.deltaTime * Mathf.Max(rotateSmoothness, tiltSmoothness));
    }
}
