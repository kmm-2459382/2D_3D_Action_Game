using UnityEngine;
namespace TheClimb.Item
{
    public interface ICommandBus    //  コマンドバスInterface
    {
        void Enqueue(IItemCommand command);
    }
}
