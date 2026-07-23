using UnityEditor;
using UnityEngine;

//  キッカーのステータスブロック
[System.Serializable]
public class KickerStatBlock
{
    [Header("端を判定する光線の位置")]
    public Vector3 EdgeRayOffset;    //  端判定のRayのオフセット
    [Header("移動値")]
    public float MoveSpd;    //  移動速度
    public float JumpForce;    //  ジャンプ力
    public float JumpFrequency;    //  ジャンプ頻度
    [Header("吹っ飛ばし")]
    public float BlowForceX;    //  X軸の吹っ飛ばし力
    public float BlowForceY;    //  Y軸の吹っ飛ばし力
    public ForceMode GroundBlowMode;  //  吹き飛ばし方式
    public ForceMode AirBlowMode;  //  吹き飛ばし方式
}
