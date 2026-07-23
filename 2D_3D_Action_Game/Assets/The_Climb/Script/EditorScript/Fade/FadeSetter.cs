#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

//  Darkのスケーリングを設定する
public class FadeSetter
{
    //  シーン内のフェード板の設定をする
    public static void ApplyToSceneFadeQuad()
    {
        GameObject[] sceneFades = FindFadeQuadInScene();

        int SetCnt = 0;    //  ID設定回数
        int TotalTask = sceneFades.Length;    //  設定を適応するオブジェクトの総数
        const string ProgressTitle = "FadePlaneの設定適用中";    //  プログレスバーのメッセージ
        const string ProgressMsg_Scene = "シーン:";    //  プログレスバーのシーン内設定適応中のメッセージ
        const string SetedImgMsg_Scene = "✅ シーンのFadePlaneに設定適応:";    //  設定適応したMsg
        
        try
        {
            foreach (GameObject sceneFade in sceneFades)
            {
                float progress = (float)SetCnt / TotalTask;    //  進行度
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Scene + sceneFade.name, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                //  イメージ設定変更のプログレスバーを表示
                if (IsCanceled)
                {
                    break;
                }
                //  設定適応
                ApplySettings(sceneFade);
                Debug.Log(SetedImgMsg_Scene + sceneFade.name, sceneFade);
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
    //  シーン内のFadePlaneを検索して取得
    static GameObject[] FindFadeQuadInScene()
    {
        return Object.FindObjectsByType<IDGenerater>(FindObjectsSortMode.None)
                     .Where(FadePlane => FadePlane != null &&
                      FadePlane.CategoryAsStringProperty == IDEnumToString.ToString(IDCategory.EFFECT) &&
                      FadePlane.LabelAsStringProperty == IDEnumToString.ToString(IDLabel.FADE))
                     .Select(FadePlane => FadePlane.gameObject)
                     .ToArray();
    }
    //  Canvasに設定を適応する
    static void ApplySettings(GameObject fadePlane)
    {
        Camera cam = Camera.main;    //  メインカメラのインスタンス

        float height = cam.orthographicSize * 2f;    //  画面に映る空間の高さ
        float width = height * cam.aspect;    //  画面の幅
        const string UndoLog = "Apply Setting To FadePlane";    //  ScalerでCtrl + Zした時のログ

        Undo.RecordObject(fadePlane, UndoLog);

        fadePlane.transform.position = cam.transform.position + Camera.main.transform.forward * 1f;
        fadePlane.transform.localScale = new Vector3(width, height, 1f);   // Quad 前提
    }
}
#endif