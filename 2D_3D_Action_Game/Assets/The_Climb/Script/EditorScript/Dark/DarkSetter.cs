#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

//  Darkのスケーリングを設定する
public class DarkSetter
{
    //  シーン内の闇の設定をする
    public static void ApplyToSceneDark()
    {
        GameObject[] sceneDarks = FindDarkInScene();

        int SetCnt = 0;    //  ID設定回数
        int TotalTask = sceneDarks.Length;    //  設定を適応するオブジェクトの総数
        const string ProgressTitle = "EdgeDarkの設定適用中";    //  プログレスバーのメッセージ
        const string ProgressMsg_Scene = "シーン:";    //  プログレスバーのシーン内設定適応中のメッセージ
        const string SetedImgMsg_Scene = "✅ シーンのEdgeDarkに設定適応:";    //  設定適応したMsg

        try
        {
            foreach (GameObject sceneDark in sceneDarks)
            {
                float progress = (float)SetCnt / TotalTask;    //  進行度
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Scene + sceneDark.name, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                //  イメージ設定変更のプログレスバーを表示
                if (IsCanceled)
                {
                    break;
                }
                //  設定適応
                ApplySettings(sceneDark);
                Debug.Log(SetedImgMsg_Scene + sceneDark.name, sceneDark);
                SetCnt++;
            }
        }
        finally
        {
            //  プログレスバー消去
            ProgressBarUtility.Clear();
        }
        Debug.Log($"CanvasScaler設定適用完了。処理対象: {SetCnt}個のCanvas");
    }
    //  シーン内のDarkを検索して取得
    static GameObject[] FindDarkInScene()
    {
        return Object.FindObjectsByType<IDGenerater>(FindObjectsSortMode.None)
                     .Where(Dark => Dark != null &&
                      Dark.CategoryAsStringProperty == IDEnumToString.ToString(IDCategory.EFFECT) &&
                      Dark.LabelAsStringProperty == IDEnumToString.ToString(IDLabel.DARK))
                     .Select(Dark => Dark.gameObject)
                     .ToArray();
    }
    //  Canvasに設定を適応する
    static void ApplySettings(GameObject dark)
    {
        Camera cam = Camera.main;    //  メインカメラのインスタンス

        float height = cam.orthographicSize * 2f;    //  画面に映る空間の高さ
        float width = height * cam.aspect;    //  画面の幅
        const string UndoLog = "Apply Setting To Dark";    //  ScalerでCtrl + Zした時のログ

        Undo.RecordObject(dark, UndoLog);

        dark.transform.position = cam.transform.position + Camera.main.transform.forward * 1f;
        dark.transform.localScale = new Vector3(width, height, 1f); // Quad 前提
    }
}
#endif
//  以下コード保存所  //
//return Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
//             .Where(Dark => Dark != null &&
//              Dark.GetComponent<IDGenerater>().CategoryProperty == IDEnumToString.ToString(IDCategory.EFFECT) &&
//              Dark.GetComponent<IDGenerater>().LabelProperty == IDEnumToString.ToString(IDLabel.DARK))
//             .ToArray();