using System.Collections.Generic;
using UniRx;

namespace Game.ECS
{
    public interface IGameSystem
    {
        void InitGameSystem();
        void RegistComponent(IComponent component, IEntity sourceEntity);
        void UnRegistComponent(IEntity sourceEntity);
    }

    public interface ITickGameSystem : IGameSystem
    {
        int ExcuteOrder { get; }
        void OnUpdate();
    }

    public interface IReactValueGameSystem<T> : IGameSystem, System.IDisposable
    {
        IReactiveProperty<T> reactiveProperty { get; }
    }
    
    public interface IReactCmdGameSystem<T> : IGameSystem, System.IDisposable
    {
        IReactiveCommand<T> reactiveCommand { get; }
    }

    public interface IOneWayReactCmdGameSystem : IGameSystem, System.IDisposable
    {
        IReactiveCommand<uint> receiveEntityEGID { get; }
    }

    public interface IOneWayReactCmdGameSystem<T> : IGameSystem, System.IDisposable
    {
        IReactiveCommand<(uint sender, T value)> receiveEntityEGID { get; }
    }

    public interface ITwoWayReactCmdGameSystem : IGameSystem, System.IDisposable
    {
        IReactiveCommand<(uint sender, uint reciever)> receiveEntityEGID { get; }
    }

    public interface ITwoWayReactCmdGameSystem<T> : IGameSystem, System.IDisposable
    {
        IReactiveCommand<(uint sender, uint receiver, T value)> receiveEntityEGID { get; }
    }
    
    

    public class TickGameSystemManager
    {
        private readonly SortedList<int, ITickGameSystem> tickGameSystems = new SortedList<int, ITickGameSystem>();

        public void AddTickSystem(ITickGameSystem system)
        {
            tickGameSystems.Add(system.ExcuteOrder, system);
        }

        public void UpdateSystem()
        {
            foreach (var systems in tickGameSystems)
            {
                systems.Value.OnUpdate();
            }
        }
        
    }
}
