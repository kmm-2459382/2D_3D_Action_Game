using UnityEngine;

//  闇の進行度を調整する
public class DarknessController : MonoBehaviour
{
    [SerializeField] EdgeDarkStatus edgeDarkStatus;    //  闇のステータス
    EdgeDarkStatBlock edgeDarkStatBlock;    //  闇のステータスクラス
    Material OverLayMaterial;    //  四隅から進行してくる闇のマテリアル

    [Header("闇進行度")]
    [SerializeField, Range(0, 1)] float CurrentProgress;    //  闇進行度
    float CurrentProgressRate_Sec;    //  秒間の闇進行速度(割合値)

    void Awake()
    {
        edgeDarkStatBlock = edgeDarkStatus.GetStats(FloorType.NOMAL_FLOOR);
        OverLayMaterial = edgeDarkStatBlock.OverLayMaterial;

        //  初期化
        Initialize();
    }
    void Update()
    {
        //  闇を時間経過で進行
        ProgressEdgeDark();
    }
    //  初期化
    void Initialize()
    {
        CurrentProgress = edgeDarkStatBlock.Progress;
        CurrentProgressRate_Sec = edgeDarkStatBlock.DarkProgressRate_Sec;
    }
    //  闇を時間経過で進行
    void ProgressEdgeDark()
    {
        CurrentProgress -= CurrentProgressRate_Sec * Time.deltaTime;
        CurrentProgress = Mathf.Clamp01(CurrentProgress);
        OverLayMaterial.SetFloat("_Progress", CurrentProgress);
    }
}