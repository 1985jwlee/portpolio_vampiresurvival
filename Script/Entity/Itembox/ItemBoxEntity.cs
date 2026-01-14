using System.Runtime.CompilerServices;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class ItemBoxEntity : Entity
    {
        [Inject] private ItemBoxFactory itemBoxFactory;
        [Inject] private TableDataHolder tableDataHolder;
        [Inject] private TokenFactory tokenFactory;

        public Rigidbody2D rigidbodyImplement;
        public Transform transformImplement;
        public TranslateImplement translateImplement;
        public BuffImplement buffImplement;
        public WaitBuffTickImplement waitbuffTickImplement;
        public OnKilledEventImplement onKilledEventImplement;


        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            transformImplement = transform;

            TryGetComponent(out rigidbodyImplement);

            if (TryGetComponent(out translateImplement))
            {
                Components.Add(translateImplement);
            }
            if (TryGetComponent(out waitbuffTickImplement))
            {
                Components.Add(waitbuffTickImplement);
            }
            if (TryGetComponent(out buffImplement))
            {
                Components.Add(buffImplement);
            }
            if (TryGetComponent(out onKilledEventImplement))
            {
                Components.Add(onKilledEventImplement);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            rigidbodyImplement.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            rigidbodyImplement.constraints = RigidbodyConstraints2D.FreezeRotation;

            foreach (var id in onKilledEventImplement.itemIdsToDrop)
            {
                tableDataHolder.ItemCollection.TryGetEntity(id.statusValue.ToString(), out var itemDataEntity);

                var tokenGo = tokenFactory.CreateGameObject(itemDataEntity.prefabPath);
                var tokenEntity = tokenGo.GetComponent<TokenEntity>();
                tokenEntity.transformImplement.position = transformImplement.position + (Vector3)Random.insideUnitCircle;
            }
        }

        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
            ApplyBuffValue(buffImplement);
        }

        protected static void SyncTransform(Transform transformImpl, TranslateImplement translateImple)
        {
            translateImple.positionProperty.statusValue = transformImpl.localPosition;
            translateImple.rotationProperty.statusValue = transformImpl.localRotation;
            translateImple.scaleProperty.statusValue = transformImpl.localScale;
        }

        private void ApplyBuffValue(BuffImplement buffImpl)
        {
            buffImpl.CheckBuffCooldown();
            buffImpl.BuffImpl(out var buffDatas);

            for (int i = 0; i < buffDatas.Count; ++i)
            {
                switch (buffDatas[i].buffType)
                {
                    case BuffType.Damage:
                    case BuffType.CriticalDamage:
                    case BuffType.MagicDamage:
                        {
                            itemBoxFactory.EnqueRecycle(this, SrcPathHashCode);
                        }
                        break;
                }
            }
        }
    }
}
