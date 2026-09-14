using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
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

    [Header("検知範囲（半径）")]
    public float detectionRadius = 10.0f;

    private Renderer rend;
    private MaterialPropertyBlock block;
    private readonly Vector3 invalidPos = new Vector3(9999f, 9999f, 9999f);

    void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void Update()
    {
        rend.GetPropertyBlock(block);

        // コライダーの代わりにPhysics.OverlapSphereで周囲の光源を検知
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, lightLayer);

        int index = 0;
        foreach (var lightCol in hitColliders)
        {
            if (index >= maxLights) break;
            if (lightCol != null)
            {
                Vector3 pos = lightCol.bounds.center;
                block.SetVector($"_LightPos{index}", new Vector4(pos.x, pos.y, pos.z, 1f));

                SphereCollider sc = lightCol.GetComponent<SphereCollider>();
                if (sc != null)
                {
                    float outer = sc.radius * lightCol.transform.lossyScale.x;
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
}