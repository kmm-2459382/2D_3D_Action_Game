using UnityEngine;

public class StagePath : MonoBehaviour
{
    public enum PathState
    {
        Locked,     // 黒：まだ行けない
        Available,  // 銀：次に行ける
        Passed      // 金：通過済み
    }

    [Header("接続ステージ番号")]
    public int fromStage;
    public int toStage;

    [Header("見た目")]
    public Renderer pathRenderer;

    [Header("マテリアル")]
    public Material lockedMat;     // 黒
    public Material availableMat;  // 銀
    public Material passedMat;     // 金

    private PathState currentState;

    private void Awake()
    {
        if (pathRenderer == null)
            pathRenderer = GetComponentInChildren<Renderer>();

        SetState(PathState.Locked);
    }

    public void SetState(PathState state)
    {
        currentState = state;

        if (pathRenderer == null) return;

        switch (state)
        {
            case PathState.Locked:
                pathRenderer.material = lockedMat;
                break;
            case PathState.Available:
                pathRenderer.material = availableMat;
                break;
            case PathState.Passed:
                pathRenderer.material = passedMat;
                break;
        }

        pathRenderer.gameObject.SetActive(true);
    }
}
