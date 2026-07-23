#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//  IDデータベース構築
public class IDDataBaseCreater
{
    //   データベース構築
    public static void BuildDatabase()
    {
        ObjectDatabase myObjectDateBase = AssetDatabase.LoadAssetAtPath<ObjectDatabase>("Assets/Script/System/ID/ScriptableObject/ObjectDatabase.asset");
        if (myObjectDateBase == null)
        {
            Debug.LogError("指定されたpathにMyObjectDateBaseがありません");
            return;
        }

        IDGenerater[] allIDObjects = Object.FindObjectsByType<IDGenerater>(FindObjectsSortMode.None);    //  IDがあるオブジェクトを取得する
        List<string> ids = new();    //  ID文字列のList
        foreach (var idObj in allIDObjects)
        {
            if (!string.IsNullOrEmpty(idObj.IDProperty))
                ids.Add(idObj.IDProperty);
        }

        myObjectDateBase.SetIDs(ids);
        EditorUtility.SetDirty(myObjectDateBase);
        AssetDatabase.SaveAssets();
        Debug.Log("ID データベース更新完了：" + ids.Count + " 件");
    }
}
#endif