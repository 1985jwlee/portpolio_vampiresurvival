using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DroneAttackIndicatorEntity : Entity
    {
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;

        public Transform transformImplement;
        public SpriteRenderer circleSprite;
        public PopRandomCirclePointsImplement droneAttackIndicatorImplement;
        public TableIndexDataImplement tableIndexDataImplement;
        public SeekingTargetImplement seekingTargetImplement;
        public GameObject rangeCircle;
        public int rotateDirection;


        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            transformImplement = transform;
            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
            }
            if (TryGetComponent(out droneAttackIndicatorImplement))
            {
                Components.Add(droneAttackIndicatorImplement);
            }
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }
        
        
        
        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            seekingTargetSystem.UnRegistComponent(this);
        }

        protected override void Update()
        {
            RotateObject(transformImplement, 20f * rotateDirection);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = entityContainer.playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.seekingTime = 0f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            
            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    CheckSeekTarget(entityContainer, seekingTargetImplement, transformImplement);
                });
        }
        
        private static void CheckSeekTarget(IEntityContainer entityContainer, SeekingTargetImplement seekingTargetImpl, Transform transformImpl)
        {
            if (entityContainer.GetEntity<PlayerCharacterEntity>(seekingTargetImpl.seekingTargetProperty.seekTargetEGID, out var playerCharacter))
            {
                transformImpl.position = playerCharacter.translateImplement.positionProperty.statusValue;
            }
        }
        
        protected static void RotateObject(Transform transformImpl, float speed)
        {
            var localRot = transformImpl.localRotation.eulerAngles;
            localRot.z += Time.deltaTime * speed;
            transformImpl.localRotation = Quaternion.Euler(localRot);
        }

        public void PopRandomAttackPosition(int count)
        {
            droneAttackIndicatorImplement.PopRandomCirclePoint(rangeCircle.transform, count);
        }
    }
}
