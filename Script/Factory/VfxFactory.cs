using Reflex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class VfxFactory : GameObjectFactory
    {
        public VfxFactory(Container _container)
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
