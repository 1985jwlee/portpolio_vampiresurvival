using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.ECS
{
    public interface IEntity 
    {
        public uint EGID { get; set; }
        public ICollection<IComponent> Components { get; }
    }
    
    public interface IEntityContainer : IDisposable
    {
        PlayerCharacterEntity playerCharacterEntity { get; }
        
        void AddEntityPull(IEntity entity);
        void RemoveEntityPull(IEntity entity);
        List<T> GetEntities<T>() where T : IEntity;
        List<T> GetEntities<T, C>() where T : IEntity where C : IComponent;
        bool GetEntity<T>(uint egid, out T o) where T : IEntity;
        bool GetEntity<T, C>(uint egid, out T o, out C c) where T : IEntity where C : IComponent;
    }

    public class EntityContainer : IEntityContainer
    {
        private uint entityCount = 0;
        private readonly Dictionary<uint, IEntity> entityPull = new Dictionary<uint, IEntity>();
        private readonly Queue<uint> removedEGID = new Queue<uint>();

        private PlayerCharacterEntity pcEntity;

        public PlayerCharacterEntity playerCharacterEntity
        {
            get
            {
                if (pcEntity != null)
                {
                    return pcEntity;
                }
                pcEntity = GetEntities<PlayerCharacterEntity>().First();
                return pcEntity;
            }
        }

        public void AddEntityPull(IEntity entity)
        {
            entity.EGID = removedEGID.Count > 0 ? removedEGID.Dequeue() : entityCount++;
            entityPull.Add(entity.EGID, entity);
        }

        public void RemoveEntityPull(IEntity entity)
        {
            entity.Components.Clear();
            entityPull.Remove(entity.EGID);
            removedEGID.Enqueue(entity.EGID);
        }

        public void Dispose()
        {
            foreach (var e in entityPull)
            {
                e.Value.Components.Clear();
            }
            entityPull.Clear();
            removedEGID.Clear();
            entityCount = 0;
        }

        public List<T> GetEntities<T>() where T : IEntity
        {
            var ret = new List<T>();

            foreach (var kv in entityPull)
            {
                if (kv.Value is T entity)
                {
                    ret.Add(entity);
                }
            }

            return ret;
        }

        public List<T> GetEntities<T,C>() where T : IEntity where C : IComponent
        {
            var ret = new List<T>();
            foreach (var kv in entityPull)
            {
                if (kv.Value is T entity && entity.Components.Select(_x => _x.GetType()).Contains(typeof(C)))
                {
                    ret.Add(entity);
                }
            }
            
            return ret;
        }

        public bool GetEntity<T>(uint egid, out T o) where T : IEntity
        {
            o = default;
            if (entityPull.TryGetValue(egid, out var entity) == false || entity is not T output)
            {
                return false;
            }
            o = output;
            return true;
        }

        public bool GetEntity<T, C>(uint egid, out T o, out C c) where T : IEntity where C : IComponent
        {
            o = default;
            c = default;
            
            if (GetEntity(egid, out o))
            {
                foreach (var comp in o.Components)
                {
                    if (comp is C typeC)
                    {
                        c = typeC;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
    }
    
}
