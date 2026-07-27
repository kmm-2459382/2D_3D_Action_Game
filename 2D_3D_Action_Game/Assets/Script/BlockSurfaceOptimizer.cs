using UnityEngine;

/// <summary>
/// ブロックの隣接状況に応じて、表面パーツ（外郭）の表示/非表示を切り替えるスクリプト
/// </summary>
[ExecuteAlways]
public class BlockSurfaceOptimizer : MonoBehaviour
{
    [Header("Surface Parts")]
    public GameObject surfaceBack;   // -Z
    public GameObject surfaceFront;  // +Z
    public GameObject surfaceLeft;   // +X
    public GameObject surfaceRight;  // -X
    public GameObject surfaceTop;    // +Y
    public GameObject surfaceUnder;  // -Y

    [HideInInspector] public Vector3Int lastGridPos;

    /// <summary>
    /// 隣接ブロックが存在する方向の面を非表示(false)にする
    /// </summary>
    public void UpdateSurfaces(bool hasBack, bool hasFront, bool hasLeft, bool hasRight, bool hasTop, bool hasUnder)
    {
        if (surfaceBack && surfaceBack.activeSelf == hasBack) surfaceBack.SetActive(!hasBack);
        if (surfaceFront && surfaceFront.activeSelf == hasFront) surfaceFront.SetActive(!hasFront);
        if (surfaceLeft && surfaceLeft.activeSelf == hasLeft) surfaceLeft.SetActive(!hasLeft);
        if (surfaceRight && surfaceRight.activeSelf == hasRight) surfaceRight.SetActive(!hasRight);
        if (surfaceTop && surfaceTop.activeSelf == hasTop) surfaceTop.SetActive(!hasTop);
        if (surfaceUnder && surfaceUnder.activeSelf == hasUnder) surfaceUnder.SetActive(!hasUnder);
    }

    private void OnTransformChildrenChanged()
    {
        BlockSurfaceManager.Instance?.RefreshAllBlocks();
    }
}