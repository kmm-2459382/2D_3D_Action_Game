using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Item
{
    public class ComnadBus : MonoBehaviour, ICommandBus    //  コマンドバス
    {
        Queue<IItemCommand> _queue = new Queue<IItemCommand>();

        public void Enqueue(IItemCommand command)
        {
            _queue.Enqueue(command);
        }

        void Update()
        {
            while (_queue.Count > 0)
            {
                IItemCommand command = _queue.Dequeue();
                command.Execute();     // ← ここで実行される
            }
        }
    }
}