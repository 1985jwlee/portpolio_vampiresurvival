using JetBrains.Annotations;
using Reflex.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.ECS
{
    public class ShieldEntity : ArcaneDeviceSummonEntity
    {
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        public SeekingTargetImplement seekingTargetImplement;
        public Rigidbody2D rigidBodyImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            seekingTargetImplement = GetComponent<SeekingTargetImplement>();
            rigidBodyImplement = GetComponent<Rigidbody2D>();

            Components.Add(seekingTargetImplement);
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = OwnerEGID;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    if (entityContainer.GetEntity<CharacterEntity>(seekingTargetImplement.seekingTargetProperty.seekTargetEGID, out var characterEntity))
                    {
                        transformImplement.localPosition = characterEntity.translateImplement.positionProperty.statusValue;
                    }
                });
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            seekingTargetSystem.UnRegistComponent(this);
        }

        protected override void Update()
        {
            base.Update();
            RotateByOwnerDirection(transformImplement, entityContainer, OwnerEGID);
        }

        private static void RotateByOwnerDirection(Transform transformImpl, IEntityContainer entityContainer, uint OwnerEGID)
        {
            if(entityContainer.GetEntity<CharacterEntity>(OwnerEGID, out var owner))
            {
                var ownerDirection = owner.translateImplement.moveDirectionProperty.statusValue;

                transformImpl.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(ownerDirection.x, ownerDirection.y, 0));
            }
        }
    }
}
