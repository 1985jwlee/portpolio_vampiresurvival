using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public interface IRemoveLifeOverAttackSystem : ITickGameSystem
    {
        
    }
    
    public class RemoveLifeOverAttackSystem : IRemoveLifeOverAttackSystem
    {
        public int ExcuteOrder => 1000;

        private readonly WeaponFactory weaponFactory;
        private readonly IEntityContainer entityContainer;
        private readonly TickGameSystemManager tickGameSystemManager;
        private Dictionary<uint, AttackLifeTimeImplement> components = new Dictionary<uint, AttackLifeTimeImplement>();
        

        public RemoveLifeOverAttackSystem(TickGameSystemManager _gameSystemManager, IEntityContainer _entityContainer, WeaponFactory _weaponFactory)
        {
            tickGameSystemManager = _gameSystemManager;
            weaponFactory = _weaponFactory;
            entityContainer = _entityContainer;
        }

        public void InitGameSystem()
        {
            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            if (component is AttackLifeTimeImplement comp)
            {
                components.Add(sourceEntity.EGID, comp);    
            }
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            components.Remove(sourceEntity.EGID);
        }

        
        public void OnUpdate()
        {
            for (int i = components.Count - 1; i >= 0; --i)
            {
                var elem = components.ElementAt(i);
                if (elem.Value.OnScanTime())
                {
                    if (entityContainer.GetEntity<Entity>(elem.Key, out var entity))
                    {
                        weaponFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                    }
                }
            }
        }
    }
}
