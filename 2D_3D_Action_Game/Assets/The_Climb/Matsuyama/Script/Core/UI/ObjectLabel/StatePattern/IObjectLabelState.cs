using UnityEngine;

namespace TheClimb.Core
{
    public interface IObjectLabelState    //  オブジェクトラベル状態インターフェース
    {
        void Enter();    //  State突入時の関数
        
        void Update();    //  State中の関数

        void Exit();    //  Stateを抜けるタイミングの関数
    }
}