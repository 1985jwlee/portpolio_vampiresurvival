using System.Collections.Generic;
using Reflex;
using UnityEngine;


namespace Game.ECS
{
    public abstract class GameObjectFactory
    {
        private readonly Dictionary<int, Entity> gameObjectPool = new Dictionary<int, Entity>();
        private readonly Dictionary<int, Queue<Entity>> recycleGameObject = new Dictionary<int, Queue<Entity>>();

        protected Container container;


        protected abstract void OnCreateGameObject(Entity o, int pathHash);

        public void EnqueRecycle(Entity go, int pathHash)
        {
            if (recycleGameObject.TryGetValue(pathHash, out var queue) == false) {return;}
            go.OnRemoveEntity();
            queue.Enqueue(go);
            go.gameObject.SetActive(false);
        }

        public GameObject CreateGameObject(string resourcePath)
        {
            var pathHashCode = resourcePath.GetHashCode();
            if (recycleGameObject.ContainsKey(pathHashCode) == false)
            {
                recycleGameObject.Add(pathHashCode, new Queue<Entity>());
            }

            Entity o = null;

            if (recycleGameObject.TryGetValue(pathHashCode, out var queue))
            {
                if (queue.Count > 0)
                {
                    o = queue.Dequeue();
                    CreateEntity(o, pathHashCode);
                    return o.gameObject;
                }
            }

            if (gameObjectPool.TryGetValue(pathHashCode, out o) == false)
            {
                o = Resources.Load<Entity>(resourcePath);
                if (o == null)
                {
                    return null;
                }
                gameObjectPool.Add(pathHashCode, o);
            }
            var inst = container.Instantiate(o);
            CreateEntity(inst, pathHashCode);
            return inst.gameObject;
        }

        private void CreateEntity(Entity inst, int pathHashCode)
        {
            OnCreateGameObject(inst, pathHashCode);
            inst.InitializeComponents();
            inst.ApplyComponentData();
            inst.gameObject.SetActive(true);
        }
    }
}
