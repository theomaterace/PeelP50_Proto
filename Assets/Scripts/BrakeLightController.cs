using UnityEngine;

public class BrakeLightController : MonoBehaviour
{
    [Tooltip("Wszystkie renderery świateł stopu (np. lewy i prawy).")]
    public Renderer[] brakeLights;

    [Header("Emisja")]
    public Color brakeColor = Color.red;
    public float emissionOn = 3f;
    public float emissionOff = 0f;
    public float smooth = 12f; // szybkość przejścia

    [Header("Detekcja hamowania")]
    public float decelThreshold = 1.0f; // m/s^2
    public bool keyOverrides = false;   // gdy true, świeć gdy trzymasz hamulec (np. Space)
    public KeyCode brakeKey = KeyCode.Space;

    Rigidbody carRb;
    float lastSpeed;
    float targetIntensity;
    Material[] mats; // własne instancje materiałów

    void Awake()
    {
        carRb = GetComponentInParent<Rigidbody>();
    }

    void Start()
    {
        if (brakeLights == null || brakeLights.Length == 0) return;

        // zrób instancje materiałów, żeby nie zmieniać globalnie
        mats = new Material[brakeLights.Length];
        for (int i = 0; i < brakeLights.Length; i++)
        {
            if (brakeLights[i] != null)
                mats[i] = brakeLights[i].material;
        }

        ApplyEmission(emissionOff); // start przygaszony
    }

    void Update()
    {
        if (carRb == null || mats == null) return;

        bool braking;
        if (keyOverrides)
        {
            braking = Input.GetKey(brakeKey);
        }
        else
        {
            float speed = carRb.linearVelocity.magnitude;
            float decel = (lastSpeed - speed) / Mathf.Max(Time.deltaTime, 0.0001f);
            lastSpeed = speed;
            braking = decel > decelThreshold;
        }

        targetIntensity = braking ? emissionOn : emissionOff;

        // płynna interpolacja
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] == null) continue;
            Color current = mats[i].GetColor("_EmissionColor");
            // bieżąca jasność to długość koloru/skalowanie — uproszczenie:
            Color target = brakeColor * targetIntensity;
            Color lerped = Color.Lerp(current, target, smooth * Time.deltaTime);
            mats[i].SetColor("_EmissionColor", lerped);
        }
    }

    void ApplyEmission(float intensity)
    {
        if (mats == null) return;
        Color c = brakeColor * intensity;
        foreach (var m in mats)
            if (m != null) m.SetColor("_EmissionColor", c);
    }
}
