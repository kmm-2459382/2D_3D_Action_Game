using UnityEngine;

namespace TheClimb.Core
{
    [System.Serializable]
    public struct EffectKey    //  エフェクト特定の為のKey
    {

        //  --  外部API

        [SerializeField] private GameMode _gameMode;     //  ゲームモード
        [SerializeField] private EffectKind _effectkind;   //  エフェクトの種類

        public EffectKey(GameMode gameMode, EffectKind effectKind)    //  コンストラクタ
        {
            _gameMode = gameMode;
            _effectkind = effectKind;
        }

        public override bool Equals(object obj)    //  等価比較演算子
        => obj is EffectKey other
        && _gameMode == other._gameMode
        && _effectkind == other._effectkind;
    }
}