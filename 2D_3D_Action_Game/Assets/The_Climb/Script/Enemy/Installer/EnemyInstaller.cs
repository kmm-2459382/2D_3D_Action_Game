using UnityEngine;
using Zenject;

//  雑魚敵インストーラー
public class EnemyInstaller : MonoInstaller
{
    [SerializeField] KickerStatus kickerStatus;    //  キッカーのステータス
    //  バインド処理
    public override void InstallBindings()
    {
        Container.Bind<KickerMoveCommander>().FromComponentInHierarchy().AsSingle();
        Container.Bind<KickerStatus>().FromScriptableObject(kickerStatus).AsCached();
    }
}
