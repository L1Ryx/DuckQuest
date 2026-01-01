using System.Text;
using TMPro;
using UnityEngine;

public class BeatDebugOverlay : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private LevelBeatDirector beatDirector;

    [Header("Update Rate")]
    [SerializeField] private float refreshRateHz = 10f;

    [Header("Toggles")]
    [SerializeField] private bool showHeader = true;
    [SerializeField] private bool showActive = true;

    [SerializeField] private bool showLevelId = true;
    [SerializeField] private bool showSequenceDescription = true;

    [SerializeField] private bool showBeatIndex = true;
    [SerializeField] private bool showBeatNumber = true;

    [SerializeField] private bool showBeatId = true;
    [SerializeField] private bool showBeatName = true;
    [SerializeField] private bool showBeatDescription = true;

    [SerializeField] private bool showOnEnterEvents = true;
    [SerializeField] private bool showAdvanceEvent = true;

    private float timer;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
        beatDirector = FindObjectOfType<LevelBeatDirector>();
    }

    private void Update()
    {
        if (!Game.IsReady || text == null || beatDirector == null)
            return;

        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, refreshRateHz);
        if (timer < interval) return;
        timer = 0f;

        var sb = new StringBuilder(768);

        if (showHeader)
            sb.AppendLine("Beat Debug:");

        var sequence = beatDirector.Sequence;
        var currentBeat = beatDirector.CurrentBeat;

        if (showActive)
        {
            sb.Append("Active: ")
              .AppendLine(beatDirector.IsRunning ? "Yes" : "No");
        }

        if (sequence == null)
        {
            if (showLevelId) sb.AppendLine("Level: (null sequence)");
            text.text = sb.ToString();
            return;
        }

        int totalBeats = sequence.Beats.Count;
        int index = beatDirector.CurrentBeatIndex;

        if (showLevelId)
        {
            sb.Append("Level: ")
              .AppendLine(sequence.LevelId);
        }

        if (showSequenceDescription)
        {
            sb.Append("Sequence Description:")
              .AppendLine();

            if (!string.IsNullOrWhiteSpace(sequence.Description))
                sb.Append("  ").AppendLine(sequence.Description.Replace("\n", "\n  "));
            else
                sb.AppendLine("  (none)");

            sb.AppendLine();
        }

        if (showBeatIndex)
        {
            sb.Append("Beat Index: ")
              .Append(index)
              .Append(" / ")
              .Append(Mathf.Max(0, totalBeats - 1))
              .AppendLine();
        }

        if (showBeatNumber)
        {
            sb.Append("Beat Number: ")
              .Append(index + 1)
              .Append(" / ")
              .Append(totalBeats)
              .AppendLine();
        }

        if (currentBeat == null)
        {
            sb.AppendLine("Beat: (none)");
            text.text = sb.ToString();
            return;
        }

        if (showBeatId)
        {
            sb.Append("Beat ID: ")
              .AppendLine(currentBeat.BeatId);
        }

        if (showBeatName)
        {
            sb.Append("Beat Name: ")
              .AppendLine(currentBeat.name);
        }

        if (showBeatDescription)
        {
            sb.Append("Beat Description:")
              .AppendLine();

            if (!string.IsNullOrWhiteSpace(currentBeat.Description))
                sb.Append("  ").AppendLine(currentBeat.Description.Replace("\n", "\n  "));
            else
                sb.AppendLine("  (none)");

            sb.AppendLine();
        }

        if (showOnEnterEvents)
        {
            sb.AppendLine("Beat OnEnter Events:");

            var enterEvents = currentBeat.OnEnterEvents;
            if (enterEvents == null || enterEvents.Count == 0)
            {
                sb.AppendLine("  (none)");
            }
            else
            {
                for (int i = 0; i < enterEvents.Count; i++)
                {
                    var evt = enterEvents[i];
                    sb.Append("  • ")
                      .AppendLine(evt != null ? evt.name : "(null)");
                }
            }

            sb.AppendLine();
        }

        if (showAdvanceEvent)
        {
            sb.Append("Beat Advance Event: ");
            sb.AppendLine(currentBeat.AdvanceEvent != null ? currentBeat.AdvanceEvent.name : "(none)");
        }

        text.text = sb.ToString();
    }
}
