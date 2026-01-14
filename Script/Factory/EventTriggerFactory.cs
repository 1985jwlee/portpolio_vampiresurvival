using Cysharp.Threading.Tasks;
using Reflex;
using Reflex.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class EventTriggerFactory : GameObjectFactory
    {
        public EventTriggerFactory(Container _container)
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
