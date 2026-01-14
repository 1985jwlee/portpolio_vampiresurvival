using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public interface IEntityCollection
    {
        bool TryGetEntity(string key, out IEntity output);
        List<IEntity> GetIterator();
    }
    
    public interface IEntityCollection<T> : IEntityCollection where T : IEntity
    {
        bool TryGetEntity(string key, out T output);
    }

    public class EntityCollectionImpl<T> : IEntityCollection<T> where T : IEntity
    {
        protected readonly Dictionary<string, T> entities = new();

        public bool TryGetEntity(string key, out T output)
        {
            if (entities.TryGetValue(key, out output))
            {
                return true;
            }
            return false;
        }

        public List<T> GetIterator()
        {
            return entities.Select(_x => _x.Value).ToList();
        }
        
        bool IEntityCollection.TryGetEntity(string key, out IEntity output)
        {
            TryGetEntity(key, out var castedOuput);
            output = castedOuput;
            return output != null;
        }

        List<IEntity> IEntityCollection.GetIterator()
        {
            var ret = new List<IEntity>();
            foreach (var e in GetIterator())
            {
                if (e is IEntity entity)
                {
                    ret.Add(entity);
                }
            }
            return ret;
        }
    }

    public class DualEntityCollectionImpl<T> : IEntityCollection<T> where T : IEntity
    {
        protected Dictionary<(string, string), T> entities = new ();

        public bool TryGetEntity(string key, out T output)
        {
            var mainKeySel = entities.ToDictionary(_key => _key.Key.Item1, _val => _val.Value);
            if (mainKeySel.TryGetValue(key, out output))
            {
                return true;
            }

            var subKeySel = entities.Where(_x => string.IsNullOrEmpty(_x.Key.Item2) == false && _x.Key.Item2.Equals(key)).Select(_x => _x.Value).ToList();
            if (subKeySel.Count > 0)
            {
                
                output = Game.ECS.ExtensionFunction.Random(subKeySel);
                return true;
            }

            return false;
        }

        public List<T> GetIterator()
        {
            return entities.Select(_x => _x.Value).ToList();
        }
        
        bool IEntityCollection.TryGetEntity(string key, out IEntity output)
        {
            TryGetEntity(key, out var castedOuput);
            output = castedOuput;
            return output != null;
        }

        List<IEntity> IEntityCollection.GetIterator()
        {
            var ret = new List<IEntity>();
            foreach (var e in GetIterator())
            {
                if (e is IEntity entity)
                {
                    ret.Add(entity);
                }
            }
            return ret;
        }
    }
}
