#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class FadeQuadEditWindow : EditorWindow
{
    [MenuItem("Tools/Fade Tool")]
    //  エディタウィンドウを表示
    public static void ShowWindow()
    {
        GetWindow<FadeQuadEditWindow>("Fade Tool");
    }
    void OnGUI()
    {
        GUILayout.Label("Fade Utility", EditorStyles.boldLabel);

        if (GUILayout.Button("Apply To Setting in FadeQuad."))
        {
            try
            {
                FadeSetter.ApplyToSceneFadeQuad();    //  四隅からくる闇のイメージを設定をシーンとアセット内に適用
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ FadeQuadImage設定適用時に例外発生: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
#endif
