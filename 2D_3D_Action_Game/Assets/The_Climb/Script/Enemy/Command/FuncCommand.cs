using System;

//  返り値ありの関数実行クラス
public class FuncCommand<T> : ICommand_Enemy
{
    Func<T> _Func;    //  

    //  コンストラクタ
    public FuncCommand(Func<T> Func)
    {
        _Func = Func;
    }

    public object Execute() => _Func();    //  関数の返り値を取得
}
