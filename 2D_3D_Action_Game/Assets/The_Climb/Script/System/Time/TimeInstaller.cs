using UnityEngine;
using Zenject;

//  時間系統をバインドする
public class TimeInstaller : MonoInstaller
{
    [SerializeField] private TimeSetting timeSettings;    //  時間設定

    //  時間系バインド
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TimeManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ITimeConfig>().To<TimeSettingWrapper>().AsSingle().WithArguments(timeSettings);
    }
}
//    以下コード保存所    //
//Container.Bind<TimeManager>().FromComponentInHierarchy().AsSingle();

//Container.Bind<ITimeProvider>().To<TimeManager>().AsSingle();