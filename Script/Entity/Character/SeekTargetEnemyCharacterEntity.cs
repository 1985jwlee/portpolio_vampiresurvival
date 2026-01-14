using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class SeekTargetEnemyCharacterEntity : EnemyCharacterEntity
    {
        protected SeekingTargetImplement seekingTargetImplement;
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
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = entityContainer.playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    if (entityContainer.GetEntity<PlayerCharacterEntity>(seekingTargetImplement.seekingTargetProperty.seekTargetEGID, out var playerCharacter))
                    {
                        TargetToMove(playerCharacter.translateImplement);
                    }
                });
        }

        private void TargetToMove(TranslateImplement targetTranslate)
        {
            var normalizeValue = (targetTranslate.positionProperty.statusValue - translateImplement.positionProperty.statusValue).normalized;
            translateImplement.moveDirectionProperty.statusValue = normalizeValue;
            rigidBodyImplement.velocity = normalizeValue * translateImplement.velocityProperty.statusValue;
        }

#region System

        protected override void Update()
        {
            base.Update();
            ResetSeekTimerForceCharging(seekingTargetImplement, knockBackReceiveImplement);
            CheckStopSeekingDiedUnit(seekingTargetImplement, statusImplement);
        }

        private static void ResetSeekTimerForceCharging(SeekingTargetImplement seekingTargetImpl, KnockBackReceiveImplement knockBackReceiveImpl)
        {
            if (knockBackReceiveImpl.knockBackResistProperty.statusValue == false && knockBackReceiveImpl.knockBackReceiveProperty.statusValue.isKnockBacking)
            {
                seekingTargetImpl.SetTimerFullCharge();
            }
        }

        public static void CheckStopSeekingDiedUnit(SeekingTargetImplement seekingTargetImpl, CommonStatusImplement statusImpl)
        {
            if (statusImpl.hitPointProperty.statusValue > 0 == false)
            {
                seekingTargetImpl.seekingTargetProperty.startSeek = false;
            }
        }
#endregion
    }
}
