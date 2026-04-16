using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer), typeof(Collider))]
[DisallowMultipleComponent]
public class LuminaGroundController : MonoBehaviour
{
    [Header("光の検知レイヤー")]
    [Tooltip("反応させたい光源のレイヤーを設定")]
    public LayerMask lightLayer;

    [Header("光の最大数")]
    public int maxLights = 8;

    [Header("Shader用のデフォルト範囲")]
    public float defaultInnerRadius = 2.0f;
    public float defaultOuterRadius = 4.0f;
    public Color baseColor = Color.white;
    public Color emissionColor = Color.white;

    private Renderer rend;
    private MaterialPropertyBlock block;

    private Queue<Collider> lightQueue = new Queue<Collider>(); // 登録順保持
    private readonly Vector3 invalidPos = new Vector3(9999f, 9999f, 9999f);

    void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void Update()
    {
        rend.GetPropertyBlock(block);

        int index = 0;
        foreach (var light in lightQueue)
        {
            if (index >= maxLights) break;
            if (light != null)
            {
                Vector3 pos = light.bounds.center;
                block.SetVector($"_LightPos{index}", new Vector4(pos.x, pos.y, pos.z, 1f));

                // コライダーのサイズに応じて半径を設定
                SphereCollider sc = light.GetComponent<SphereCollider>();
                if (sc != null)
                {
                    float outer = sc.radius * light.transform.lossyScale.x;
                    float inner = outer * 0.5f;
                    block.SetFloat($"_OuterRadius{index}", outer);
                    block.SetFloat($"_InnerRadius{index}", inner);
                }
                else
                {
                    block.SetFloat($"_OuterRadius{index}", defaultOuterRadius);
                    block.SetFloat($"_InnerRadius{index}", defaultInnerRadius);
                }
            }
            index++;
        }

        // 残りライトは無効化
        for (int i = index; i < maxLights; i++)
        {
            block.SetVector($"_LightPos{i}", new Vector4(invalidPos.x, invalidPos.y, invalidPos.z, 1f));
            block.SetFloat($"_OuterRadius{i}", 0f);
            block.SetFloat($"_InnerRadius{i}", 0f);
        }

        block.SetColor("_BaseColor", baseColor);
        block.SetColor("_EmissionColor", emissionColor);

        rend.SetPropertyBlock(block);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & lightLayer) == 0) return;

        if (lightQueue.Contains(other)) return;

        if (lightQueue.Count >= maxLights)
        {
            Collider old = lightQueue.Dequeue();
            //Debug.Log($"[LuminaGround] Light dequeued: {old?.name}");
        }

        lightQueue.Enqueue(other);
        //Debug.Log($"[LuminaGround] Light entered: {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!lightQueue.Contains(other)) return;

        Queue<Collider> newQueue = new Queue<Collider>();
        foreach (var l in lightQueue)
        {
            if (l != other) newQueue.Enqueue(l);
        }
        lightQueue = newQueue;

        //Debug.Log($"[LuminaGround] Light exited: {other.name}");
    }
}