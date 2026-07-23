namespace TheClimb.Astral
{
    public interface IAttractable
    {
        AttractTargetStatusBlock statProperty { get; }    //  ターゲットステータスブロック
        AttractTargetStateID currentStateIDProperty { get; }    //  ターゲットステータスブロック

        void OnAttract();    //  引き寄せがスタートした瞬間の処理
    }
}