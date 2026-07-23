using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStats/DropStats")]
public class DropStats : ScriptableObject
{
    [Header("突進設定")]
    public float RushSpeed = 10f;//往復速度
    public int DiagonalRushCount = 3;//往復回数（片道ずつのカウント）
    public float PointA_X = -10f;  // 左端（A地点）
    public float PointB_X = 10f;   // 右端（B地点）

    [Header("メテオドロップ設定")]
    public float MeteorDropSpeed = 20f;//メテオドロップの速度
    public int meteorDropCount = 2;
    public float RiseSpeed = 5f;
    public float WaitBeforeMeteor = 1.5f;
    public float HoverHeight = 20f; // メテオ前のホバーポジションY

    [Header("メテオドロップのエイム中の設定")]
    public float AimMoveLeftX = -3f;  // 狙い動作の左端
    public float AimMoveRightX = 3f;  // 狙い動作の右端
    public float AimMoveSpeed = 5f;   // 狙い動作のスピード
    public int AimMoveCount = 3;      // 往復回数（片道 = 1 回）

    [Header("補足詳細設定")]
    public float GroundY = 0f;     // 地面Y座標（接地判定や落下停止用）
        
}
