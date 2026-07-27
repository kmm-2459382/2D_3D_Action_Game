using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン内のすべてのBlockSurfaceOptimizerを管理し、非再生時でも面の非表示判定を行う
/// </summary>
[ExecuteAlways]
public class BlockSurfaceManager : MonoBehaviour
{
    public static BlockSurfaceManager Instance { get; private set; }

    private Dictionary<Vector3Int, BlockSurfaceOptimizer> blockGrid = new Dictionary<Vector3Int, BlockSurfaceOptimizer>();

    private void OnEnable()
    {
        Instance = this;
        RefreshAllBlocks();
    }

    private void Update()
    {
        CheckGridChanges();
    }

    public void RefreshAllBlocks()
    {
        blockGrid.Clear();
        BlockSurfaceOptimizer[] blocks = FindObjectsByType<BlockSurfaceOptimizer>(FindObjectsSortMode.None);

        foreach (var block in blocks)
        {
            Vector3Int gridPos = Vector3Int.RoundToInt(block.transform.position);
            block.lastGridPos = gridPos;

            if (!blockGrid.ContainsKey(gridPos))
            {
                blockGrid.Add(gridPos, block);
            }
        }

        foreach (var kvp in blockGrid)
        {
            UpdateBlockSurfaces(kvp.Key, kvp.Value);
        }
    }

    private void CheckGridChanges()
    {
        BlockSurfaceOptimizer[] blocks = FindObjectsByType<BlockSurfaceOptimizer>(FindObjectsSortMode.None);

        if (blocks.Length != blockGrid.Count)
        {
            RefreshAllBlocks();
            return;
        }

        foreach (var block in blocks)
        {
            Vector3Int currentPos = Vector3Int.RoundToInt(block.transform.position);
            if (currentPos != block.lastGridPos)
            {
                RefreshAllBlocks();
                return;
            }
        }
    }

    private void UpdateBlockSurfaces(Vector3Int pos, BlockSurfaceOptimizer block)
    {
        // 指定された軸の対応関係に基づいて隣接チェック
        // Back:  -Z (Vector3Int.back)
        // Front: +Z (Vector3Int.forward)
        // Left:  +X (Vector3Int.right)  ※+X方向がLeft
        // Right: -X (Vector3Int.left)   ※-X方向がRight
        // Top:   +Y (Vector3Int.up)
        // Under: -Y (Vector3Int.down)

        bool hasBack = blockGrid.ContainsKey(pos + Vector3Int.back);    // -Z
        bool hasFront = blockGrid.ContainsKey(pos + Vector3Int.forward); // +Z
        bool hasLeft = blockGrid.ContainsKey(pos + Vector3Int.right);   // +X
        bool hasRight = blockGrid.ContainsKey(pos + Vector3Int.left);    // -X
        bool hasTop = blockGrid.ContainsKey(pos + Vector3Int.up);      // +Y
        bool hasUnder = blockGrid.ContainsKey(pos + Vector3Int.down);    // -Y

        block.UpdateSurfaces(hasBack, hasFront, hasLeft, hasRight, hasTop, hasUnder);
    }
}