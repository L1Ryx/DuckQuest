using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class DialogueRunner : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image nextIndicator;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Typing")]
    [SerializeField] private float charactersPerSecond = 40f;
    
    [Header("Dialogue Events")]
    [SerializeField] private UnityEvent OnDialogueStarted;
    [SerializeField] private UnityEvent OnDialogueFinished;
    private bool currentLineCompleted;

    
    

    private DialogueEncounter currentEncounter;
    private int currentLineIndex;
    private Coroutine typingRoutine;
    private bool isTyping;

    public bool IsRunning { get; private set; }

    // ===== Public API =====

    private void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
    public void StartDialogue(DialogueEncounter encounter)
    {
        if (encounter == null || encounter.lines.Count == 0)
            return;

        StopAllCoroutines();

        currentEncounter = encounter;
        currentLineIndex = 0;
        IsRunning = true;

        SetVisible(true);
        OnDialogueStarted?.Invoke();

        ShowLine(currentEncounter.lines[currentLineIndex]);
    }
    
    private void EndEncounter()
    {
        IsRunning = false;
        nextIndicator.gameObject.SetActive(false);

        currentEncounter.onEncounterEnd?.Raise();
        OnDialogueFinished?.Invoke();

        SetVisible(false);

        if (currentEncounter.nextEncounter != null)
            StartDialogue(currentEncounter.nextEncounter);
    }
    
    private void Update()
    {
        if (!IsRunning)
            return;

        if (Input.GetMouseButtonDown(0))
            Advance();
    }



    public void Advance()
    {
        if (!IsRunning)
            return;

        if (isTyping)
        {
            FinishTypingInstantly();
            return;
        }

        AdvanceToNextLine();
    }

    private void FinishTypingInstantly()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        var line = currentEncounter.lines[currentLineIndex];
        dialogueText.text = line.text;

        isTyping = false;
        nextIndicator.gameObject.SetActive(true);

        // Raise after-line event once
        line.afterLineEvent?.Raise();
    }

    // ===== Internal flow =====

    private void ShowLine(DialogueEncounter.Line line)
    {
        currentLineCompleted = false;
        
        // UI setup
        portraitImage.sprite = line.speaker.portrait;
        nameText.text = line.speaker.displayName;
        dialogueText.text = string.Empty;
        nextIndicator.gameObject.SetActive(false);

        // Optional line-start audio
        if (line.lineCue != null)
            Game.Ctx.Audio.PlayCueGlobal(line.lineCue);
        else if (line.speaker.lineStartCue != null)
            Game.Ctx.Audio.PlayCueGlobal(line.speaker.lineStartCue);

        typingRoutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(DialogueEncounter.Line line)
    {
        isTyping = true;

        float delay = 1f / charactersPerSecond;
        string text = line.text;

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];

            if (line.speaker != null && line.speaker.typingCue != null)
                Game.Ctx.Audio.PlayCueGlobal(line.speaker.typingCue);

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        nextIndicator.gameObject.SetActive(true);

        MarkLineCompleted(line);

        typingRoutine = null;
    }

    
    private void MarkLineCompleted(DialogueEncounter.Line line)
    {
        if (currentLineCompleted)
            return;

        currentLineCompleted = true;
        line.afterLineEvent?.Raise();
    }

    private void AdvanceToNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentEncounter.lines.Count)
        {
            EndEncounter();
            return;
        }

        ShowLine(currentEncounter.lines[currentLineIndex]);
    }
}
