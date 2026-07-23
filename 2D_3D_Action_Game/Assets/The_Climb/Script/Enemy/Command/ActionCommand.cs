using System;

//  返り値なしの関数実行クラス
public class ActionCommand : ICommand_Enemy
{
    Action _Action;    //  関数保有変数

    //  コンストラクタ
    public ActionCommand(Action Action)
    {
        _Action = Action;
    }
    //  関数実行
    public object Execute()
    {
        _Action();
        return null;
    }
}
