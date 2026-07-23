using System.Collections;
using UnityEngine;
using TheClimb.Core;
using TheClimb.Logging;

namespace TheClimb.Item
{
    public class CountTillActivate : ItemCommandBase   //  爆発するまでカウントする
    {
        ImpactBallContext _ctx;                //  設定データ
        ImpactBallRuntimeData _runtimeData;    //  実行データ(キャッシュ用)

        IItemStateFactory _itemStateFactory;    //  ItemのStateFacotry
        ICommandContext _commandContext;    //  コマンドコンテキスト
        ICorutineRunner _coroutineRunner;    //  コルーチンランナー
        IItemStateContext _ItemStateContext;

        public CountTillActivate(ImpactBallContext ctx, IItemStateFactory stateFactory)    //  コンストラクタ
        {
            _ctx = ctx;
            _runtimeData = ctx.RuntimeData;

            _itemStateFactory = stateFactory;
        }

        public void InjectContext(ICommandContext commandContext, IItemStateContext itemStateContext)    //  コンテキスト注入
        {
            _commandContext = commandContext;
            _ItemStateContext = itemStateContext;
        }

        public override void Execute()    //  カウント開始
        {
            LogUtility.Log(LogPrefix.countTillActivate, "爆発タイマーカウント開始", LogLevel.Debug);
            //_coroutineRunner.StartCoroutine(CountTillExplosion());
            ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(CountTillExplosion());
        }

        IEnumerator CountTillExplosion()    //  爆発するまでカウントするコルーチン
        {
            while ((_runtimeData._RemainingFuseTime -= Time.deltaTime) > 0)
            {
                yield return null;
            }
            _commandContext.ChangeState(_itemStateFactory.CreateState(ItemStateID.Expolosing), _ItemStateContext);    //  爆発stateに変更
        }
    }
}