//  イメージの名前一覧
public readonly struct ImgName
{
    public static readonly ImgName EdgeDark = new ImgName("EdgeDark");    //  端から迫る闇のイメージ
    public string Value { get; }
    //  Valueにvalueをセットするコンストラクタ
    private ImgName(string value) => Value = value;
    //  ToStringでValueを返すようにオーバーライド
    public override string ToString() => Value;
    //  ImgName型ではなくString型を返すように変更
    public static implicit operator string(ImgName Img) => Img.Value;
}
