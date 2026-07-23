//  タグの名前一覧
public readonly struct TagName
{
    public static readonly TagName Wall = new TagName("Wall");    //  壁のタグの名前
    public static readonly TagName Ground = new TagName("Ground");    //  地面のタグの名前
    public static readonly TagName Enemy = new TagName("Enemy");    //  敵のタグの名前
    public static readonly TagName Player = new TagName("Player");    //  プレイヤーのタグの名前
    public static readonly TagName SearchItem = new TagName("SearchItem");    //  探索アイテムのタグの名前
    public static readonly TagName Platform = new TagName("Platform");    //  探索アイテムのタグの名前
    public string Value { get; }

    //  Valueにvalueをセットするコンストラクタ
    private TagName(string value) => Value = value;
    //  ToStringでValueを返すようにオーバーライド
    public override string ToString() => Value;
    //  TagName型ではなくString型を返すように変更
    public static implicit operator string(TagName Tag) => Tag.Value;
}