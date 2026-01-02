using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StartFinishTrigger : MonoBehaviour
{
    public RaceManager manager;
    [Tooltip("W kt�r� stron� 'do przodu' przecinamy lini�. U�yje forward tego obiektu, je�li DirectionRef nie podane.")]
    public Transform directionRef;

    [Header("Filtry antyb��dne")]
    public float minSpeed = 0.5f;      // minimalna pr�dko��, by uzna� przeci�cie
    public float minInterval = 0.5f;   // odst�p czasowy mi�dzy zliczeniami (debounce)
    public float minForwardDot = 0.25f;// zgodno�� kierunku (0..1): 1 = idealnie do przodu

    float lastCrossTime = -999f;

    void Awake()
    {
        if (manager == null) manager = FindObjectOfType<RaceManager>();
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (manager == null) return;
        if (!other.CompareTag("Player")) return;
        if (manager.IsResetting) return;

        // Debounce czasowy
        if (Time.time - lastCrossTime < minInterval) return;

        // Pr�dko�� i kierunek auta
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector3 v = rb.linearVelocity; v.y = 0f;
        float speed = v.magnitude;
        if (speed < minSpeed) return;

        // Kierunek "w�a�ciwy" (z obiektu lub z tego triggera)
        Vector3 fwd = directionRef ? directionRef.forward : transform.forward;
        fwd.y = 0f; fwd.Normalize();

        Vector3 velDir = v.normalized;
        float dot = Vector3.Dot(fwd, velDir);
        if (dot < minForwardDot) return; // z�a strona przeci�cia

        // START je�li nie by�o startu; META je�li wy�cig ju� trwa
        if (!manager.RaceActive)
            manager.StartRace();
        else
            manager.FinishRace();

        lastCrossTime = Time.time;
    }
}
