using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ISyncEntityTransformSystem : ITickGameSystem
    {
        
    }
    
    public class SyncEntityTransformSystem : ISyncEntityTransformSystem
    {
        public int ExcuteOrder => 10;
        private readonly TickGameSystemManager tickGameSystemManager;
        private readonly IEntityContainer entityContainer;
        private readonly HashSet<uint> entityEGID = new HashSet<uint>();

        public SyncEntityTransformSystem(TickGameSystemManager _tickSystemManager, IEntityContainer _entityContainer)
        {
            tickGameSystemManager = _tickSystemManager;
            entityContainer = _entityContainer;
        }

        public void InitGameSystem()
        {
            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            entityEGID.Add(sourceEntity.EGID);
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            entityEGID.Remove(sourceEntity.EGID);
        }

        
        public void OnUpdate()
        {
            foreach (var egid in entityEGID)
            {
                if (entityContainer.GetEntity(egid, out Entity entity))
                {
                    
                }
            }
        }
    }
}
