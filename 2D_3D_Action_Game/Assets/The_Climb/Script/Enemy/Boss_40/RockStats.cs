using UnityEngine;

[CreateAssetMenu(menuName = "BossStats/RockStats")]
public class RockStats : ScriptableObject
{
    [Header("移動設定")]
    public float crawlSpeed = 2f;
    public Vector3[] crawlPoints;

    [Header("噛みつき（ジャンプ攻撃）設定")]
    public float lungeSpeed = 10f;
    public float biteRange = 2f;
    public float waitBeforeLunge = 1.5f;

    [Header("噛みつき成功時演出")]
    public float spinDuration = 1.5f;
    public float throwForce = 25f;

    [Header("噛みつき失敗時のスタン")]
    public float staggerDuration = 2.5f;

    [Header("被弾判定")]
    public int maxMeteorHits = 3;
}