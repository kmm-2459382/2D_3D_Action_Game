using System.Collections.Generic;
using UnityEngine;

public class LuminaTerrainManager : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundLayer;   // Inspectorで「Ground」を選択
    [SerializeField] private LayerMask triggerLayer;  // Inspectorで「LuminaAppearerTrigger」を選択

    // 内部で保持するGroundオブジェクトのデータ構造
    private class GroundData
    {
        public MeshFilter filter;
        public MeshCollider collider;
        public Vector3[] baseVertices;
        public int[] baseTriangles;
        public Mesh generatedMesh;
    }

    private List<GroundData> groundList = new List<GroundData>();
    private List<SphereCollider> triggerList = new List<SphereCollider>();

    // メッシュ再構築用のデータバッファ（使い回してGCを防止）
    private List<Vector3> extractedVertices = new List<Vector3>();
    private List<int> extractedTriangles = new List<int>();
    private Dictionary<int, int> vertexIndexMap = new Dictionary<int, int>();

    void Start()
    {
        InitializeObjects();
    }

    /// <summary>
    /// シーン内の Ground と Trigger を自動集積して初期化
    /// </summary>
    public void InitializeObjects()
    {
        groundList.Clear();
        triggerList.Clear();

        // 1. 全 Ground オブジェクトのメッシュ情報をキャッシュ
        MeshFilter[] filters = FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
        foreach (var filter in filters)
        {
            if (((1 << filter.gameObject.layer) & groundLayer) != 0)
            {
                if (filter.sharedMesh == null) continue;

                MeshCollider col = filter.GetComponent<MeshCollider>();
                if (col == null) col = filter.gameObject.AddComponent<MeshCollider>();

                // 初期状態のメッシュコライダーを完全に消去する
                col.sharedMesh = null;

                Mesh genMesh = new Mesh();
                genMesh.MarkDynamic(); // 動的書き換えの最適化

                groundList.Add(new GroundData
                {
                    filter = filter,
                    collider = col,
                    baseVertices = filter.sharedMesh.vertices,
                    baseTriangles = filter.sharedMesh.triangles,
                    generatedMesh = genMesh
                });
            }
        }

        // 2. 全 Trigger オブジェクト（SphereCollider）を取得
        SphereCollider[] colliders = FindObjectsByType<SphereCollider>(FindObjectsSortMode.None);
        foreach (var col in colliders)
        {
            if (((1 << col.gameObject.layer) & triggerLayer) != 0)
            {
                triggerList.Add(col);
            }
        }
    }

    void Update()
    {
        UpdateAllColliders();
    }

    /// <summary>
    /// すべての Ground に対して、全 Trigger の範囲を考慮して MeshCollider を更新
    /// </summary>
    private void UpdateAllColliders()
    {
        foreach (var ground in groundList)
        {
            extractedVertices.Clear();
            extractedTriangles.Clear();
            vertexIndexMap.Clear();

            Matrix4x4 localToWorld = ground.filter.transform.localToWorldMatrix;
            Matrix4x4 worldToLocal = ground.filter.transform.worldToLocalMatrix;

            // ポリゴン（三角面）ごとに判定
            for (int i = 0; i < ground.baseTriangles.Length; i += 3)
            {
                int i0 = ground.baseTriangles[i];
                int i1 = ground.baseTriangles[i + 1];
                int i2 = ground.baseTriangles[i + 2];

                Vector3 w0 = localToWorld.MultiplyPoint3x4(ground.baseVertices[i0]);
                Vector3 w1 = localToWorld.MultiplyPoint3x4(ground.baseVertices[i1]);
                Vector3 w2 = localToWorld.MultiplyPoint3x4(ground.baseVertices[i2]);
                Vector3 polyCenter = (w0 + w1 + w2) / 3f;

                // いずれか「1つでも」Triggerの範囲内に入っているかチェック
                bool isInsideAnyTrigger = false;
                foreach (var trigger in triggerList)
                {
                    if (trigger == null || !trigger.enabled) continue;

                    Vector3 triggerCenter = trigger.transform.position;
                    // スケールを考慮した球体の実際の半径
                    float scaledRadius = trigger.radius * Mathf.Max(
                        trigger.transform.lossyScale.x,
                        trigger.transform.lossyScale.y,
                        trigger.transform.lossyScale.z
                    );

                    if (Vector3.Distance(polyCenter, triggerCenter) <= scaledRadius)
                    {
                        isInsideAnyTrigger = true;
                        break; // 1つでも範囲内なら確定
                    }
                }

                // 範囲内ならメッシュに追加
                if (isInsideAnyTrigger)
                {
                    AddVertexToMesh(i0, w0, worldToLocal);
                    AddVertexToMesh(i1, w1, worldToLocal);
                    AddVertexToMesh(i2, w2, worldToLocal);
                }
            }

            // メッシュの適用
            ground.generatedMesh.Clear();

            if (extractedTriangles.Count > 0)
            {
                // 範囲内にポリゴンが存在する場合のみ、メッシュとColliderを適用
                ground.generatedMesh.SetVertices(extractedVertices);
                ground.generatedMesh.SetTriangles(extractedTriangles, 0);

                // バウンディングボックス（境界）の再計算
                ground.generatedMesh.RecalculateBounds();

                // 物理判定の即時ベイク（遅延による判定残り防止）
                Physics.BakeMesh(ground.generatedMesh.GetInstanceID(), false);

                ground.collider.sharedMesh = null; // 更新通知のため一時リセット
                ground.collider.sharedMesh = ground.generatedMesh;
            }
            else
            {
                // 範囲内にポリゴンが無い（非表示領域）場合は、Colliderを完全に空（null）にする
                ground.collider.sharedMesh = null;
            }
        }
    }

    private void AddVertexToMesh(int originalIndex, Vector3 worldPos, Matrix4x4 worldToLocal)
    {
        if (!vertexIndexMap.TryGetValue(originalIndex, out int newIndex))
        {
            newIndex = extractedVertices.Count;
            extractedVertices.Add(worldToLocal.MultiplyPoint3x4(worldPos));
            vertexIndexMap.Add(originalIndex, newIndex);
        }
        extractedTriangles.Add(newIndex);
    }
}