using Reflex;
using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class DummyFactory : GameObjectFactory
    {
        public DummyFactory(Container _container)
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
