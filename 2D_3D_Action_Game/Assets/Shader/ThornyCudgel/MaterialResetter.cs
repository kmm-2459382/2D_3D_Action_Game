using UnityEngine;
using UnityEditor;

public class MaterialResetter : Editor
{
    [MenuItem("Tools/Reset Selected Materials Keywords")]
    public static void ResetKeywords()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is Material mat)
            {
                // マテリアルにこびりついた全キーワードを削除
                foreach (string keyword in mat.shaderKeywords)
                {
                    mat.DisableKeyword(keyword);
                }
                EditorUtility.SetDirty(mat);
                Debug.Log($"{mat.name} のキーワードをリセットしました");
            }
        }
    }
}