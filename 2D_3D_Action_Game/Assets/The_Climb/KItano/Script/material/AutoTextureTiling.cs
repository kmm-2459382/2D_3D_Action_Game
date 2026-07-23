using UnityEngine;

[ExecuteAlways]
public class AutoTextureTiling : MonoBehaviour
{
    private Renderer rend;

    [Header("Texture Settings")]
    [SerializeField] private float textureDensity = 1f;
    [SerializeField] private bool useXZ = true;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateTiling();
    }

    void Update()
    {
        UpdateTiling();
    }

    void UpdateTiling()
    {
        if (rend == null) return;

        Vector3 scale = transform.localScale;

        Vector2 tiling;

        if (useXZ)
        {
            tiling = new Vector2(scale.x, scale.z) * textureDensity;
        }
        else
        {
            tiling = new Vector2(scale.x, scale.y) * textureDensity;
        }

        rend.sharedMaterial.mainTextureScale = tiling;
    }
}