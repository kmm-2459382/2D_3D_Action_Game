using Zenject;

public class PlayerInstaller : MonoInstaller
{
    //  プレイヤー関係のバインド
    public override void InstallBindings()
    {
        Container.Bind<PlayerState>().FromComponentInHierarchy().AsSingle();
    }
}
