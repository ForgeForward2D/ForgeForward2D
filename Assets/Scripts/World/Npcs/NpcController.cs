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

    [SerializeField] private string[] pages;
    [SerializeField] private bool isDialogueActive = false;

    [Header("Mob Spawn Control")]
    [SerializeField] public bool reduceSpawn = false;
    [SerializeField] public int swordLevel = 0;
    [SerializeField] public bool acceptsSword = false;

    [Header("Dialogue Templates")]
    [SerializeField] private string giveSwordDialogue = "Thanks for the {0}! I'll use it to keep the monsters at bay. Mob spawns are now reduced by {1}%!";
    [SerializeField] private string sameSwordDialogue = "I already have a {0}, but thanks anyway!";
    [SerializeField] private string worseSwordDialogue = "My current sword is much better than that {0}!";
    [SerializeField] private string reduceSpawnReminderDialogue = "Like I said before, mob spawns are currently reduced by {0}%!";

    public void GiveSword(SwordItem sword)
    {
        if (sword == null || !acceptsSword) return;

        swordLevel = sword.swordLevel;
        reduceSpawn = true;
        Debug.Log($"Gave sword of level {sword.swordLevel} to NPC {GetDisplayName()}");

        int reductionPercentage = swordLevel * 20;
        string message = string.Format(giveSwordDialogue, sword.displayName, reductionPercentage);
        pages = BuildPages(new string[] { message });
        currentLineIndex = 0;
    }

    public void RejectSword(SwordItem sword)
    {
        if (sword == null || !acceptsSword) return;

        Debug.Log($"Rejected sword of level {sword.swordLevel} from NPC {GetDisplayName()} because they already have level {swordLevel}");

        List<string> lines = new List<string>();

        if (sword.swordLevel == swordLevel)
        {
            lines.Add(string.Format(sameSwordDialogue, sword.displayName));
        }
        else
        {
            lines.Add(string.Format(worseSwordDialogue, sword.displayName));
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

        InputManager.OnMoveInput += HandleMoveInput;
        InputManager.OnAttackInputUpdate += HandleAttackUpdate;
        InputManager.OnInteractionInput += HandleInteractionInput;
        UIManager.OnUpdatePage += HandleUpdatePage;
    }

    private void HandleUpdatePage(UIPage page)
    {
        if (page != UIPage.Dialogue)
            isDialogueActive = false;
    }

    private void HandleAttackUpdate((UIPage uiPage, bool isAttacking) data)
    {
        if (!isDialogueActive || !data.isAttacking) return;
        HandleDialogueNavigate(-1);
    }

    private void HandleInteractionInput(UIPage uiPage)
    {
        if (!isDialogueActive || uiPage != UIPage.Dialogue) return;
        HandleDialogueNavigate(1);
    }

    private void HandleMoveInput((UIPage uiPage, bool performed, Vector2 input) data)
    {
        if (!isDialogueActive || !data.performed) return;
        if (data.input.y == 0) return;

        if (data.input.y < 0f) HandleDialogueNavigate(1);
        else HandleDialogueNavigate(-1);
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

    public void CloseDialogue()
    {
        if (!isDialogueActive) return;
        isDialogueActive = false;
        OnSetDialogueUIActive?.Invoke(false);
    }

    public void HandleInteraction(UIPage uiPage)
    {
        if (uiPage == UIPage.None)
        {
            if (!CanBeginDialogue())
            {
                Debug.LogWarning($"Cannot begin dialogue with {GetDisplayName()}. Check previous warnings for details.");
                return;
            }
            currentLineIndex = 0;
            isDialogueActive = true;
            OnSetDialogueUIActive?.Invoke(true);
        }
        else if (uiPage == UIPage.Dialogue && isDialogueActive)
        {
            HandleDialogueNavigate(1);
            return;
        }
        OnNpcControllerUpdate?.Invoke(this);
    }

    public void HandleDialogueNavigate(int direction)
    {
        if (direction > 0)
        {
            if (currentLineIndex == pages.Length - 1) return;
            currentLineIndex++;
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
    public NpcType NpcType => npcType;
    public int LineIndex => currentLineIndex;
    public int TotalLines => pages != null ? pages.Length : 0;
    public Sprite CharacterSprite => npcType != null ? npcType.characterSprite : null;
}
