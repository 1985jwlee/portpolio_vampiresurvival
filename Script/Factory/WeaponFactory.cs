using Reflex;
using UnityEngine;

namespace Game.ECS
{
    public class WeaponFactory : GameObjectFactory
    {
        public WeaponFactory(Container _container)
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
