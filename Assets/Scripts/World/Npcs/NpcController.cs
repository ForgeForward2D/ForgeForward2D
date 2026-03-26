using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public static event Action<bool> OnSetDialogueUIActive;
    public static event Action<NpcController> OnNpcControllerUpdate;

    [Header("Settings")]
    private const int CharsPerPage = 200;

    [Header("Reference")]
    [SerializeField] private NpcType npcType;

    [Header("Debugging")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int currentLineIndex;

    private string[] pages;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (npcType != null && spriteRenderer != null)
            spriteRenderer.sprite = npcType.characterSprite;

        if (npcType != null)
            pages = BuildPages(npcType.dialogueLines);
    }

    private string[] BuildPages(string[] lines)
    {
        if (lines == null) return Array.Empty<string>();

        var result = new List<string>();
        foreach (string rawLine in lines)
        {
            string line = rawLine ?? string.Empty;
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

    private bool CanBeginDialogue()
    {
        if (npcType == null)
        {
            Debug.LogError($"NpcController on {gameObject.name} has no NpcType assigned.", this);
            return false;
        }

        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning($"NpcController on {gameObject.name} has no dialogue lines.", this);
            return false;
        }

        return true;
    }

    public void HandleInteraction(UIPage uiPage)
    {
        if (uiPage == UIPage.None)
        {
            // Not active
            if (!CanBeginDialogue())
            {
                Debug.LogWarning($"Cannot begin dialogue with {GetDisplayName()}. Check previous warnings for details.");
                return;
            }
            currentLineIndex = 0;
            OnSetDialogueUIActive?.Invoke(true);
        }
        else if (uiPage == UIPage.Dialogue)
        {
            currentLineIndex++;
            if (currentLineIndex == pages.Length)
            {
                OnSetDialogueUIActive?.Invoke(false);
                return;
            }
        }
        OnNpcControllerUpdate?.Invoke(this);
    }

    public void HandleDialogueNavigate(int direction)
    {
        if (direction > 0)
        {
            currentLineIndex++;
            if (currentLineIndex == pages.Length)
            {
                OnSetDialogueUIActive?.Invoke(false);
                return;
            }
        }
        else
        {
            if (currentLineIndex == 0) return;
            currentLineIndex--;
        }
        OnNpcControllerUpdate?.Invoke(this);
    }

    public string GetDisplayName()
    {
        return npcType != null ? npcType.displayName : gameObject.name;
    }
    public string CurrentLine => pages != null && currentLineIndex < pages.Length ? pages[currentLineIndex] : string.Empty;
    public int LineIndex => currentLineIndex;
    public int TotalLines => pages != null ? pages.Length : 0;
    public Sprite CharacterSprite => npcType != null ? npcType.characterSprite : null;
}
