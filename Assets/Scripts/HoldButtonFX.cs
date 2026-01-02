using UnityEngine;
using UnityEngine.UI;

public class HoldButtonFX : MonoBehaviour
{
    [Header("Animacja")]
    public float pressedScale = 0.92f;
    public float fadeTo = 0.75f;        // 1=pe³ny, 0=przezroczysty
    public float animIn = 0.08f;        // czas „w dó³”
    public float animOut = 0.12f;       // czas „w górê”

    [Header("Opcjonalnie dŸwiêk")]
    public AudioSource click;           // jednokrotne klikniêcie

    RectTransform rt;
    Graphic g;          // Image lub TMP
    Vector3 startScale;
    float startAlpha;

    void Awake()
    {
        rt = (RectTransform)transform;
        g = GetComponent<Graphic>();
        startScale = rt.localScale;
        startAlpha = g ? g.color.a : 1f;
    }

    public void Press()
    {
        if (click) click.Play();
        StopAllCoroutines();
        StartCoroutine(Lerp(rt.localScale, startScale * pressedScale,
                            GetAlpha(), fadeTo * startAlpha, animIn));
    }

    public void Release()
    {
        StopAllCoroutines();
        StartCoroutine(Lerp(rt.localScale, startScale,
                            GetAlpha(), startAlpha, animOut));
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
