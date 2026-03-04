
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldInteractionManager : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;
    [SerializeField] PlayerController playerController;

    private TileMapManager tileMapManager;
    private bool isBreakingBlock;
    private bool cancelBreakingBlock;

    public event Action<(BlockType, Vector2Int)> OnBlockBroken;

    public void Start()
    {
        tileMapManager = GetComponent<TileMapManager>();
        isBreakingBlock = false;
        cancelBreakingBlock = false;
    }

    public Vector2Int PositionToCoordinate(Vector3 worldPosition)
    {
        return tileMapManager.PositionToCoordinate(worldPosition);
    }


    public bool StartBlockBreaking(Vector2Int cellPosition)
    {
        if (isBreakingBlock)
        {
            Debug.Log("Already breaking a block, cannot start breaking another block");
            return false;
        }

        BlockType blockType = tileMapManager.GetBlockTypeAtPosition(cellPosition);
        if (blockType == null)
        {
            Debug.Log("Cant interact with null block type (pos: " + cellPosition + ")");
            return false;
        }

        if (!blockType.breakable)
        {
            Debug.Log("Interacted with non-breakable block of type " + blockType.displayName + " at position " + cellPosition);
            return false;
        }

        Debug.Log("Started breaking block of type " + blockType.displayName + " at position " + cellPosition);
        StartCoroutine(BlockBreakingCoroutine(blockType, cellPosition));
        return true;
    }

    public void CancelBlockBreaking()
    {
        cancelBreakingBlock = true;
        Debug.Log("Block breaking cancelled");
    }

    private IEnumerator BlockBreakingCoroutine(BlockType blockType, Vector2Int cellPosition)
    {
        isBreakingBlock = true;
        cancelBreakingBlock = false;
        float totalBreakingTime = blockType.health / gameConfig.player_breaking_speed;

        float elapsed = 0f;
        float lastAnimation = -gameConfig.player_breaking_animation_interval;
        float lastTextureUpdateTime = -gameConfig.min_destruction_animation_interval;
        int lastStage = 0;
        while (elapsed < totalBreakingTime)
        {
            if (cancelBreakingBlock || playerController.GetTargettingBlock() != cellPosition)
            {
                tileMapManager.UpdateBlockBreakingProgress(cellPosition, 0);
                isBreakingBlock = false;
                yield break;
            }

            elapsed += Time.deltaTime;

            // Handle player animation update
            if (elapsed > lastAnimation + gameConfig.player_breaking_animation_interval)
            {
                playerController.TriggerAttackAnimation();
                lastAnimation = elapsed;
            }

            // Handle destroy animation  update
            float progress = Mathf.Clamp01(elapsed / totalBreakingTime);
            int stage = Mathf.CeilToInt(progress * 10f);

            if (stage != lastStage && elapsed > lastTextureUpdateTime + gameConfig.min_destruction_animation_interval)
            {
                tileMapManager.UpdateBlockBreakingProgress(cellPosition, stage);
                lastTextureUpdateTime = elapsed;
                lastStage = stage;
            }

            yield return null;
        }

        isBreakingBlock = false;
        tileMapManager.UpdateBlockBreakingProgress(cellPosition, 0);

        BlockType replacementBlockType = BlockTypeRepository.GetBlockById(blockType.replacementBlockId);
        tileMapManager.DrawBlock(replacementBlockType, cellPosition);

        Debug.Log("[EVENT] Broke block of type " + blockType.displayName + " at position " + cellPosition + ", replaced with " + replacementBlockType.displayName);
        OnBlockBroken?.Invoke((blockType, cellPosition));
    }


    public void InteractWithBlock(Vector2Int cellPosition)
    {
        // TODO: interactions with blocks (e.g. chests, crafting station, furnace )
    }
}