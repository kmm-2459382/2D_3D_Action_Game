using UnityEngine;

[ExecuteInEditMode]
public class AreaTracker : MonoBehaviour
{
    [Header("表示させたいマテリアルを複数登録")]
    public Material[] targetMaterials;

    [Header("登録がない場合は全体に送る")]
    public bool useGlobalShaderVariable = false;

    void Update()
    {
        // このオブジェクトの「ワールド座標からローカルへの変換行列」を取得
        Matrix4x4 m = transform.worldToLocalMatrix;

        // 1. リストに登録された個別のマテリアルに送る
        if (targetMaterials != null && targetMaterials.Length > 0)
        {
            foreach (Material mat in targetMaterials)
            {
                if (mat != null)
                {
                    mat.SetMatrix("_AreaWorldToLocal", m);
                }
            }
        }

        // 2. もしフラグがONなら、プロジェクト全体のシェーダーにも送る
        if (useGlobalShaderVariable)
        {
            Shader.SetGlobalMatrix("_AreaWorldToLocal", m);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(0, 1, 1, 0.1f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}