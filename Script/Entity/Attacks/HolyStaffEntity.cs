using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class HolyStaffEntity : AttackEntity
    {
        public SeekingTargetImplement seekingTargetImplement;
        public RefreshSingleInstanceAttackImplement refreshSingleInstanceAttackImplement;
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            
            seekingTargetImplement = GetComponent<SeekingTargetImplement>();
            refreshSingleInstanceAttackImplement = GetComponent<RefreshSingleInstanceAttackImplement>();
            
            Components.Add(seekingTargetImplement);
            Components.Add(refreshSingleInstanceAttackImplement);
            
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
                transformImpl.localPosition = playerCharacter.translateImplement.positionProperty.statusValue;
            }
        }
    }
}
