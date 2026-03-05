using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform playerTransform;

    [SerializeField] TileMapManager tileMapManager;

    public void Start()
    {
        playerController.OnBlockBroken += HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType, Vector2Int) brokenBlockInfo)
    {
        BlockType block = brokenBlockInfo.Item1;
        Vector2Int position = brokenBlockInfo.Item2;

        Debug.Log("ResourceGenerator received block broken event for " + block.displayName + " at " + position);

        // Check if the broken block has a spawn rate and valid spawn range
        if (block.respawnRate > 0)
        {
            // Start a coroutine to generate resources after the specified spawn rate
            StartCoroutine(RegenerateResourceAfterDelay(block, position));
        }
    }

    private IEnumerator RegenerateResourceAfterDelay(BlockType block, Vector2Int position)
    {
        while (true)
        {
            yield return new WaitForSeconds(block.respawnRate);

            Vector2Int cellPosition = position;
            Vector2Int playerCellPosition = tileMapManager.PositionToCoordinate(playerTransform.position);

            BlockType currentBlock = tileMapManager.GetBlockTypeAtPosition(cellPosition);
            // if current block does not equal the old block's replacementBlock
            int currentBlockId = currentBlock == null ? 0 : currentBlock.id;
            if (currentBlockId == block.replacementBlockId)
            {
                if (cellPosition != playerCellPosition)
                {
                    tileMapManager.DrawBlock(block, cellPosition);
                    Debug.Log("Generated " + block.displayName + " at " + cellPosition + " after delay of " + block.respawnRate + " seconds.");
                    break;
                }
                else
                {
                    Debug.Log("Skipping regeneration of " + block.displayName + " at " + cellPosition + " because player is too close (distance: " + Vector2Int.Distance(cellPosition, playerCellPosition) + ").");
                }
            }
            else
            {
                Debug.Log("Skipping regeneration of " + block.displayName + " at " + cellPosition + " because block at cell position (id: " + currentBlockId + ") does not equal the replacement block (id: " + block.replacementBlockId + ").");
                break;
            }
        }

    }
}