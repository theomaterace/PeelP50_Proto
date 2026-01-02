using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleReturn : MonoBehaviour
{
    public float idleTime = 40f;  // ile sekund bez dotyku
    float timer = 0f;

    void Update()
    {
        // Reset jeœli dotkniêto ekranu lub nacisniêto mysz/klawisz
        if (Input.touchCount > 0 ||
            Input.GetMouseButtonDown(0) ||
            Input.anyKeyDown)
        {
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }

        // Powrót do menu
        if (timer >= idleTime)
        {
            SceneManager.LoadScene("Menu_PeelP50"); // twoja nazwa sceny menu
        }
    }
}
