using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMapManager))]
public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private TileMapManager tileMapManager;

    private void OnEnable()
    {
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
        tileMapManager = GetComponent<TileMapManager>();
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType blockType, Vector2Int position) brokenBlockInfo)
    {
        BlockType block = brokenBlockInfo.blockType;
        Vector2Int position = brokenBlockInfo.position;

        if (block != null && block.respawnRate > 0)
        {
            // Start a coroutine to generate resources after the specified spawn rate
            StartCoroutine(RegenerateResourceAfterDelay(block, position));
        }
    }

    private IEnumerator RegenerateResourceAfterDelay(BlockType block, Vector2Int position)
    {
        if (block == null)
        {
            Debug.LogError("BlockType is null. Cannot regenerate resource.");
            yield break;
        }
        Debug.Log($"Started regeneration process for {block.displayName} at {position} with respawn rate of {block.respawnRate} seconds.");
        while (true)
        {
            yield return new WaitForSeconds(block.respawnRate);

            Vector2Int cellPosition = position;
            Vector2Int playerCellPosition = tileMapManager.PositionToCoordinate(playerController.GetPosition());

            BlockType currentBlock = tileMapManager.GetBlockTypeAtPosition(cellPosition);
            BlockType replacementBlock = block.replacementBlock;

            // if current block does not equal the old block's replacementBlock
            if (currentBlock == replacementBlock)
            {
                if (cellPosition != playerCellPosition)
                {
                    tileMapManager.DrawBlock(block, cellPosition);
                    Debug.Log($"Generated {block.displayName} at {cellPosition} after delay of {block.respawnRate} seconds.");
                    break;
                }
                else
                {
                    Debug.Log($"Skipping regeneration of {block.displayName} at {cellPosition} because player is too close (distance: {Vector2Int.Distance(cellPosition, playerCellPosition)}).");
                }
            }
            else
            {
                Debug.Log($"Skipping regeneration of {block.displayName} at {cellPosition} because block at cell position ({currentBlock.displayName}) does not equal the replacement block ({block.replacementBlock.displayName}).");
                break;
            }
        }

    }
}