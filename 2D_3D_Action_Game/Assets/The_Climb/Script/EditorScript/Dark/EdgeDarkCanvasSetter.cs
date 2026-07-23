#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

//  EdgeDarkCanvasのスケーリングを設定する
public class EdgeDarkCanvasSetter
{
    //  シーン内とAssets何のすべてのEdgeDarkCanvasにスケーリングの設定をする
    public static void ApplyToAllEdgeDarkCanvas(EdgeDarkCanvasSetting edgeDarkCanvasSetting)
    {
        CanvasScaler[] SceneEdgeDarkCanvasScaler = FindEdgeDarkCanvasScalersInScene();
        List<(CanvasScaler, GameObject, string)> AssetEdgeDarkCanvasScaler = LoadEdgeDarkCanvasScalerInPrefabs();
        
        int SetCnt = 0;    //  スケーリング設定回数
        int TotalTask = SceneEdgeDarkCanvasScaler.Length + AssetEdgeDarkCanvasScaler.Count;    //  設定を適応するオブジェクトの総数
        const string ProgressTitle = "EdgeDarkCanvasのスケーリング適用中";    //  プログレスバーのメッセージ
        const string ProgressMsg_Scene = "シーン:";    //  プログレスバーのシーン内設定適応中のメッセージ
        const string ProgressMsg_Asset = "アセット:";    //  プログレスバーのアセット内設定適応中のメッセージ
        const string SetedImgMsg_Scene = "✅ シーンのCanvasScalerに適応:";    //  設定適応したMsg
        const string SetedImgMsg_Asset = "✅ アセットのCanvasScalerに適応:";    //  設定適応したMsg

        try
        {
            foreach (CanvasScaler canvasScaler in SceneEdgeDarkCanvasScaler)
            {
                float progress = (float)SetCnt / TotalTask;    //  進行度
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Scene + canvasScaler.gameObject.name, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                //  イメージ設定変更のプログレスバーを表示
                if (IsCanceled)
                {
                    break;
                }
                //  スケーリング設定適応
                ApplySettings(canvasScaler, edgeDarkCanvasSetting);
                Debug.Log(SetedImgMsg_Scene + canvasScaler.gameObject.name, canvasScaler);
                SetCnt++;
            }
            foreach (var (canvasScale, PrefabRoot, Path) in AssetEdgeDarkCanvasScaler)
            {
                float progress = (float)SetCnt / TotalTask;    //  進行度
                //  イメージ設定変更のプログレスバーを表示
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Asset + Path, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                if (IsCanceled)
                {
                    break;
                }
                SetCnt++;
                //  スケーリング設定適応
                ApplySettings(canvasScale, edgeDarkCanvasSetting);
                EditorUtility.SetDirty(PrefabRoot);
                PrefabUtility.SaveAsPrefabAsset(PrefabRoot, Path);
                Debug.Log(SetedImgMsg_Asset + Path, PrefabRoot);
                PrefabUtility.UnloadPrefabContents(PrefabRoot);
            }
        }
        finally
        {
            //  プログレスバー消去
            ProgressBarUtility.Clear();
        }
        Debug.Log($"CanvasScaler設定適用完了。処理対象: {SetCnt}個のCanvas");
    }
    //  アッセット内のEdgeDarkを検索して取得
    static List<(CanvasScaler canvasScaler, GameObject PrefabRoot, string Path)> LoadEdgeDarkCanvasScalerInPrefabs()
    {
        List<(CanvasScaler, GameObject, string)> Result = new();    //  プレハブのデータリスト

        string[] Guids = AssetDatabase.FindAssets("t:Prefab");    //  プレハブのGUIDを取得
        foreach (string Guid in Guids)
        {
            string Path = AssetDatabase.GUIDToAssetPath(Guid);    //  プレハブのパスを取得
            GameObject PrefabRoot = PrefabUtility.LoadPrefabContents(Path);    //  プレハブのルートを取得s

            CanvasScaler canvasScaler = PrefabRoot.GetComponentsInChildren<CanvasScaler>(true)    //  プレハブ内の最初のEdgeDarkCanvasのScalerを取得
                                .FirstOrDefault(CanvasScaler => CanvasScaler!= null && CanvasScaler.gameObject.name == CanvasName.EdgeDarkCacvas);
            if (canvasScaler != null)
            {
                Result.Add((canvasScaler, PrefabRoot, Path));
            }
            else
            {
                PrefabUtility.UnloadPrefabContents(PrefabRoot);
            }
        }
        return Result;
    }
    //  シーン内のEdgeDarkCanvasを検索して取得
    static CanvasScaler[] FindEdgeDarkCanvasScalersInScene()
    {
        return Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None)
                     .Where(s => s != null && s.gameObject.name == CanvasName.EdgeDarkCacvas)
                     .ToArray();
    }
    //  アセット内のEdgeDarkCanvasを検索して取得
    static IEnumerable<CanvasScaler> FindEdgeDarkCanvasScalersInPrefab(GameObject prefabRoot)
    {
        return prefabRoot.GetComponentsInChildren<CanvasScaler>(true)
                         .Where(s => s != null && s.gameObject.name == CanvasName.EdgeDarkCacvas);
    }
    //  Canvasに設定を適応する
    static void ApplySettings(CanvasScaler Scaler, EdgeDarkCanvasSetting edgeDarkImgSetting)
    {
        const string UndoLog = "Apply EdgeDarckCanvas Setting";    //  ScalerでCtrl + Zした時のログ

        Undo.RecordObject(Scaler, UndoLog);

        Scaler.uiScaleMode = edgeDarkImgSetting.ScaleMode;
        Scaler.referenceResolution = AppDefault.DefaultResolution.ToVector2();
        Scaler.screenMatchMode = edgeDarkImgSetting.ScreenMatchMode;
        Scaler.matchWidthOrHeight = edgeDarkImgSetting.MatchWidthOrHeight;
    }
}
#endif