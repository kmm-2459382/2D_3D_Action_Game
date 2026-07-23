using System.Collections.Generic;
//    コマンド型辞書提供プロバイダー

public interface ICommandProvider
{
    Dictionary<KickerMoveCommander.KickerCommanderMethod, ICommand_Enemy> GetCommandMap();
}