using Reflex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game.ECS
{
    public class InteractableFactory : GameObjectFactory
    {
        public InteractableFactory(Container _container)
        {
            container = _container;
        }

        protected override void OnCreateGameObject(Entity o, int pathHash)
        {
            o.SrcPathHashCode = pathHash;
            o.OnCreateEntity();
        }
    }
}
