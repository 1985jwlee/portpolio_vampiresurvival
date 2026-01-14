using UniRx;

namespace Game.ECS
{
    public interface IComponent
    {
        void InitializeComponent();
    }

    public interface ITickComponent
    {
        float fixedTickTime { get; }
        float currentTickTime { get; set; }
        bool OnScanTime();
    }

    public interface IReactivePropertyComponent<T>
    {
        IReactiveProperty<T> reactiveProperty { get; }
    }

    public interface IReactiveCommandComponent<T>
    {
        IReactiveCommand<T> reactiveCommand { get; }
    }

    public interface IObserveComponent<T> where T : IComponent
    {
        
    }
}
