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

    [Header("Mob Spawn Control")]
    [SerializeField] public bool reduceSpawn = false;
    [SerializeField] public int swordLevel = 0;

    [Header("Dialogue Templates")]
    [SerializeField] private string giveSwordDialogue = "Thanks for the {0}! I'll use it to keep the monsters at bay. Mob spawns are now reduced by {1}%!";
    [SerializeField] private string sameSwordDialogue = "I already have a {0}, but thanks anyway!";
    [SerializeField] private string worseSwordDialogue = "My current sword is much better than that {0}!";
    [SerializeField] private string reduceSpawnReminderDialogue = "Like I said before, mob spawns are currently reduced by {0}%!";

    public void GiveSword(Tool tool)
    {
        if (tool == null || tool.type != ToolType.Sword) return;

        swordLevel = (int)tool.tier;
        reduceSpawn = true;
        Debug.Log($"Gave sword of tier {tool.tier} to NPC {GetDisplayName()}");

        int reductionPercentage = swordLevel * 20;
        string message = string.Format(giveSwordDialogue, tool.displayName, reductionPercentage);
        pages = BuildPages(new string[] { message });
        currentLineIndex = 0;
    }

    public void RejectSword(Tool tool)
    {
        if (tool == null || tool.type != ToolType.Sword) return;

        Debug.Log($"Rejected sword of tier {tool.tier} from NPC {GetDisplayName()} because they already have tier {swordLevel}");

        List<string> lines = new List<string>();

        if ((int)tool.tier == swordLevel)
        {
            lines.Add(string.Format(sameSwordDialogue, tool.displayName));
        }
        else
        {
            lines.Add(string.Format(worseSwordDialogue, tool.displayName));
        }

        if (reduceSpawn)
        {
            int reductionPercentage = swordLevel * 20;
            lines.Add(string.Format(reduceSpawnReminderDialogue, reductionPercentage));
        }
        else if (npcType != null && npcType.dialogueLines != null)
        {
             lines.AddRange(npcType.dialogueLines);
        }

        pages = BuildPages(lines.ToArray());
        currentLineIndex = 0;
    }

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

    public string GetDisplayName()
    {
        return npcType != null ? npcType.displayName : gameObject.name;
    }
    public string CurrentLine => pages != null && currentLineIndex < pages.Length ? pages[currentLineIndex] : string.Empty;
    public int LineIndex => currentLineIndex;
    public int TotalLines => pages != null ? pages.Length : 0;
    public Sprite CharacterSprite => npcType != null ? npcType.characterSprite : null;
}
