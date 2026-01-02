using UnityEngine;
using UnityEngine.SceneManagement;

public class NSUSceneLoader : MonoBehaviour
{
    [Header("Nazwa sceny z jazd¹ testow¹ NSU")]
    [SerializeField] private string nsuTestSceneName = "NSU_TestDrive";

    // Wywo³ywane z przycisku "Jazda testowa"
    public void LoadNSUTestDrive()
    {
        if (!string.IsNullOrEmpty(nsuTestSceneName))
        {
            SceneManager.LoadScene(nsuTestSceneName);
        }
        else
        {
            Debug.LogError("NSUSceneLoader: nie ustawiono nazwy sceny jazdy testowej NSU!");
        }
    }
}
