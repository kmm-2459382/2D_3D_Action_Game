using UnityEngine;

//  地面に着地したことを通知
public class LandGroundNotifier : CollisionNotifier<ILandingHandler>
{
    CharacterGroundChecker characterGroundChecker;    //  グラウンドチェックのインスタンス
    protected override void Awake()
    {
        //  Awake処理実行
        base.Awake();
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
    }
    void OnCollisionEnter(Collision collision)
    {
        //    OnLandStage実行
        NotifyIfTagMatches(collision, TagName.Platform, h => {
            if (characterGroundChecker.CheckIsGround())
                h.OnLandStage();
        });
    }
}