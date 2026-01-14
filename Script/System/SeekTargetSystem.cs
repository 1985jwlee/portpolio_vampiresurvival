using System.Collections.Generic;

namespace Game.ECS
{
    public interface ISeekingTargetSystem : ITickGameSystem
    {
        
    }
    
    public class SeekTargetSystem : ISeekingTargetSystem
    {
        private readonly Dictionary<uint, BaseSeekTargetImplement> components = new Dictionary<uint, BaseSeekTargetImplement>(); 
        private readonly TickGameSystemManager tickGameSystemManager;
        public int ExcuteOrder => 101;
        
        
        public SeekTargetSystem(TickGameSystemManager _gameSystemManager)
        {
            tickGameSystemManager = _gameSystemManager;
        }
        
        public void InitGameSystem()
        {
            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            if (component is BaseSeekTargetImplement implement)
            {
                components.Add(sourceEntity.EGID, implement);
            }
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            components.Remove(sourceEntity.EGID);
        }

        
        public void OnUpdate()
        {
            foreach (var kv in components)
            {
                kv.Value.OnScanTime();
            }
        }
    }
}
