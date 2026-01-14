using UnityEngine;

namespace Game.ECS
{
    public class ChargeEnemyCharacterEntity : SeekTargetEnemyCharacterEntity
    {
        public ChargeEnemyPatternImplement patternImplement;
        public ChargeAttackManageImplement chargeAttackManageImplement;
        public AttackTimerManageImplement attackTimerManageImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out patternImplement))
            {
                Components.Add(patternImplement);
            }
            if (TryGetComponent(out chargeAttackManageImplement))
            {
                Components.Add(chargeAttackManageImplement);
            }
            if (TryGetComponent(out attackTimerManageImplement))
            {
                Components.Add(attackTimerManageImplement);
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdatePattern();
            SpecialEnemyFunctionalities.UpdateAttackTimer(characterDeathImplement, attackTimerManageImplement, animationImplement);
            SpecialEnemyFunctionalities.UpdateCharge(characterDeathImplement, chargeAttackManageImplement, attackTimerManageImplement, transformImplement, rigidBodyImplement, weaponFactory, 0, EGID, EnemyWeaponCreator.GetCommonBlaze());
        }

        public void UpdatePattern()
        {
            UpdatePattern(patternImplement, chargeAttackManageImplement, attackTimerManageImplement, transformImplement, seekingTargetImplement, entityContainer, 0);

            static void UpdatePattern(ChargeEnemyPatternImplement patternImpl, ChargeAttackManageImplement chargeAttackManageImpl, AttackTimerManageImplement attackTimerManageImpl, Transform transformImpl, SeekingTargetImplement seekingTargetImpl, IEntityContainer entityContainer, int timerIndex)
            {
                Vector2 selfPosition = transformImpl.position;
                var attackTimer = attackTimerManageImpl.attackTimerStatuses[timerIndex].statusValue;

                switch (attackTimer.attackTimerState)
                {
                    case AttackTimerState.Waiting:
                        {
                            var playerClosestPoint = entityContainer.playerCharacterEntity.rigidBodyImplement.ClosestPoint(selfPosition);
                            if (Vector2.Distance(selfPosition, playerClosestPoint) < patternImpl.DetectionRadiusProperty.statusValue)
                            {
                                seekingTargetImpl.seekingTargetProperty.startSeek = false;
                                SpecialEnemyFunctionalities.StartCharge(chargeAttackManageImpl, attackTimerManageImpl, timerIndex, selfPosition, entityContainer.playerCharacterEntity.transformImplement.position);
                            }
                        }
                        break;
                    case AttackTimerState.Finish:
                        {
                            seekingTargetImpl.seekingTargetProperty.startSeek = true;
                        }
                        break;
                }
            }
        }

        
    }
}
