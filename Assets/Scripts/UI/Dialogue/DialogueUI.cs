using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIComponent<DialogueState>
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueLineText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image characterImage;

    public void OnEnable()
    {
        NpcController.OnNpcDialogueAdvance += RefreshUI;
    }

    private void OnDisable()
    {
        NpcController.OnNpcDialogueAdvance -= RefreshUI;
    }

    public override void RefreshUI(DialogueState state)
    {
        npcNameText.text = state.NpcName;
        dialogueLineText.text = state.CurrentLine;
        progressText.text = $"{state.LineIndex + 1} / {state.TotalLines}";
        characterImage.sprite = state.CharacterSprite;
    }
}
