using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Boss_20_StatusObjectScript", menuName = "Scriptable Objects/Boss_20_StatusObjectScript")]
public class Boss_20_StatusObjectScript : ScriptableObject
{
    public string NAME;               //敵の名前
    public int    HP;　　　           //敵のHP
    public float  Speed;              //敵の速さ
    public int    Blow_away;          //吹っ飛ばし力
    public int    LEFT;               //左の移動
    public int    LEFT_Max;           //左の移動の動ける範囲
    public int    RIGHT;　　　　　　  //右の移動
    public int    RIGHT_Max;          //右の移動の動ける範囲
    public int    Rest;               //休憩のタイミング
    public float  Vertical;           //縦移動
    public int    Attack;             //遠距離攻撃のタイミング
    public float  Attack_Speed;       //遠距離攻撃の速さ
}
