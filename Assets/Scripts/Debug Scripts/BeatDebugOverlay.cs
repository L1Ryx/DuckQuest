using System.Text;
using TMPro;
using UnityEngine;

public class BeatDebugOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float refreshRateHz = 10f;
    [SerializeField] private LevelBeatDirector beatDirector;

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

        var sb = new StringBuilder(512);
        sb.AppendLine("Beat Debug:");

        var sequence = beatDirector.Sequence;
        var currentBeat = beatDirector.CurrentBeat;

        sb.Append("Active: ")
          .AppendLine(beatDirector.IsRunning ? "Yes" : "No");

        if (sequence == null)
        {
            sb.AppendLine("Sequence: (null)");
            text.text = sb.ToString();
            return;
        }

        int totalBeats = sequence.Beats.Count;
        int index = beatDirector.CurrentBeatIndex;

        sb.Append("Level: ")
          .AppendLine(sequence.LevelId);

        sb.Append("Sequence Description:")
          .AppendLine();

        if (!string.IsNullOrWhiteSpace(sequence.Description))
        {
            sb.Append("  ")
              .AppendLine(sequence.Description.Replace("\n", "\n  "));
        }
        else
        {
            sb.AppendLine("  (none)");
        }

        sb.AppendLine();

        sb.Append("Beat Index: ")
          .Append(index)
          .Append(" / ")
          .Append(Mathf.Max(0, totalBeats - 1))
          .AppendLine();

        sb.Append("Beat Number: ")
          .Append(index + 1)
          .Append(" / ")
          .Append(totalBeats)
          .AppendLine();

        if (currentBeat == null)
        {
            sb.AppendLine("Beat: (none)");
        }
        else
        {
            sb.Append("Beat ID: ")
              .AppendLine(currentBeat.BeatId);

            sb.Append("Beat Name: ")
              .AppendLine(currentBeat.name);

            sb.Append("Beat Description:")
              .AppendLine();

            if (!string.IsNullOrWhiteSpace(currentBeat.Description))
            {
                sb.Append("  ")
                  .AppendLine(currentBeat.Description.Replace("\n", "\n  "));
            }
            else
            {
                sb.AppendLine("  (none)");
            }
        }

        text.text = sb.ToString();
    }
}
