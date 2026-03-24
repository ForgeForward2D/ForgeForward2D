using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMapManager))]
public class ResourceGenerator : MonoBehaviour
{
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

    private void HandleBlockBroken((BlockType blockType, Vector2Int position, Tool tool) brokenBlockInfo)
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

            BlockType currentBlock = tileMapManager.GetBlockTypeAtPosition(position);
            BlockType replacementBlock = block.replacementBlock;

            // if current block does not equal the old block's replacementBlock
            if (currentBlock == replacementBlock)
            {
                if (!tileMapManager.IsOccupied(position))
                {
                    tileMapManager.DrawBlock(block, position);
                    Debug.Log($"Generated {block.displayName} at {position} after delay of {block.respawnRate} seconds.");
                    break;
                }
                else
                {
                    Debug.Log($"Skipping regeneration of {block.displayName} at {position} because tile is occupied");
                }
            }
            else
            {
                Debug.Log($"Skipping regeneration of {block.displayName} at {position} because block at cell position ({currentBlock?.displayName}) does not equal the replacement block ({block.replacementBlock?.displayName}).");
                break;
            }
        }

    }
}