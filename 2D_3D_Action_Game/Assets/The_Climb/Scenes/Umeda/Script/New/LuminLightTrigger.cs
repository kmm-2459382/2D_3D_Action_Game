using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LuminaLightTrigger : MonoBehaviour
{
    public Material targetMaterial; // 光を当てたい地面のマテリアル
    public float lightRadius = 3f;  // 光の半径

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground")) // 地面タグ
        {
            // 光の中心をシェーダーに渡す
            Vector3 lightPos = transform.position;
            targetMaterial.SetVector("_LightPos", new Vector4(lightPos.x, lightPos.y, lightPos.z, 1));
            targetMaterial.SetFloat("_LightRadius", lightRadius);
        }
    }
}