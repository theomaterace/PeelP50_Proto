using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rt;
    Rect last;

    void Awake() { Cache(); }
    void OnEnable() { Cache(); Apply(); }

    void Cache()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
    }

    void OnRectTransformDimensionsChange()
    {
        Apply();
    }

    public void Apply()
    {
        if (rt == null) { Cache(); if (rt == null) return; }

        Rect safe = Screen.safeArea;
        if (safe == last) return;
        last = safe;

        // zabezpieczenie na wypadek dziwnych rozdzielczoœci
        float sw = Mathf.Max(1f, (float)Screen.width);
        float sh = Mathf.Max(1f, (float)Screen.height);

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;

        min.x /= sw; min.y /= sh;
        max.x /= sw; max.y /= sh;

        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
