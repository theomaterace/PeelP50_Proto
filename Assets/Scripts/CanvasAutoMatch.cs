using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasAutoMatch : MonoBehaviour
{
    [Range(0f, 1f)] public float wideMatch = 0.35f; // szerokie ekrany (tablety)
    [Range(0f, 1f)] public float tallMatch = 0.65f; // bardzo wysokie ekrany (telefony 20:9)
    public float pivotAspect = 16f / 9f;

    void Start()
    {
        var scaler = GetComponent<CanvasScaler>();
        float aspect = (float)Screen.width / Screen.height;
        scaler.matchWidthOrHeight = (aspect >= pivotAspect) ? wideMatch : tallMatch;
    }
}
