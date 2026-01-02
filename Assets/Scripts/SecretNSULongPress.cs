using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SecretNSULongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Ustawienia tajnego przycisku")]
    [Tooltip("Ile sekund trzeba przytrzymać palcem.")]
    public float holdTime = 2.0f;

    [Tooltip("Nazwa sceny z prezentacją NSU (dokładnie tak jak w Build Settings).")]
    public string nsuSceneName = "NSU_Presentation";

    float timer = 0f;
    bool isHolding = false;
    bool hasLoaded = false;

    void Update()
    {
        if (!isHolding || hasLoaded)
            return;

        timer += Time.unscaledDeltaTime;

        if (timer >= holdTime)
        {
            hasLoaded = true;
            // na wszelki wypadek wyłącz dalsze trzymanie
            isHolding = false;

            // Dla debugowania w edytorze
            Debug.Log($"SECRET NSU: long press OK, loading scene '{nsuSceneName}'");

            SceneManager.LoadScene(nsuSceneName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // zaczynamy odliczanie
        isHolding = true;
        timer = 0f;
        hasLoaded = false;
        Debug.Log("SECRET NSU: PointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // puszczone za wcześnie → reset
        isHolding = false;
        timer = 0f;
        Debug.Log("SECRET NSU: PointerUp (reset)");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // palec wyszedł poza obszar → reset
        isHolding = false;
        timer = 0f;
        Debug.Log("SECRET NSU: PointerExit (reset)");
    }
}
