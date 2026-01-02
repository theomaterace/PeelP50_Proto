using UnityEngine;
using UnityEngine.SceneManagement;

public class PeelSoundMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip engineStart;
    public AudioClip engineIdle;
    public AudioClip horn;
    public AudioClip door;

    [Header("Nazwa sceny z menu Peela")]
    public string peelMenuSceneName = "PeelPresentation"; // tu wpisz swoj¹ nazwê sceny

    void PlayClip(AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void PlayEngineStart() => PlayClip(engineStart);
    public void PlayEngineIdle() => PlayClip(engineIdle);
    public void PlayHorn() => PlayClip(horn);
    public void PlayDoor() => PlayClip(door);

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(peelMenuSceneName);
    }
}
