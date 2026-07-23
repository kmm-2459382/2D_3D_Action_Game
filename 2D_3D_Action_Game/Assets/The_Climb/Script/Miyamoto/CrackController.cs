using UnityEngine;

public class CrackController : MonoBehaviour
{
    public Material mat;
    public float speed = 0.3f;

    float amount = 0;

    void Update()
    {
        amount += Time.deltaTime * speed;
        mat.SetFloat("_CrackAmount", amount);
    }
}