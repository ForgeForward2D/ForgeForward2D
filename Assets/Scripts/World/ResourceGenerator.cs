using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMapManager))]
public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private TileMapManager tileMapManager;

    public void Start()
    {
        tileMapManager = GetComponent<TileMapManager>();
        GetComponent<WorldInteractionManager>().OnBlockBroken += HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType, Vector2Int) brokenBlockInfo)
    {
        BlockType block = brokenBlockInfo.Item1;
        Vector2Int position = brokenBlockInfo.Item2;

        // Check if the broken block is regeneratable
        if (block.respawnRate > 0)
        {
            // Start a coroutine to generate resources after the specified spawn rate
            StartCoroutine(RegenerateResourceAfterDelay(block, position));
        }
    }

    private IEnumerator RegenerateResourceAfterDelay(BlockType block, Vector2Int position)
    {
        Debug.Log("Started regeneration process for " + block.displayName + " at " + position + " with respawn rate of " + block.respawnRate + " seconds.");
        while (true)
        {
            yield return new WaitForSeconds(block.respawnRate);

            Vector2Int cellPosition = position;
            Vector2Int playerCellPosition = tileMapManager.PositionToCoordinate(playerController.GetPosition());

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