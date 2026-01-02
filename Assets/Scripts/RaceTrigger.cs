using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    public enum TriggerType { Start, Finish }
    public TriggerType type;
    public RaceManager manager;

    void Awake()
    {
        if (manager == null) manager = FindObjectOfType<RaceManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (manager == null) return;

        // Start dzia³a zawsze, gdy nie ma resetu
        if (type == TriggerType.Start)
        {
            if (!manager.IsResetting)
                manager.StartRace();
            return;
        }

        // Meta bywa chwilowo ignorowana
        if (type == TriggerType.Finish)
        {
            if (!manager.IsResetting && !manager.IgnoreFinish)
                manager.FinishRace();
        }
    }
}
