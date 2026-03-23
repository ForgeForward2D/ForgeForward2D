using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public static event Action<(UIPage, NpcController)> OnNpcInteraction;
    public static event Action<DialogueState> OnNpcDialogueAdvance;
    public static event Action OnNpcInteractionEnd;

    private const int CharsPerPage = 200;

    [Header("Reference")]
    [SerializeField] private NpcType npcType;

    [Header("Debugging")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int currentLineIndex;
    [SerializeField] private bool inDialogue;

    private string[] pages;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (npcType != null && spriteRenderer != null)
            spriteRenderer.sprite = npcType.characterSprite;

        if (npcType != null)
            pages = BuildPages(npcType.dialogueLines);

        InputManager.OnInteractionInput += HandleInteractionInput;
    }

    private void OnDestroy()
    {
        InputManager.OnInteractionInput -= HandleInteractionInput;
    }

    private static string[] BuildPages(string[] lines)
    {
        var result = new List<string>();
        foreach (string line in lines)
        {
            if (line.Length <= CharsPerPage)
            {
                result.Add(line);
                continue;
            }

            string remaining = line;
            while (remaining.Length > CharsPerPage)
            {
                int split = remaining.LastIndexOf(' ', CharsPerPage);
                if (split <= 0) split = CharsPerPage;
                result.Add(remaining[..split].TrimEnd());
                remaining = remaining[split..].TrimStart();
            }
            if (remaining.Length > 0)
                result.Add(remaining);
        }
        return result.ToArray();
    }

    private void HandleInteractionInput(UIPage uiPage)
    {
        if (!inDialogue) return;
        if (uiPage != UIPage.Dialogue) return;

        AdvanceDialogue();
    }

    public void BeginDialogue()
    {
        if (npcType == null)
        {
            Debug.LogError($"NpcController on {gameObject.name} has no NpcType assigned.", this);
            return;
        }

        currentLineIndex = 0;
        inDialogue = true;

        Debug.Log($"Beginning dialogue with {npcType.displayName}");
        OnNpcDialogueAdvance?.Invoke(BuildCurrentState());
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex >= pages.Length)
        {
            EndDialogue();
            return;
        }

        Debug.Log($"Dialogue advancing to line {currentLineIndex}: {pages[currentLineIndex]}");
        OnNpcDialogueAdvance?.Invoke(BuildCurrentState());
    }

    public void EndDialogue()
    {
        if (!inDialogue) return;

        inDialogue = false;
        currentLineIndex = 0;
        Debug.Log($"Ending dialogue with {npcType.displayName}");
        OnNpcInteractionEnd?.Invoke();
    }

    private DialogueState BuildCurrentState()
    {
        return new DialogueState(
            npcType.displayName,
            pages[currentLineIndex],
            currentLineIndex,
            pages.Length,
            npcType.characterSprite
        );
    }

    public void RaiseInteraction(UIPage uiPage)
    {
        OnNpcInteraction?.Invoke((uiPage, this));
    }

    public string GetDisplayName()
    {
        return npcType != null ? npcType.displayName : gameObject.name;
    }
}
