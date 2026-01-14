using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class ArcaneCycleEntity : AttackEntity
    {
        public GameObject srcArcaneChild;
        public SeekingTargetImplement seekingTargetImplement;
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
            }
            
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            seekingTargetSystem.UnRegistComponent(this);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.seekingTime = -1f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            transformImplement.localRotation = Quaternion.identity;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    CheckSeekTarget(entityContainer, seekingTargetImplement, transformImplement);
                });
        }

        protected override void Update()
        {
            base.Update();
            RotateObject(transformImplement,  20f * weaponDataSetImplement.weaponDataSetProperty.statusValue.projectileSpeed * entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue);
        }
        
        private static void CheckSeekTarget(IEntityContainer entityContainer, SeekingTargetImplement seekingTargetImpl, Transform transformImpl)
        {
            if (entityContainer.GetEntity<PlayerCharacterEntity>(seekingTargetImpl.seekingTargetProperty.seekTargetEGID, out var playerCharacter))
            {
                transformImpl.localPosition = playerCharacter.translateImplement.positionProperty.statusValue;
            }
        }

        public override void OnApplyShootCountChanged()
        {
            var index = shootCounterImplement.shootCountProperty.statusValue;
            var count = shootCounterImplement.maxShootCountProperty.statusValue;
            srcArcaneChild.transform.localPosition = GetChildLocalPosition(count, index - 1, 1.8f);
        }

        protected static Vector3 GetChildLocalPosition(int count, int index, float range)
        {
            var needAngle = 360f / count * index;
            var radianAngle = needAngle * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radianAngle);
            var sin = Mathf.Sin(radianAngle);
            return new Vector3(cos, sin, 0f) * range;
        }
    }
}
