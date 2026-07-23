using UnityEngine;
using Zenject;

public class SunInstaller : MonoInstaller
{
    [SerializeField] SunStat sunStat;    //  太陽のステータス

    //  太陽関係のバインド
    public override void InstallBindings()
    {
        Container.Bind<SunMoveCommander>().FromComponentInHierarchy().AsSingle();
        Container.BindInstance(sunStat).AsSingle();
        Container.Bind<SunMover>().FromComponentInHierarchy().AsSingle();
    }
}
