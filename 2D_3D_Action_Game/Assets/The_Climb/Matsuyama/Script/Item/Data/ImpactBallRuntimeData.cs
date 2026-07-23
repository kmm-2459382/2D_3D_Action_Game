namespace TheClimb.Item
{
    public class ImpactBallRuntimeData    //  衝撃球実行中データ
    {
        public float _RemainingFuseTime;    //  爆発までの残り時間
        public float _RemainingExplosionTime;        //  爆発持続時間

        public ImpactBallRuntimeData(ImpactBallConfigSO configSO)    //  コンフィグからの初期値取得
        {
            _RemainingFuseTime = configSO.FuseTime;
            _RemainingExplosionTime = configSO.ExplosionDuration;
        }
    }
}