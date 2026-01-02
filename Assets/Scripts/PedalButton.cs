using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PedalButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Type { Gas, Brake }
    public Type type;

    [Header("Efekt wizualny")]
    public float pressedScale = 0.92f;
    public float fadeTo = 0.75f;     // 1 = bez przyciemnienia
    public float animIn = 0.08f;
    public float animOut = 0.12f;
    public AudioSource click;        // opcjonalnie

    RectTransform rt;
    Graphic g;
    Vector3 startScale;
    float startAlpha;

    void Awake()
    {
        rt = (RectTransform)transform;
        g = GetComponent<Graphic>(); // Image lub TMP na tym samym obiekcie
        startScale = rt.localScale;
        startAlpha = g ? g.color.a : 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (type == Type.Gas) MobileInput.GasHeld = true;
        else MobileInput.BrakeHeld = true;

        if (click) click.Play();
        StopAllCoroutines();
        StartCoroutine(Lerp(startScale, startScale * pressedScale, GetAlpha(), fadeTo * startAlpha, animIn));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // palec wyszedł poza ikonę → traktuj jak puszczenie
        Release();
    }

    void Release()
    {
        if (type == Type.Gas) MobileInput.GasHeld = false;
        else MobileInput.BrakeHeld = false;

        StopAllCoroutines();
        StartCoroutine(Lerp(rt.localScale, startScale, GetAlpha(), startAlpha, animOut));
    }

    System.Collections.IEnumerator Lerp(Vector3 sA, Vector3 eA, float sAl, float eAl, float tDur)
    {
        float t = 0f;
        while (t < tDur)
        {
            t += Time.unscaledDeltaTime;
            float k = tDur > 0f ? t / tDur : 1f;
            rt.localScale = Vector3.Lerp(sA, eA, k);
            if (g) { var c = g.color; c.a = Mathf.Lerp(sAl, eAl, k); g.color = c; }
            yield return null;
        }
        rt.localScale = eA;
        if (g) { var c = g.color; c.a = eAl; g.color = c; }
    }

    float GetAlpha() => g ? g.color.a : 1f;
}
