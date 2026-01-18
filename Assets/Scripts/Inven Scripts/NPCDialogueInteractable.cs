using System.Collections.Generic;
using UnityEngine;

public sealed class NPCDialogueInteractable : InventoryCostInteractable
{
    [Header("Dialogue Sequence")]
    [SerializeField] private List<DialogueEncounter> encounters = new List<DialogueEncounter>();

    [Tooltip("If true, the NPC becomes non-interactable after the last encounter is used.")]
    [SerializeField] private bool stopAfterLastEncounter = false;

    [Tooltip("If false and stopAfterLastEncounter is also false, the NPC will keep replaying the last encounter.")]
    [SerializeField] private bool loopLastEncounter = true;

    [Header("Optional Audio")]
    [SerializeField] private AudioCue onInteractCue;

    [Header("Debug")]
    [SerializeField] private int currentEncounterIndex = 0;

    public override void Interact(GameObject interactor)
    {
        if (!Game.IsReady || Game.Ctx == null)
            return;

        if (Game.Ctx.Dialogue == null)
        {
            Debug.LogError($"{name}: DialogueRunner not bound in GameContext.");
            return;
        }

        if (encounters == null || encounters.Count == 0)
        {
            Debug.LogWarning($"{name}: No dialogue encounters assigned.");
            return;
        }

        // If we've exhausted the list...
        if (currentEncounterIndex >= encounters.Count)
        {
            if (stopAfterLastEncounter)
            {
                // Reuse existing base behavior pattern for "used up"
                hasBeenUsed = true;
                if (moveToInteractedLayerOnUse)
                    MoveHierarchyToLayer(interactedLayerName);
                return;
            }

            // Otherwise, clamp to last
            currentEncounterIndex = encounters.Count - 1;
        }

        var encounter = encounters[currentEncounterIndex];
        if (encounter == null)
        {
            Debug.LogWarning($"{name}: Encounter at index {currentEncounterIndex} is null.");
            return;
        }

        // Optional feedback cue
        if (onInteractCue != null)
            Game.Ctx.Audio.PlayCueGlobal(onInteractCue);

        // Start dialogue
        Game.Ctx.Dialogue.StartDialogue(encounter);

        // Keep optional hook consistent with other interactables
        onSuccess?.Invoke();

        // Advance index for next interaction
        if (currentEncounterIndex < encounters.Count - 1)
        {
            currentEncounterIndex++;
        }
        else
        {
            // We just used the last encounter
            if (stopAfterLastEncounter)
            {
                hasBeenUsed = true;
                if (moveToInteractedLayerOnUse)
                    MoveHierarchyToLayer(interactedLayerName);
                currentEncounterIndex++; // mark as exhausted
            }
            else if (loopLastEncounter)
            {
                // Stay at last index
                currentEncounterIndex = encounters.Count - 1;
            }
            else
            {
                // If loopLastEncounter is false, default to staying on last anyway
                // (prevents going out of range; this flag is mostly for clarity)
                currentEncounterIndex = encounters.Count - 1;
            }
        }
    }

    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        // Not used: this interactable is free (it overrides Interact()).
    }
}
