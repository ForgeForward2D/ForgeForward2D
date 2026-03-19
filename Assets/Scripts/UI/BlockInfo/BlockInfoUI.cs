using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockInfoUI : UIComponent<BlockInfoManager>
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private Image blockIcon;

    public void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        BlockInfoManager.OnBlockInfoUpdate += RefreshUI;
    }

    public override void RefreshUI(BlockInfoManager manager)
    {
        if (manager == null)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        BlockType block = manager.GetTargetBlock();
        bool isBreakable = block != null && block.breakable;
        bool canBreak = isBreakable && manager.CanBreakWithCurrentTool(block);

        canvasGroup.alpha = 1f;

        Color color = !isBreakable ? Color.white : canBreak ? Color.green : Color.red;
        actionLabel.color = color;

        actionLabel.text = isBreakable ? "Mine" : "Interact";

        Tool relevantTool = isBreakable ? manager.GetRelevantTool(block) : null;
        blockIcon.sprite = relevantTool?.icon;
        blockIcon.gameObject.SetActive(relevantTool != null);
    }
}
