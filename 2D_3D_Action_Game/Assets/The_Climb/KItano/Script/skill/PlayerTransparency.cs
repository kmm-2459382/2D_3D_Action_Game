using UnityEngine;
using System.Collections;

public class PlayerTransparency : MonoBehaviour
{
    [SerializeField] float transparentAlpha = 0.5f; // 半透明にする透明度
    [SerializeField] float duration = 2f;           // 戻るまでの時間

    Renderer rend;
    Material mat;
    float originalAlpha;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        originalAlpha = mat.color.a;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TransparentRoutine());
        }
    }

    IEnumerator TransparentRoutine()
    {
        // 半透明にする
        SetAlpha(transparentAlpha);

        // 時間待ち
        yield return new WaitForSeconds(duration);

        // 元の透明度に戻す
        SetAlpha(originalAlpha);
    }

    void SetAlpha(float a)
    {
        Color c = mat.color;
        c.a = a;
        mat.color = c;
    }
}
