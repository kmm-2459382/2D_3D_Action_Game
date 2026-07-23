#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

//  闇の設定をする
static public class EdgeDarkImgSetter
{
    //  シーンとアセットの全てのEdgeDarkに設定を適応する
    public static void ApplyToAllEdgeDarkImg(EdgeDarkImgSetting edgeDarkImgSetting)
    {
        //  シーン内のEdgeDarkを検索して取得
        Image[] SceneEdgeDarkImg = FindEdgeDarkImagesInScene();
        //  アセット内のEdgeDarkを検索して取得
        List<(Image, GameObject, string)> AssetEdgeDarkImg = LoadEdgeDarkImagesInPrefabs();

        int SetCount = 0;    //  セットした回数値
        int TotalTask = SceneEdgeDarkImg.Length + AssetEdgeDarkImg.Count;    //  設定を適応するオブジェクトの総数
        const string ProgressTitle = "EdgeDark(Image)のスケーリング適用中";    //  プログレスバーのメッセージ
        const string ProgressMsg_Scene = "シーン:";    //  プログレスバーのシーン内設定適応中のメッセージ
        const string ProgressMsg_Asset = "アセット:";    //  プログレスバーのアセット内設定適応中のメッセージ
        const string SetedImgMsg_Scene = "✅ シーンのEdgeDark(Image)に適応:";    //  設定適応したMsg
        const string SetedImgMsg_Asset = "✅ アセットのCanvasScaler(Image)に適応:";    //  設定適応したMsg

        try
        {
            // シーン内のImageを検索して設定適用
            foreach (Image image in SceneEdgeDarkImg)
            {
                float progress = (float)SetCount / TotalTask;    //  進行度
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Scene + image.gameObject.name, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                //  イメージ設定変更のプログレスバーを表示
                if (IsCanceled)
                {
                    break;
                }
                System.Threading.Thread.Sleep(500);    //  デバッグのために500ミリ秒(0.5秒)停止
                //  設定を適応
                ApplySettings(image, edgeDarkImgSetting);
                Debug.Log(SetedImgMsg_Scene + image.gameObject.name, image);
                SetCount++;
            }
            foreach (var (img, PrefabRoot, Path) in AssetEdgeDarkImg)
            {
                float progress = (float)SetCount / TotalTask;    //  進行度
                bool IsCanceled = ProgressBarUtility.ShowCancelable(ProgressTitle, ProgressMsg_Asset + Path, progress);    //  キャンセルボタンが押されるかどうかのフラグ
                //  イメージ設定変更のプログレスバーを表示
                if (IsCanceled)
                {
                    break;
                }

                //  設定を適応
                ApplySettings(img, edgeDarkImgSetting);
                EditorUtility.SetDirty(img);
                PrefabUtility.SaveAsPrefabAsset(PrefabRoot, Path);
                Debug.Log(SetedImgMsg_Asset + Path);
                SetCount++;
                PrefabUtility.UnloadPrefabContents(PrefabRoot);
            }
            Debug.Log($"Image設定適用完了。処理対象: {SetCount}個のImage");
        }
        finally
        {
            ProgressBarUtility.Clear();
        }
    }
    //  アッセット内のEdgeDarkを検索して取得
    static List<(Image img, GameObject PrefabRoot, string Path)> LoadEdgeDarkImagesInPrefabs()
    {
        List<(Image, GameObject, string)> Result = new();    //  プレハブのデータリスト

        string[] Guids = AssetDatabase.FindAssets("t:Prefab");    //  プレハブのGUIDを取得
        foreach (string Guid in Guids)
        {
            string Path = AssetDatabase.GUIDToAssetPath(Guid);    //  プレハブのパスを取得
            GameObject PrefabRoot = PrefabUtility.LoadPrefabContents(Path);    //  プレハブのルートを取得

            Image img = PrefabRoot.GetComponentsInChildren<Image>(true)    //  プレハブ内の最初のEdgeDarkイメージを取得
                                .FirstOrDefault(Img => Img != null && Img.gameObject.name == ImgName.EdgeDark);
            if (img != null)
            {
                Result.Add((img, PrefabRoot, Path));
            }
            else
            {
                PrefabUtility.UnloadPrefabContents(PrefabRoot);
            }
        }
        return Result;
    }
    //  シーン内のEdgeDarkを取得
    static Image[] FindEdgeDarkImagesInScene()
    {
        return Object.FindObjectsByType<Image>(FindObjectsSortMode.None)
                     .Where(Img => Img != null && Img.gameObject.name == ImgName.EdgeDark)
                     .ToArray(); ;
    }
    //  アセット内のEdgeDarkを取得
    static Image FindEdgeDarkImagesInPrefab(GameObject prefabRoot)
    {
        return prefabRoot.GetComponentsInChildren<Image>(true)
                         .FirstOrDefault(img => img != null && img.gameObject.name == ImgName.EdgeDark);
    }
    //  設定を適応する
    static void ApplySettings(Image img, EdgeDarkImgSetting edgeDarkImgSetting)
    {
        RectTransform rt = img.GetComponent<RectTransform>();    //  EdgeRackのReactTransformを取得

        const string UndoLog_img = "Apply EdgeDarcImg Setting";    //  EdgeDarkのimgでCtrl + Zした時のログ
        const string UndoLog_rt = "Apply EdgeDarc RectTransoform Setting";    //  EdgeDarkのrtでCtrl + Zした時のログ

        Undo.RecordObject(img, UndoLog_img);
        Undo.RecordObject(rt, UndoLog_rt);

        rt.anchorMin = edgeDarkImgSetting.AnchorMin;
        rt.anchorMax = edgeDarkImgSetting.AnchorMax;

        rt.anchoredPosition = edgeDarkImgSetting.AnchoredPosition;
        rt.sizeDelta = edgeDarkImgSetting.SizeDelta;

        rt.pivot = edgeDarkImgSetting.Pivot;
        img.raycastTarget = edgeDarkImgSetting.RaycastTarget;
    }
}
#endif