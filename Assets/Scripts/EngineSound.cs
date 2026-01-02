using System.Collections;
using UnityEngine;

public class PeelEngineSound : MonoBehaviour
{
    [Header("Źródła audio")]
    [Tooltip("AudioSource z dźwiękiem odpalenia (jednorazowo).")]
    public AudioSource startSource;

    [Tooltip("AudioSource z dźwiękiem pracującego silnika (loop).")]
    public AudioSource loopSource;

    [Header("Ustawienia")]
    [Tooltip("Czy zagrać od razu po starcie sceny (po odliczaniu możesz wywołać ręcznie)?")]
    public bool playOnStart = true;

    [Tooltip("Minimalne nakładanie się końcówki startu i początku loopa.")]
    public float crossfade = 0.05f;

    bool hasPlayed = false;

    void Start()
    {
        if (playOnStart)
        {
            PlayEngine();
        }
    }

    /// <summary>
    /// Wywołaj to, gdy chcesz odpalić silnik:
    /// np. z RaceManagera po odliczaniu.
    /// </summary>
    public void PlayEngine()
    {
        if (hasPlayed) return; // żeby nie odpalali się na siebie kilka razy
        hasPlayed = true;
        StartCoroutine(PlayStartThenLoop());
    }

    IEnumerator PlayStartThenLoop()
    {
        // na wszelki wypadek
        if (loopSource != null) loopSource.Stop();

        // 1) Odpalenie silnika (jednorazowo)
        if (startSource != null && startSource.clip != null)
        {
            startSource.loop = false;
            startSource.Play();

            float waitTime = Mathf.Max(0f, startSource.clip.length - crossfade);
            yield return new WaitForSeconds(waitTime);
        }

        // 2) Pętla silnika – gra w kółko
        if (loopSource != null && loopSource.clip != null)
        {
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    /// <summary>
    /// Zatrzymanie wszystkiego (np. przy wyjściu z jazdy).
    /// </summary>
    public void StopEngine()
    {
        hasPlayed = false;
        if (startSource != null) startSource.Stop();
        if (loopSource != null) loopSource.Stop();
    }
}
