using System.Linq;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class EnemyMeleeAttackImplement : MonoBehaviour, IComponent, ITickComponent
    {
        [Inject] private IEntityContainer entityContainer;
        
        private bool isAttackAvailable;
        private bool isFirstAttack;

        private EnemyCharacterEntity attackEntity;
        
        public float fixedTickTime => 0.3f;
        public float currentTickTime { get; set; }
        
        
        public void InitializeComponent()
        {
            currentTickTime = 0f;
            isFirstAttack = false;
            attackEntity = GetComponent<EnemyCharacterEntity>();
        }
        public bool OnScanTime()
        {
            if (isAttackAvailable)
            {
                if (isFirstAttack == false)
                {
                    Attack();
                    isFirstAttack = true;
                    return true;
                }
                
                currentTickTime += Time.deltaTime;

                if (currentTickTime > fixedTickTime)
                {
                    currentTickTime -= fixedTickTime;
                    Attack();
                    return true;
                }
            }

            return false;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out PlayerCharacterEntity _))
            {
                isAttackAvailable = true;
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!isAttackAvailable)
            {
                return;
            }

            if (col.TryGetComponent(out PlayerCharacterEntity _))
            {
                isAttackAvailable = false;
            }
        }
        
        private void Attack()
        {
            var applyBuffImpl = attackEntity.applyBuffImplement;
            var playerCharacter = entityContainer.playerCharacterEntity;
                
            if (playerCharacter != null && playerCharacter.statusImplement.hitPointProperty.statusValue > 0)
            {
                if (playerCharacter.buffImplement.appliedBuff.Any(_x => _x.statusValue.rootEntityId == attackEntity.EGID) == false)
                {
                    var dmgValue = applyBuffImpl.applyBuffList[0].statusValue.buffValue;
                    playerCharacter.buffImplement.appliedBuff.Add(applyBuffImpl.applyBuffList[0]);
                    playerCharacter.hitTintImplement.hitTintTriggerProperty.statusValue = true;
                    var reflectDmg = (int)(playerCharacter.characterStatusImplement.buffedcounterAttackMultipleProperty.statusValue * dmgValue);
                    if (reflectDmg > 0)
                    {
                        attackEntity.buffImplement.appliedBuff.Add(new Buff()
                        {
                            statusValue = new BuffData()
                            {
                                buffType = BuffType.Damage, remainTime = -1f, rootEntityId = playerCharacter.EGID, buffValue = reflectDmg
                            }
                        });
                    }
                }
            }
        }
    }
}
