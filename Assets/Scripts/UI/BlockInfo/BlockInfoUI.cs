using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class BlockInfoUI : UIComponent<TargetBlockInfoManager>
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private Image blockIcon;

    public void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        TargetBlockInfoManager.OnBlockInfoUpdate += RefreshUI;
    }

    public override void RefreshUI(TargetBlockInfoManager manager)
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

        if (isBreakable)
        {
            Tool relevantTool = manager.GetRelevantTool(block);
            blockIcon.sprite = relevantTool?.icon;
            blockIcon.gameObject.SetActive(relevantTool != null);
        }
        else
        {
            Sprite tileSprite = (block.tile as Tile)?.sprite;
            blockIcon.sprite = tileSprite;
            blockIcon.gameObject.SetActive(tileSprite != null);
        }
    }
}
