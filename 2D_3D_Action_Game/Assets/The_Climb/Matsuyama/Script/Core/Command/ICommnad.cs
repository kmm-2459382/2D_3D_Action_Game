namespace TheClimb.Astral
{
    //  ICommnadのベースインターフェース
    public interface ICommandBase
    {

    }

    public interface IActionCommand : ICommandBase
    {
        void Execute();
    }

    public interface IFunctionCommand<T> : ICommandBase
    {
        T Execute();
    }
}