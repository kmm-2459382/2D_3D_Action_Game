#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoCacheCleaner
{
    static AutoCacheCleaner()
    {
        // プレイモードの状態変化イベントを登録
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 1. プレイ開始直前（Playボタンを押した瞬間）
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            ClearCache("プレイ開始時");
        }

        // 2. プレイ終了時（Editモードに戻った瞬間）
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ClearCache("プレイ終了時");
        }
    }

    private static void ClearCache(string timingNotice)
    {
        // ① 不要な未開放アセットのメモリキャッシュをアンロード
        Resources.UnloadUnusedAssets();

        // ② ガベージコレクション（GC）の実行
        System.GC.Collect();

        // ③ Unityのダウンロード・アセット系キャッシュの削除（必要に応じて）
        // Caching.ClearCache();

        Debug.Log($"[{timingNotice}] キャッシュと不要なメモリを正常に消去しました。");
    }
}
#endif