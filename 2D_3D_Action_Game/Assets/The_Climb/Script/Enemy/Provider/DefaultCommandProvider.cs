using System.Collections.Generic;
using UnityEngine;
using static KickerMoveCommander;

public class DefaultCommandProvider : ICommandProvider
{
    readonly KickerMoveCommander _KicekrMoveCommander;

    public DefaultCommandProvider(KickerMoveCommander kickerMoveCommander)
    {
        _KicekrMoveCommander = kickerMoveCommander;
    }

    public Dictionary<KickerMoveCommander.KickerCommanderMethod, ICommand_Enemy> GetCommandMap()
    {
        return new Dictionary<KickerMoveCommander.KickerCommanderMethod, ICommand_Enemy>
        {
            {KickerCommanderMethod.MOVE, new ActionCommand(_KicekrMoveCommander.Move)},
            {KickerCommanderMethod.IS_EDGE_POS, new FuncCommand<bool>(_KicekrMoveCommander.IsEdgePos) },
            {KickerCommanderMethod.FLIP_MOVE_DIR, new ActionCommand(_KicekrMoveCommander.FlipMoveDir)},
            {KickerCommanderMethod.JUMP, new ActionCommand(_KicekrMoveCommander.Jump)},
        };
    }
}
