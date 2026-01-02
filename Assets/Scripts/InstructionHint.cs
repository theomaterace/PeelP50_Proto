using UnityEngine;
using TMPro;

public class InstructionHint : MonoBehaviour
{
    [Header("Czas")]
    [Tooltip("Ile sekund napis ma byæ w pe³ni widoczny zanim zacznie znikaæ.")]
    public float showSeconds = 5f;
    [Tooltip("Czas wygaszania (sekundy).")]
    public float fadeSeconds = 1.5f;

    [Header("Znikanie po pierwszym ruchu")]
    [Tooltip("Jeœli true: napis zacznie znikaæ od razu po pierwszym drag'u.")]
    public bool hideOnFirstDrag = true;
    [Tooltip("Minimalny ruch (w pikselach), ¿eby uznaæ go za 'drag'.")]
    public float dragThreshold = 2f;

    CanvasGroup cg;
    TextMeshProUGUI tmp;
    float timer;
    bool fading;
    Vector2 lastPos;
    bool hasLast;

    void Awake()
    {
        // Upewnij siê, ¿e jest CanvasGroup
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();

        // TMP tylko po to, by mieæ pewnoœæ ¿e kolor/alpha s¹ spójne
        tmp = GetComponent<TextMeshProUGUI>();

        // Napis startuje widoczny
        cg.alpha = 1f;

        // Ten napis nie powinien przejmowaæ klików
        var raycastTargetSetter = GetComponent<TMPro.TextMeshProUGUI>();
        if (raycastTargetSetter) raycastTargetSetter.raycastTarget = false;

        timer = showSeconds;
        fading = false;
        hasLast = false;
    }

    void Update()
    {
        // Opcja: schowaj od razu po pierwszym realnym ruchu palcem/mysz¹
        if (!fading && hideOnFirstDrag && IsDragging())
            fading = true;

        // Jeœli nie schowaliœmy po drag'u, czekamy na up³yniêcie czasu
        if (!fading)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0f)
                fading = true;
        }

        // Wygaszanie
        if (fading && cg.alpha > 0f)
        {
            cg.alpha = Mathf.Max(0f, cg.alpha - (Time.unscaledDeltaTime / fadeSeconds));
        }
    }

    bool IsDragging()
    {
        // DOTYK
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                lastPos = t.position; hasLast = true;
            }
            else if (t.phase == TouchPhase.Moved && hasLast)
            {
                if ((t.position - lastPos).sqrMagnitude >= dragThreshold * dragThreshold)
                    return true;
            }
        }
        else
        {
            // MYSZ (LPM przytrzymany + ruch)
            if (Input.GetMouseButton(0))
            {
                var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                if (delta.sqrMagnitude >= (dragThreshold / 1000f)) // delikatny próg dla myszy
                    return true;
            }
        }
        return false;
    }
}
