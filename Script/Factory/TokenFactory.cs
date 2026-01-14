using System.Collections;
using System.Collections.Generic;
using Reflex;
using UnityEngine;

namespace Game.ECS
{
    public class TokenFactory : GameObjectFactory
    {
        public TokenFactory(Container _container)
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
