using UnityEngine;
using System.Collections;

public class LightImpactCollider : MonoBehaviour
{
    [Header("床オブジェクト設定（ColliderB）")]
    public Transform floorVisual;          // 床の見た目（透明Sphereなど）

    [Header("スケール設定")]
    public float minScale = 0.0f;          // 消えた状態の大きさ
    public float maxScale = 1.5f;          // 拡大後の最大サイズ
    public float expandTime = 0.1f;        // 拡大にかかる時間
    public float stayTime = 0.0f;          // 最大サイズで維持する時間
    public float shrinkTime = 4f;        // 縮小にかかる時間

    [Header("検知レイヤー設定")]
    public LayerMask lightLayer;           // LuminaLightBall のレイヤー

    private Coroutine effectRoutine;

    private void Start()
    {
        if (floorVisual != null)
            floorVisual.localScale = Vector3.one * minScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 指定レイヤー以外は無視
        if (((1 << other.gameObject.layer) & lightLayer.value) == 0) return;

        // すでにエフェクト中なら再開（リセットではなく中断から拡大）
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(ImpactEffectRoutine());
    }

    private IEnumerator ImpactEffectRoutine()
    {
        // --- 現在スケールから拡大 ---
        float timer = 0f;
        Vector3 start = floorVisual.localScale; // ← 現在のサイズから拡大する
        Vector3 target = Vector3.one * maxScale;

        while (timer < expandTime)
        {
            timer += Time.deltaTime;
            float t = timer / expandTime;
            floorVisual.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        floorVisual.localScale = target;

        // --- 最大サイズで少し維持 ---
        yield return new WaitForSeconds(stayTime);

        // --- 縮小 ---
        timer = 0f;
        start = floorVisual.localScale; // ← 現在のサイズ（max想定）から縮小開始
        target = Vector3.one * minScale;

        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;
            float t = timer / shrinkTime;
            floorVisual.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        floorVisual.localScale = target;
        effectRoutine = null;
    }

    public void CreateImpact(Vector3 position)
    {
        // オブジェクトを指定位置に移動させてエフェクト発動
        transform.position = position;

        // 現在スケールを維持したまま再始動
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(ImpactEffectRoutine());
    }
}