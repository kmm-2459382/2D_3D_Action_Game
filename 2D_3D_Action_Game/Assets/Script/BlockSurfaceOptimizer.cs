using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブロックの隣接状況に応じて、各方向の表面パーツ群（外郭）の表示/非表示を切り替えるスクリプト
/// </summary>
[ExecuteAlways]
public class BlockSurfaceOptimizer : MonoBehaviour
{
    [Header("Surface Parts Lists")]
    public List<GameObject> surfaceBackList = new List<GameObject>();   // -Z
    public List<GameObject> surfaceFrontList = new List<GameObject>();  // +Z
    public List<GameObject> surfaceLeftList = new List<GameObject>();   // +X
    public List<GameObject> surfaceRightList = new List<GameObject>();  // -X
    public List<GameObject> surfaceTopList = new List<GameObject>();    // +Y
    public List<GameObject> surfaceUnderList = new List<GameObject>();  // -Y

    [HideInInspector] public Vector3Int lastGridPos;

    /// <summary>
    /// 隣接ブロックが存在する方向の面リストを非表示(false)にする
    /// </summary>
    public void UpdateSurfaces(bool hasBack, bool hasFront, bool hasLeft, bool hasRight, bool hasTop, bool hasUnder)
    {
        SetSurfaceListActive(surfaceBackList, !hasBack);
        SetSurfaceListActive(surfaceFrontList, !hasFront);
        SetSurfaceListActive(surfaceLeftList, !hasLeft);
        SetSurfaceListActive(surfaceRightList, !hasRight);
        SetSurfaceListActive(surfaceTopList, !hasTop);
        SetSurfaceListActive(surfaceUnderList, !hasUnder);
    }

    /// <summary>
    /// 指定されたリスト内のすべてのGameObjectの表示状態を一括変更する
    /// </summary>
    private void SetSurfaceListActive(List<GameObject> surfaceList, bool active)
    {
        if (surfaceList == null) return;

        foreach (var surface in surfaceList)
        {
            if (surface != null && surface.activeSelf != active)
            {
                surface.SetActive(active);
            }
        }
    }

    private void OnTransformChildrenChanged()
    {
        BlockSurfaceManager.Instance?.RefreshAllBlocks();
    }
}