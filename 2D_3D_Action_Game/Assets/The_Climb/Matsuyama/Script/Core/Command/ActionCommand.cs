using System;

namespace TheClimb.Astral
{
    public abstract class ActionCommand : IActionCommand    //  返り値なしの関数
    {
        Action _action;    //  アクションコマンド

        //  コンストラクタ
        protected ActionCommand(Action action)
        {
            _action = action;
        }

        protected void InvokeAction()    //  渡された関数実行
        {
            _action.Invoke();
        }

        //  渡された関数実行の指示出し関数
        public abstract void Execute();
        //{
        //    InvokeAction();
        //}
    }
}