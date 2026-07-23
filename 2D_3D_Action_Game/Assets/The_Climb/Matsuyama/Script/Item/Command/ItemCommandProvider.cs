using TheClimb.Core;
using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Item
{
    public class ItemCommandProvider    //  コマンドプロバイダー
    {
        ImpactBallContext _ctx;    //  衝撃球コンテキスト
        ItemStateFactory _stateFactory;    //  ItemのStateFacotry

        ICommandContext _commandContext;    //  コマンドコンテキスト
        IPlayerDataProvider _playerDataProvider;    //  コマンドコンテキスト
        ICorutineRunner _coroutineRunner;    //  コマンドコンテキスト

        public CountTillActivate countTillActivate {get ;}    //  アクティブになるまでカウントする
        public ExplosionInpact explosionInpact{get ;}    //  アクティブになるまでカウントする

        public ItemCommandProvider
            (ImpactBallContext ctx, IItemStateFactory stateFactory, IImpactable targetPlayer, GameObject explosionEffect, GameObject sparkingEffect, GameObject Ball)    //  コンストラクタ
        {
            _ctx = ctx;

            countTillActivate = new CountTillActivate(_ctx, stateFactory);
            explosionInpact = new ExplosionInpact(_ctx, stateFactory, targetPlayer, explosionEffect, sparkingEffect, Ball);
        }

        public void InjectContext(ICommandContext context, IItemStateContext itemStateContext)    //  コンテキスト依存注入
        {
            _commandContext = context;
            countTillActivate.InjectContext(context, itemStateContext);
            explosionInpact.InjectContext(context, itemStateContext);    
        }
    }
}