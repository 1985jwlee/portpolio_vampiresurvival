using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class DummyEntity : Entity
    {
        public Transform transformImplement;
        public SpriteRenderer spriteRendererImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            transformImplement = transform;
            TryGetComponent(out spriteRendererImplement);
        }
    }
}
