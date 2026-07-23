using UnityEngine;

public class BrickWallBuilder : MonoBehaviour
{
    [Header("レンガ設定")]
    public GameObject brickPrefab;

    [Header("塔の構造")]
    public int bricksPerLayer = 122;         // 1段のレンガ数（戸数）
    public int heightLayers = 120;           // 縦の段数
    public float radius = 20f;               // 半径
    public float verticalSpacing = 0.39f;    // 上下の間隔
    public float totalAngle = 360f;          // 囲む角度（例：360で円、180で半円）

    [Header("塔の中心位置（レンガの中心基準）")]
    public Vector3 baseCenterPosition = new Vector3(0f, -20f, 0f); // 塔の一番下段の中心位置

    void Start()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("brickPrefab が設定されていません！");
            return;
        }

        BuildWall();
    }

    void BuildWall()
    {
        float angleStep = totalAngle / bricksPerLayer;

        for (int y = 0; y < heightLayers; y++)
        {
            float currentHeight = baseCenterPosition.y + y * verticalSpacing;

            // 偶数段は半分ずらしてレンガ積みに
            float angleOffset = (y % 2 == 1) ? angleStep / 2f : 0f;

            for (int i = 0; i < bricksPerLayer; i++)
            {
                float angleDeg = -totalAngle / 2f + i * angleStep + angleOffset;
                float angleRad = angleDeg * Mathf.Deg2Rad;

                // 塔の中心位置からオフセットして配置
                Vector3 position = new Vector3(
                    baseCenterPosition.x + Mathf.Cos(angleRad) * radius,
                    currentHeight,
                    baseCenterPosition.z + Mathf.Sin(angleRad) * radius
                );

                Quaternion rotation = Quaternion.Euler(0, -angleDeg, 0);

                Instantiate(brickPrefab, position, rotation, transform);
            }
        }
    }
}
