using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class RecieveAttackableColliderImplement : MonoBehaviour, IComponent
    {   
        [Inject] private IEntityContainer entityContainer;
        [Inject] private ISpearAttackSystem spearAttackSystem;
        [Inject] private IDealingDamageAchivementSystem dealingDamageAchivementSystem;
        [Inject] private IDealingDamageSpecificDeviceAchievementSystem dealingDamageSpecificDeviceAchievementSystem;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<AttackEntity>(out var attackEntity) == false)
            {
                return;
            }
            
            var applyBuffImpl = attackEntity.applyBuffImplement;

            for (int i = 0; i < applyBuffImpl.applyBuffList.Count; ++i)
            {
                // if (attackEntity.EGID == applyBuffImpl.applyBuffList[i].statusValue.rootEntityId)
                // {
                //     continue;
                // }
                var applyBuff = applyBuffImpl.applyBuffList[i];
                switch (applyBuff.statusValue.buffType)
                {
                    case BuffType.Damage:
                        {
                            var critical = attackEntity.weaponDataSetImplement.weaponDataSetProperty.statusValue;
                            var isCritical = entityContainer.playerCharacterEntity.characterStatusImplement.buffedaddCriticalRatioProperty.statusValue + critical.criticalProbability > Random.Range(0f, 1f);
                            if (isCritical)
                            {
                                var tmp = applyBuff.statusValue;
                                tmp.buffType = BuffType.CriticalDamage;
                                tmp.buffValue *= (critical.criticalRatio);
                                applyBuff.statusValue = tmp;
                            }
                        }
                        break;
                    case BuffType.MagicDamage:
                        {
                            var tmp = applyBuff.statusValue;
                            tmp.buffValue *= (1f + entityContainer.playerCharacterEntity.characterStatusImplement.buffedaddMagicRatioProperty.statusValue);
                            applyBuff.statusValue = tmp;
                        }
                        break;
                }
                
                switch (applyBuffImpl.applyBuffList[i].statusValue.rootCharacter.statusValue)
                {
                    case CharacterTypes.PlayerCharacter:
                        {
                            {
                                if (TryGetComponent(out EnemyCharacterEntity characterEntity))
                                {
                                    if (characterEntity.statusImplement.hitPointProperty.statusValue > 0)
                                    {
                                        if (characterEntity.waitbuffTickImplement.HasWaitBuffTick(attackEntity.EGID))
                                        {
                                            continue;
                                        }
                                        characterEntity.buffImplement.appliedBuff.Add(applyBuff);
                                        characterEntity.hitTintImplement.hitTintTriggerProperty.statusValue = true;
                                        characterEntity.knockBackReceiveImplement.knockBackReceiveProperty.statusValue = attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue.CalcKnockBackReceiveDataSet(
                                            entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue,
                                            attackEntity.translateImplement.positionProperty.statusValue,
                                            attackEntity.rigidBodyImplement.velocity,
                                            characterEntity.translateImplement.positionProperty.statusValue
                                        );

                                        dealingDamageAchivementSystem.reactiveProperty.Value += (int)applyBuff.statusValue.buffValue;
                                        dealingDamageSpecificDeviceAchievementSystem.reactiveCommand.Execute((attackEntity.weaponDataSetImplement.weaponDataSetProperty.statusValue.id, (int)applyBuff.statusValue.buffValue));
                                        
                                        spearAttackSystem.receiveEntityEGID.Execute(attackEntity.EGID);
                                    }
                                }
                            }

                            {
                                if (TryGetComponent(out ItemBoxEntity itemBoxEntity))
                                {
                                    if (itemBoxEntity.waitbuffTickImplement.HasWaitBuffTick(attackEntity.EGID))
                                    {
                                        continue;
                                    } 
                                    itemBoxEntity.buffImplement.appliedBuff.Add(applyBuff);
                                    spearAttackSystem.receiveEntityEGID.Execute(attackEntity.EGID);
                                }
                            }
                        }
                        break;
                    case CharacterTypes.Enemy:
                        {
                            if (TryGetComponent(out PlayerCharacterEntity characterEntity))
                            {
                                if (characterEntity.statusImplement.hitPointProperty.statusValue > 0)
                                {
                                    if (characterEntity.waitbuffTickImplement.HasWaitBuffTick(attackEntity.EGID))
                                    {
                                        continue;
                                    }
                                    characterEntity.hitTintImplement.hitTintTriggerProperty.statusValue = true;
                                    characterEntity.buffImplement.appliedBuff.Add(applyBuff);
                                
                                    spearAttackSystem.receiveEntityEGID.Execute(attackEntity.EGID);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void InitializeComponent()
        {
            
        }
    }
}
