using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public int ID;                   //番号
    public string EnemyName;         //名前
    public string Period;            //時代
    public int HP;　　　　　　　     //体力
    public int AttackPower;　　　　　//攻撃力
    public float Speed;　　　　　　　//速さ
    public GameObject EnemyPrefab;   //敵のモデル
    public AttackMethod[] Methods;   // 攻撃の配列
    public Defense[] Defenses;       // 防御の配列
}