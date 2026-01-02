using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonDebug : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData e)
    {
        Debug.Log($"Klikniêto przycisk: {name}");
    }
}
