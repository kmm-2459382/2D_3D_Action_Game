using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Core
{
    public abstract class PlayerAPIFacadeBase    //  イベント時に差し替えるための抽象化
    {
        protected PlayerPhysics playerPhysics;    //  プレイヤーの物理処理関数保持クラス
        protected PlayerPhysicsJudge judge;    //  物理挙動関数を使っていい状況か判断するクラス


        public PlayerAPIFacadeBase(PlayerPhysics physics, PlayerPhysicsJudge judge)
        {
            playerPhysics = physics;
            this.judge = judge;
        }

        public abstract void AddForce(Vector3 force, AddForceMode mode);    //  RBのAddForceを使うクラス
    }
}