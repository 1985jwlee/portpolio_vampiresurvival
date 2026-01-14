using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class SeekingEnemyCircleAttackIndicatorEntity : EnemyAttackIndicatorEntity
    {
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;

        public SeekingTargetImplement seekingTargetImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            seekingTargetImplement = GetComponent<SeekingTargetImplement>();

            Components.Add(seekingTargetImplement);

            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    CheckSeekTarget(entityContainer, seekingTargetImplement, transformImplement);
                });
        }
        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            seekingTargetSystem.UnRegistComponent(this);
        }

        private static void CheckSeekTarget(IEntityContainer entityContainer, SeekingTargetImplement seekingTargetImpl, Transform transformImpl)
        {
            if (entityContainer.GetEntity<CharacterEntity>(seekingTargetImpl.seekingTargetProperty.seekTargetEGID, out var characterEntity))
            {
                transformImpl.localPosition = characterEntity.translateImplement.positionProperty.statusValue;
            }
        }
    }
}
