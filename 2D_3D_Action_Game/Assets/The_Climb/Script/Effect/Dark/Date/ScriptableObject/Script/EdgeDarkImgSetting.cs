using UnityEngine;
[CreateAssetMenu(fileName = "EdgeDarkImgSetting", menuName = "GameData/Dark/EdgeDarkImgSetting")]
public class EdgeDarkImgSetting : ScriptableObject
{
    [Header("Image RectTransform Settings")]
    public Vector2 AnchorMin = Vector2.zero;    //  Imageの最小アンカー値
    public Vector2 AnchorMax = Vector2.one;    //  Imageの最大アンカー値
    public Vector2 AnchoredPosition = Vector2.zero;    //  Imageの最小アンカー値
    public Vector2 SizeDelta = Vector2.zero;    //  Imageのサイズ値
    public Vector2 Pivot = new Vector2(0.5f, 0.5f);    //  Imageのピボット値

    [Header("Image Settings")]
    public bool RaycastTarget = false;    //  ImageのRaycastTargetにするかどうか
}
