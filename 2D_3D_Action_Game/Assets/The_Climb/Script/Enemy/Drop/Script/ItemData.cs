using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;              //アイテムの名前
    public Sprite Icon;　　　　      //アイコン
    public GameObject DropPrefab;　　//落とすときのプレハブ
    public float DropRate;           //落とす確率
}
