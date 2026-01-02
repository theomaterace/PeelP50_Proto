using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SteerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Side { Left, Right }

    [Header("Która strona?")]
    public Side side = Side.Left;

    [Header("Prosty efekt wciœniêcia")]
    public float pressedScale = 0.92f;

    Vector3 startScale;

    void Awake()
    {
        startScale = transform.localScale;
    }

    float Dir => (side == Side.Left) ? -1f : 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInput.SteerAxis = Dir;
        transform.localScale = startScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // jeœli ten przycisk by³ aktywny – puszczamy skrêt
        if (Mathf.Approximately(MobileInput.SteerAxis, Dir))
            MobileInput.SteerAxis = 0f;

        transform.localScale = startScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerUp(eventData);
    }
}
