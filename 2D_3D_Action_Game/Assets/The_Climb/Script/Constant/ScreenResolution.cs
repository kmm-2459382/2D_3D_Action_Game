//  スクリーンのデータ一覧
using UnityEngine;

public readonly struct ScreenResolution
{
    public float Width { get; }    //  横の解像度取得
    public float Height { get; }    //  縦の解像度取得

    //  スクリーンの高さと横幅のデータを入れるコンストラクタ
    public ScreenResolution(float width, float height)
    {
        Width = width;
        Height = height;
    }

    //  スクリーンの高さと横幅を出力
    public Vector2 ToVector2() => new Vector2(Width, Height);
    //  スクリーンの高さと横幅を文字列で出力
    public override string ToString() => $"{Width} x {Height}";
}