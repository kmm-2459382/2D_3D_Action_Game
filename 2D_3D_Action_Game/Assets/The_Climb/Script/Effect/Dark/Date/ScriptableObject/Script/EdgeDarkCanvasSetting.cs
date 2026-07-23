using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "EdgeDarkCanvasSetting", menuName = "GameData/Dark/EdgeDarkCanvasSetting")]
public class EdgeDarkCanvasSetting : ScriptableObject
{
    [Header("CanvasScaler Settings")]
    public CanvasScaler.ScaleMode ScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;    //  CanvasScalerのモード
    
    public Vector2 ReferenceResolution = new Vector2(1920, 1080);    //  CanvasScalerのReferenceResolutionの値

    [Range(0f, 1f)]
    public float MatchWidthOrHeight = 0.5f;    //  縦横どちらに合わせるかの比重値
    
    public CanvasScaler.ScreenMatchMode ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;    //  CanvasScalerのScreenMatchModeを変更
}
