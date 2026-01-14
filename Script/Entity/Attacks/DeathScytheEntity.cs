using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DeathScytheEntity : AttackEntity
    {
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        public SeekingTargetImplement seekingTargetImplement;
        public DelayAttackSecondImplement delayAttackImplement;
        public Animator animatorImplement;
        private static readonly int Wait = Animator.StringToHash("Wait");
        private float randomAngle;
        
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            
            TryGetComponent(out animatorImplement);

            if (TryGetComponent(out delayAttackImplement))
            {
                Components.Add(delayAttackImplement);
            }
            
            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
            }
            
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }
        
        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            animatorImplement.ResetTrigger(Wait);
            animatorImplement.enabled = false;
            seekingTargetSystem.UnRegistComponent(this);
        }


        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            randomAngle = Random.Range(0f, 360f);
            animatorImplement.enabled = true;
            animatorImplement.SetTrigger(Wait);
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.position = playerCharacterEntity.translateImplement.positionProperty.statusValue;
            transformImplement.rotation = Quaternion.Euler(0f, 0f, randomAngle);
            
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.seekingTime = -1f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    CheckSeekTarget(entityContainer, seekingTargetImplement, transformImplement);
                });
            
            delayAttackImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    seekingTargetImplement.seekingTargetProperty.startSeek = false;
                });

        }
        
        private static void CheckSeekTarget(IEntityContainer entityContainer, SeekingTargetImplement seekingTargetImpl, Transform transformImpl)
        {
            if (entityContainer.GetEntity<CharacterEntity>(seekingTargetImpl.seekingTargetProperty.seekTargetEGID, out var playerCharacter))
            {
                transformImpl.position = playerCharacter.translateImplement.positionProperty.statusValue;
            }
        }
    }
}
