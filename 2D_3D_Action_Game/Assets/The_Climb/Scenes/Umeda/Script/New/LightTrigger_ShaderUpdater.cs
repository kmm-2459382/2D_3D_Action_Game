using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LightTrigger_ShaderUpdater : MonoBehaviour
{
    public string groundTag = "Ground";
    public float outerRadius = 3f;
    public float innerRadius = 0f;
    public float softness = 0.2f;
    public Color emission = Color.white;

    // shader property IDs for speed
    int idLightPos, idOuter, idInner, idSoft, idEmission;

    void Awake()
    {
        idLightPos = Shader.PropertyToID("_LightPos");
        idOuter = Shader.PropertyToID("_OuterRadius");
        idInner = Shader.PropertyToID("_InnerRadius");
        idSoft = Shader.PropertyToID("_Softness");
        idEmission = Shader.PropertyToID("_EmissionColor");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(groundTag))
        {
            Renderer rend = other.GetComponent<Renderer>();
            if (rend == null) return;

            // Optional: Raycast down to find exact surface point if ground isn't flat
            Vector3 lightWorldPos = transform.position;

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);

            mpb.SetVector(idLightPos, new Vector4(lightWorldPos.x, lightWorldPos.y, lightWorldPos.z, 1f));
            mpb.SetFloat(idOuter, outerRadius);
            mpb.SetFloat(idInner, innerRadius);
            mpb.SetFloat(idSoft, softness);
            mpb.SetColor(idEmission, emission);

            rend.SetPropertyBlock(mpb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(groundTag))
        {
            Renderer rend = other.GetComponent<Renderer>();
            if (rend == null) return;

            // Clear: ”¼Œa‚ð0‚É‚·‚é‚È‚Ç
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);

            mpb.SetFloat(idOuter, 0f);
            rend.SetPropertyBlock(mpb);
        }
    }
}
