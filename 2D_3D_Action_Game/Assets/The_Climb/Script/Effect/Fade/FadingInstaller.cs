using Zenject;

public class FadingInstalelr : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDownFading>().To<FadeContoroller>().FromComponentInHierarchy().AsSingle();
    }
}
