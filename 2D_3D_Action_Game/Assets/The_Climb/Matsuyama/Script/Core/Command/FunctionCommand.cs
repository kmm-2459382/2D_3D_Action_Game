using System;

namespace TheClimb.Astral
{
    public abstract class FuncCommand<T> : IFunctionCommand<T>
    {
        Func<T> _func;    //  アクションコマンド

        //  コンストラクタ
        protected FuncCommand(Func<T> func)
        {
            _func = func;
        }

        //  関数の返り値を取得
        public abstract T Execute();    //  => _func();
    }
}