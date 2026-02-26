using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AreaManager : MonoBehaviour
{
    [Header("このエリア情報を適用するマテリアル")]
    public Material[] targetMaterials;

    // シーン内のAreaTrackerを自動収集するためのリスト
    private static List<Transform> _areaTransforms = new List<Transform>();

    public static void Register(Transform t) { if (!_areaTransforms.Contains(t)) _areaTransforms.Add(t); }
    public static void Unregister(Transform t) { _areaTransforms.Remove(t); }

    void Update()
    {
        // 有効なエリア情報を収集
        int count = Mathf.Min(_areaTransforms.Count, 8); // 最大8個まで
        Matrix4x4[] matrices = new Matrix4x4[count];

        for (int i = 0; i < count; i++)
        {
            matrices[i] = _areaTransforms[i].worldToLocalMatrix;
        }

        // 全マテリアルに一括送信
        foreach (Material mat in targetMaterials)
        {
            if (mat != null)
            {
                mat.SetMatrixArray("_AreaMatrices", matrices);
                mat.SetInt("_AreaCount", count);
            }
        }
    }
}