using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Sceny docelowe")]
    public string sceneSpecs = "Peel_Specs";
    public string sceneInterior360 = "PeelInterior360";
    public string sceneSounds = "Peel_Sounds";
    public string sceneTestDrive = "SampleScene";
    public string sceneHomeMenu = "HomeMenu"; // 🔹 NOWA LINIA

    public void OpenSpecs() { Load(sceneSpecs); }
    public void OpenInterior360() { Load(sceneInterior360); }
    public void OpenSounds() { Load(sceneSounds); }
    public void OpenTestDrive() { Load(sceneTestDrive); }
    public void ReturnToHome() { Load(sceneHomeMenu); } // 🔹 NOWA METODA

    private void Load(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}
