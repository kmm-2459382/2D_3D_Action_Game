using UnityEngine;
using System.Collections.Generic;

//   オブジェクトデータベース
[CreateAssetMenu(fileName = "ObjectDatabase", menuName = "GameData/ID Object Database")]
public class ObjectDatabase : ScriptableObject
{
    [Header("オブジェクトID(Toolから取得)")]
    [SerializeField] private List<string> ObjectIDs = new();    //  オブジェクトIDリスト
   
    //  ObjectIdsプロパティ
    public List<string> ObjectIDsProperty => ObjectIDs;

    //  オブジェクトIDを一括設定
    public void SetIDs(List<string> IDs)
    {
        ObjectIDs = IDs;
    }
}
//    コード保存所    ////
//// IDからデータを取得
//public ObjectData GetDataByID(string id)
//{
//    return objects.Find(obj => obj.id == id);
//}

//// プレハブ取得（必要なら）
//public GameObject GetPrefabByID(string id)
//{
//    var data = GetDataByID(id);
//    return data?.prefab;
//}
//}