#if UNITY_EDITOR
using UnityEditor;

//  プログレスバー表示
public static class ProgressBarUtility
{
    //  プログレスバーを表示
    public static void Show(string title, string message, float progress)
    {
        EditorUtility.DisplayProgressBar(title, message, progress);
    }
    //  キャンセルボタン付きプログレスバーを表示
    public static bool ShowCancelable(string title, string message, float progress)
    {
        return EditorUtility.DisplayCancelableProgressBar(title, message, progress);
    }
    //  プログレスバーを消去
    public static void Clear()
    {
        EditorUtility.ClearProgressBar();
    }
}
#endif