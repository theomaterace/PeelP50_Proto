using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;        // licznik czasu
    public TextMeshProUGUI resultText;      // komunikaty / countdown
    public TextMeshProUGUI bestTimesText;   // tablica TOP5
    public Button restartButton;            // "Zagraj ponownie"
    public Button clearButton;              // "Nowe rekordy" (czyści TOP5)

    [Header("Gracz / kontrola")]
    public Transform player;                // np. PeelRoot / NSURoot
    public MonoBehaviour playerController;  // np. PeelController_Lite (opcjonalnie)

    [Header("Start / Meta")]
    public float finishIgnoreTime = 0.3f;   // chwilowa immunizacja mety po starcie
    public int countdownSeconds = 3;        // 3…2…1… Start!

    [Header("Audio (opcjonalnie)")]
    public AudioSource sfxSource;           // jeśli null, doda się automatycznie
    public AudioClip beepClip;              // 3/2/1
    public AudioClip startBeepClip;         // "Start!"
    [Range(0f, 1f)] public float countdownBeepVolume = 0.4f;
    [Range(0f, 1f)] public float startBeepVolume = 0.6f;

    // ─ stan wyścigu ─
    bool raceActive = false;
    float raceTime = 0f;

    // update UI co 0.1s
    float timeUiTimer = 0f;

    // ─ pozycja startowa ─
    Rigidbody playerRb;
    Vector3 startPos;
    Quaternion startRot;

    // ─ flagi ─
    public bool IsResetting { get; private set; } = false;
    public bool IgnoreFinish { get; private set; } = false;
    public bool RaceActive => raceActive;

    // ─ rekordy ─
    int lastInsertedIndex = -1;

    void Awake()
    {
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            if (playerController == null)
                playerController = player.GetComponent<MonoBehaviour>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D
            sfxSource.volume = 0.8f;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => StartCoroutine(ResetRunCoroutine()));
            restartButton.gameObject.SetActive(false);
        }

        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(ClearRecords);
            clearButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (player != null)
        {
            startPos = player.position;
            startRot = player.rotation;
        }

        if (timeText != null) timeText.text = FormatTime(0f);
        if (resultText != null) resultText.text = "";
        if (bestTimesText != null)
        {
            bestTimesText.text = "";
            bestTimesText.gameObject.SetActive(false);
        }

        StartCoroutine(InitialCountdownRoutine());
    }

    void Update()
    {
        if (raceActive)
        {
            raceTime += Time.deltaTime;

            if (timeText != null)
            {
                timeUiTimer += Time.deltaTime;
                if (timeUiTimer >= 0.1f)
                {
                    timeUiTimer = 0f;
                    timeText.text = FormatTime(raceTime);
                }
            }
        }
    }

    // ===== API z triggerów =====
    public void StartRace()
    {
        if (IsResetting) return;
        raceTime = 0f;
        raceActive = true;
        timeUiTimer = 0f;

        if (timeText != null) timeText.text = FormatTime(0f);
        if (resultText != null) resultText.text = "";
    }

    public void FinishRace()
    {
        if (IsResetting || IgnoreFinish) return;

        raceActive = false;

        if (resultText != null)
            resultText.text = "Czas: " + FormatTime(raceTime);

        SaveBestTime(raceTime);
        ShowBestTimes();

        if (restartButton != null) restartButton.gameObject.SetActive(true);
        if (clearButton != null) clearButton.gameObject.SetActive(true);
    }

    // ===== Procedury =====
    IEnumerator InitialCountdownRoutine()
    {
        IsResetting = true;
        if (playerController != null) playerController.enabled = false;

        HideButtons();
        HideBestTimes();

        // — SNAP DO ZIEMI (+3 cm) —
        if (player != null)
        {
            float startClearance = 0.03f;
            if (Physics.Raycast(startPos + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore))
                startPos.y = hit.point.y + startClearance;
        }

        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.position = startPos;
            playerRb.rotation = startRot;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            playerRb.isKinematic = false;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.Sleep();
        }

        // wyzeruj też wewnętrzny stan kontrolera
        var ctrl = player != null ? player.GetComponent<PeelController_Lite>() : null;
        if (ctrl != null) ctrl.ResetSpeed();

        // 3..2..1.. Start!
        yield return CountdownRoutine();

        if (playerController != null) playerController.enabled = true;
        IsResetting = false;

        // krótka immunizacja mety
        yield return ImmunizeFinish(finishIgnoreTime);
    }

    IEnumerator ResetRunCoroutine()
    {
        IsResetting = true;
        raceActive = false;
        raceTime = 0f;
        timeUiTimer = 0f;

        if (timeText != null) timeText.text = FormatTime(0f);
        if (resultText != null) resultText.text = "";

        HideButtons();
        HideBestTimes();

        // — SNAP DO ZIEMI (+3 cm) —
        if (player != null)
        {
            float startClearance = 0.03f;
            if (Physics.Raycast(startPos + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore))
                startPos.y = hit.point.y + startClearance;
        }

        if (player != null && playerRb != null)
        {
            if (playerController != null) playerController.enabled = false;

            playerRb.isKinematic = true;
            playerRb.position = startPos;
            playerRb.rotation = startRot;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            playerRb.isKinematic = false;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.Sleep();
        }

        var ctrl = player != null ? player.GetComponent<PeelController_Lite>() : null;
        if (ctrl != null) ctrl.ResetSpeed();

        // 3..2..1.. Start!
        yield return CountdownRoutine();

        if (playerController != null) playerController.enabled = true;
        IsResetting = false;

        yield return ImmunizeFinish(finishIgnoreTime);
    }

    IEnumerator CountdownRoutine()
    {
        if (resultText != null && countdownSeconds > 0)
        {
            for (int i = countdownSeconds; i >= 1; i--)
            {
                resultText.text = i.ToString();
                PlayBeep(beepClip, countdownBeepVolume);
                yield return new WaitForSeconds(1f);
            }
            resultText.text = "Start!";
            PlayBeep(startBeepClip, startBeepVolume);
            yield return new WaitForSeconds(0.2f);
            resultText.text = "";
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ImmunizeFinish(float seconds)
    {
        IgnoreFinish = true;
        yield return new WaitForSeconds(seconds);
        IgnoreFinish = false;
    }

    void PlayBeep(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    // ===== Rekordy TOP5 =====
    void SaveBestTime(float newTime)
    {
        float[] times = new float[5];
        for (int i = 0; i < 5; i++)
            times[i] = PlayerPrefs.GetFloat("BestTime" + i, 9999f);

        lastInsertedIndex = -1;
        for (int i = 0; i < 5; i++)
        {
            if (newTime < times[i])
            {
                for (int j = 4; j > i; j--) times[j] = times[j - 1];
                times[i] = newTime;
                lastInsertedIndex = i;
                break;
            }
        }

        for (int i = 0; i < 5; i++)
            PlayerPrefs.SetFloat("BestTime" + i, times[i]);

        PlayerPrefs.Save();
    }

    void ShowBestTimes(bool instant = false)
    {
        if (bestTimesText == null) return;

        string text = "<align=center><size=160%><b>░▒▓  Najlepsze czasy  ▓▒░</b></size></align>\n\n";
        for (int i = 0; i < 5; i++)
        {
            float t = PlayerPrefs.GetFloat("BestTime" + i, 9999f);
            if (t < 9999f)
            {
                string formatted = FormatTime(t);
                if (i == lastInsertedIndex)
                    text += $"<align=center><color=#FFD700><b>{i + 1}.  {formatted}  * NOWY</b></color></align>\n";
                else
                    text += $"<align=center><size=150%>{i + 1}.  {formatted}</size></align>\n";
            }
        }

        bestTimesText.text = text;
        bestTimesText.gameObject.SetActive(true);

        if (!instant) StartCoroutine(FadeInBestTimes());
        if (lastInsertedIndex >= 0) StartCoroutine(BlinkNewRecord());
    }

    void HideBestTimes()
    {
        if (bestTimesText != null) bestTimesText.gameObject.SetActive(false);
    }

    void HideButtons()
    {
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (clearButton != null) clearButton.gameObject.SetActive(false);
    }

    IEnumerator FadeInBestTimes()
    {
        CanvasGroup cg = bestTimesText.GetComponent<CanvasGroup>();
        if (cg == null) cg = bestTimesText.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator BlinkNewRecord()
    {
        float blinkTime = 1.5f, elapsed = 0f;
        bool dim = false;

        while (elapsed < blinkTime && bestTimesText != null)
        {
            string src = bestTimesText.text;
            string bright = "* NOWY";
            string dimmed = "<alpha=#66>* NOWY</alpha>";

            bestTimesText.text = dim ? src.Replace(bright, dimmed)
                                     : src.Replace(dimmed, bright);

            dim = !dim;
            yield return new WaitForSeconds(0.2f);
            elapsed += 0.2f;
        }

        if (bestTimesText != null)
            bestTimesText.text = bestTimesText.text.Replace("<alpha=#66>* NOWY</alpha>", "* NOWY");
    }

    void ClearRecords()
    {
        for (int i = 0; i < 5; i++)
            PlayerPrefs.DeleteKey("BestTime" + i);

        PlayerPrefs.Save();
        lastInsertedIndex = -1;
        ShowBestTimes(true);

        if (resultText != null)
            resultText.text = "Nowe rekordy ustawione";
    }

    // ===== Formatowanie czasu: mm:ss.ff =====
    string FormatTime(float timeSeconds)
    {
        int totalCentis = Mathf.FloorToInt(timeSeconds * 100f + 0.5f);
        int minutes = totalCentis / 6000;
        int seconds = (totalCentis / 100) % 60;
        int centis = totalCentis % 100;

        return $"{minutes:00}:{seconds:00}.{centis:00}";
    }
}
