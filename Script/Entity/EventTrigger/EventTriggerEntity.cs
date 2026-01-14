using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Game.ECS
{
    public class EventTriggerEntity : Entity
    {
        [Inject] DummyFactory dummyFactory;
        [Inject] EventTriggerFactory eventTriggerFactory;

        public Transform transformImplement;
        public BossSpawnImplement bossSpawnImplement;
        public EnemySpawnImplement enemySpawnImplement;
        public DummySpawnImplement dummySpawnImplement;
        public DummyManageImplement dummyManageImplement;
        public EventTriggerColliderImplement eventTriggerColliderImplement;
        public BoxCollider2D boxCollider;
        public CircleCollider2D circleCollider;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            transformImplement = transform;
            TryGetComponent(out boxCollider);
            TryGetComponent(out circleCollider);

            if (TryGetComponent(out bossSpawnImplement))
            {
                Components.Add(bossSpawnImplement);
            }

            if (TryGetComponent(out enemySpawnImplement))
            {
                Components.Add(enemySpawnImplement);
            }

            if (TryGetComponent(out dummySpawnImplement))
            {
                Components.Add(dummySpawnImplement);
            }

            if (TryGetComponent(out dummyManageImplement))
            {
                Components.Add(dummyManageImplement);
            }

            if(TryGetComponent(out eventTriggerColliderImplement))
            {
                Components.Add(eventTriggerColliderImplement);
            }
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            foreach (var egid in dummyManageImplement.spawnedDummyEgids)
            {
                if (entityContainer.GetEntity<DummyEntity>(egid.statusValue, out var dummyEntity))
                {
                    dummyFactory.EnqueRecycle(dummyEntity, dummyEntity.SrcPathHashCode);
                }
            }
            dummyManageImplement.spawnedDummyEgids.Clear();
        }
        protected override void Update()
        {
            base.Update();
            RemovedIfTriggered(this, eventTriggerColliderImplement, eventTriggerFactory);
        }

        private static void RemovedIfTriggered(EventTriggerEntity eventTriggerEntity, EventTriggerColliderImplement eventTriggerColliderImplement, EventTriggerFactory eventTriggerFactory)
        {
            if (eventTriggerColliderImplement.ShouldRemoveAfterTriggeredProperty.statusValue && eventTriggerColliderImplement.IsTriggeredProperty.statusValue)
            {
                eventTriggerFactory.EnqueRecycle(eventTriggerEntity, eventTriggerEntity.SrcPathHashCode);
            }
        }
    }
}
