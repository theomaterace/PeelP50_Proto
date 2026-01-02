using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretNSUController : MonoBehaviour
{
    [Header("Nazwa sceny z prezentacj¹ NSU")]
    public string nsuSceneName = "NSU_Presentation";   // ustaw w Inspectorze

    // Wywo³ywane po TAPniêciu w reflektor
    public void GoToNSU()
    {
        if (!string.IsNullOrEmpty(nsuSceneName))
        {
            SceneManager.LoadScene(nsuSceneName);
        }
        else
        {
            Debug.LogWarning("SecretNSUController: nsuSceneName jest puste.");
        }
    }
}
