using System.Collections.Generic;

namespace Game.ECS
{
    public interface IEnemyMeleeAttackSystem : ITickGameSystem
    {
        
    }
    
    public class EnemyMeleeAttackSystem : IEnemyMeleeAttackSystem
    {
        public int ExcuteOrder => 100;

        private readonly Dictionary<uint, EnemyMeleeAttackImplement> components = new Dictionary<uint, EnemyMeleeAttackImplement>(); 

        private readonly TickGameSystemManager tickGameSystemManager;

        public EnemyMeleeAttackSystem(TickGameSystemManager _gameSystemManager)
        {
            tickGameSystemManager = _gameSystemManager;
        }

        public void OnUpdate()
        {
            foreach (var kv in components)
            {
                kv.Value.OnScanTime();
            }
        }

        public void InitGameSystem()
        {
            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            if (component is EnemyMeleeAttackImplement implement)
            {
                components.Add(sourceEntity.EGID, implement);
            }
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            components.Remove(sourceEntity.EGID);
        }
    }
}
