using UnityEngine;

//  IDからオブジェクト表示
public class IDShower
{
    //  IDを所持しているオブジェクトを表示
    public static void ShowObjID()
    {
        IDGenerater[] allID = GameObject.FindObjectsByType<IDGenerater>(FindObjectsSortMode.None);
        foreach (IDGenerater IDs in allID)
        {
            Debug.Log($"ID: {IDs.IDProperty} - Object: {IDs.name}", IDs.gameObject);
        }
    }
}
