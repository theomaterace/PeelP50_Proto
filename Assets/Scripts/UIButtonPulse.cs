using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float hoverScale = 1.05f;
    public float pressScale = 0.98f;
    public float lerpSpeed = 12f;
    public AudioSource clickAudio; // opcjonalnie
    public AudioClip clickClip;

    Vector3 baseScale;
    Vector3 targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData e) { targetScale = baseScale * hoverScale; }
    public void OnPointerExit(PointerEventData e) { targetScale = baseScale; }
    public void OnPointerDown(PointerEventData e) { targetScale = baseScale * pressScale; }
    public void OnPointerUp(PointerEventData e)
    {
        targetScale = baseScale * hoverScale;
        if (clickAudio && clickClip) clickAudio.PlayOneShot(clickClip);
    }
}
