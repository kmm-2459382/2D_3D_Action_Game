using UnityEngine;

[CreateAssetMenu(fileName = "HevvyStats", menuName = "BossStats/HevvyStats")]
public class HevvyStats : ScriptableObject
{
    [Header("移動・ジャンプ関連")]
    public float hopSpeed = 2f;
    public float verticalJumpForce = 12f;
    public float arcJumpForce = 8f;
    public float arcJumpHeight = 5f;
    public float smallHopForce = 1.5f;          //

    [Header("ホップ移動（ジャンプ型移動）")]
    public int hopMoveCount = 3;                 // ホップの回数
    public float hopMoveForce = 3f;              // 前方向の力
    public float hopMoveHeight = 4f;             // 上方向のジャンプ力

    [Header("チャージ・スタン時間")]
    public float chargeTime = 1.5f;
    public float stunDuration = 3f;

    [Header("距離トリガー")]
    public float nearTriggerDistance = 3f;　　　 //
    public float farTriggerDistance = 6f;　　　　//ボスの垂直ジャンプのエリア範囲

    [Header("必要ヒット数")]
    public int requiredHitsToDefeat = 3;         //ボスのライフ


}