using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIComponent<NpcController>
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueLineText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image characterImage;

    public void OnEnable()
    {
        NpcController.OnNpcControllerUpdate += RefreshUI;
    }

    private void OnDisable()
    {
        NpcController.OnNpcControllerUpdate -= RefreshUI;
    }

    public override void RefreshUI(NpcController state)
    {
        npcNameText.text = state.GetDisplayName();
        dialogueLineText.text = state.CurrentLine;
        progressText.text = $"{state.LineIndex + 1} / {state.TotalLines}";
        characterImage.sprite = state.CharacterSprite;
    }
}
