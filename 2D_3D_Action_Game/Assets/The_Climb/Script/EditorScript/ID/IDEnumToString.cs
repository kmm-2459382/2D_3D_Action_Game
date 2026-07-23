using System.Collections.Generic;

//  IDに使うカテゴリーやベルどの列挙型をストリングに変換
public class IDEnumToString
{
    private static readonly Dictionary<IDCategory, string> IDCategoryMap = new()    //  ID生成に使うカテゴリーマップ
    {
        { IDCategory.UNDEFINED, "Undefined" },
        { IDCategory.EFFECT, "Effect" },
        { IDCategory.PLAYER, "Player" },
    };
    private static readonly Dictionary<IDLabel, string> IDLabelMap = new()    //  ID生成に使うラベルマップ
    {
        { IDLabel.UNDNAMED, "Unnamed" },
        { IDLabel.DARK, "Dark" },
        { IDLabel.SPINE, "Spine" },
        { IDLabel.FADE, "Fade" },
    };
    //  IDカテゴリーに応じて辞書型からキーを出力
    public static string ToString(IDCategory category) =>
        IDCategoryMap.TryGetValue(category, out var result) ? result : "Undefined";

    //  IDラベルに応じて辞書型からキーを出力
    public static string ToString(IDLabel label) =>
        IDLabelMap.TryGetValue(label, out var result) ? result : "Unnamed";
}
