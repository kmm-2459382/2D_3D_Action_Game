#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class IDDatebaseEditWindow : EditorWindow
{
    [MenuItem("Tools/ID Datebase Tool")]
    //  エディタウィンドウを表示
    public static void ShowWindow()
    {
        GetWindow<IDDatebaseEditWindow>("ID Datebase Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("ID Datebase Utility", EditorStyles.boldLabel);

        if (GUILayout.Button("Show all ids"))
        {
            try
            {
                //  オブジェクトIDを表示
                IDShower.ShowObjID();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"✖オブジェクトID表示中にエラー発生✖: {ex.Message}\n{ex.StackTrace}");
            }
        }
        if (GUILayout.Button("Create Id datebase"))
        {
            try
            {
                //  IDDateBase作成
                IDDataBaseCreater.BuildDatabase();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"✖IDDateBase作成中にエラー発生: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
#endif
