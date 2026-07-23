
using UnityEngine;

public class LightColorToggle : MonoBehaviour
{
    [SerializeField] private Light targetLight; // 対象のライト
    [SerializeField] private Color normalColor = Color.white; // 通常の色
    [SerializeField] private Color blacklightColor = new Color(0.5f, 0f, 1f); // 紫っぽい色
    private bool isBlacklight = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isBlacklight = !isBlacklight;
            targetLight.color = isBlacklight ? blacklightColor : normalColor;
        }
    }
}
