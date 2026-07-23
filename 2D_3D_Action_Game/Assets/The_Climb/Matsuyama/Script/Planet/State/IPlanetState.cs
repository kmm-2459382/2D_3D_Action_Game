using UnityEngine;

namespace TheClimb.Astral
{
    public interface IPlanetState    //  プラネットステートインターフェ－ス
    {
        void Enter();     //  状態に突入したとき
        void Update();    //  状態中のずっとする処理
        void Exit();      //  状態を抜ける時
    }

    //public abstract class PlanetState : IPlanetState    DRY的にいいかなと思って実装しようとしたけど、思ったより使い勝手が悪そうだったからコメントアウト
    //{
    //    PlanetMover _mover;                        //  プラネット移動クラス
    //    PlanetCommandProvider _commandProvider;    //  コマンドプロバイダー
    //    Transform _transform;                      //  プラネットトランスフォーム

    //    protected PlanetState(PlanetMover planetMover, PlanetCommandProvider planetCommandProvider, Transform transform)    //  コンストラクタ
    //    {
    //        _mover = planetMover;
    //        _commandProvider = planetCommandProvider;
    //        _transform = transform;
    //    }

    //    public abstract void Enter();
    //    public abstract void Update();
    //    public abstract void Exit();
    //}
}