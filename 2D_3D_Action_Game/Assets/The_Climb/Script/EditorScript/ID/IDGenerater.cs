using UnityEngine;

[RequireComponent(typeof(ObjectRegistry))]
//  オブジェクトID自動生成
[ExecuteInEditMode]
public class IDGenerater : MonoBehaviour
{
    [SerializeField] private IDCategory Category = IDCategory.UNDEFINED;   // 種類例：Player, Enemy
    [SerializeField] private IDLabel Label = IDLabel.UNDNAMED;        // 意味例：Main, Boss
    [SerializeField] private string ID = "";            // 自動生成されるID

    public IDCategory CategoryProperty => Category;    //  カテゴリープロパティ
    public IDLabel LabelProperty => Label;    //  ラベルプロパティ

    public string CategoryAsStringProperty => IDEnumToString.ToString(Category);    //  カテゴリプロパティ(文字列出力)
    public string LabelAsStringProperty => IDEnumToString.ToString(Label);    //  ラベルプロパティ(文字列出力)
    public string IDProperty => ID;    //  IDプロパティ
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = $"{CategoryAsStringProperty}_{LabelAsStringProperty}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
//  以下コード保存所  //
//if (string.IsNullOrEmpty(ID))    IDがなかったら実行
//{
//    ID = $"{CategoryProperty}_{LabelProperty}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";
//#if UNITY_EDITOR
//    UnityEditor.EditorUtility.SetDirty(this);
//#endif
//}
//[SerializeField, HideInInspector] private IDCategory prevCategory = IDCategory.UNDEFINED;    //  前回のカテゴリーを保持
//[SerializeField, HideInInspector] private IDLabel prevLabel = IDLabel.UNDNAMED;    //  前回のラベルを保持

//public IDCategory CategoryProperty => Category;    //  カテゴリープロパティ
//public IDLabel LabelProperty => Label;    //  ラベルプロパティ

//public string CategoryAsStringProperty => IDEnumToString.ToString(Category);    //  カテゴリプロパティ(文字列出力)
//public string LabelAsStringProperty => IDEnumToString.ToString(Label);    //  ラベルプロパティ(文字列出力)
//public string IDProperty => ID;    //  IDプロパティ
//private void OnValidate()
//{
//    // カテゴリーかラベルが変わったときと両方未定義の時IDを再生成
//    if (Category != prevCategory || Label != prevLabel || (Category == IDCategory.UNDEFINED && Label == IDLabel.UNDNAMED))
//    {
//        ID = $"{CategoryAsStringProperty}_{LabelAsStringProperty}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";

//        // 前回値を更新
//        prevCategory = Category;
//        prevLabel = Label;
//#if UNITY_EDITOR