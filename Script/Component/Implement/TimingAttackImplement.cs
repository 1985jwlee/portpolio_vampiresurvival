using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class TimingAttackImplement : MonoBehaviour, IComponent, ITickComponent
    {
        [Inject] private IEntityContainer entityContainer;
        [SerializeField] private CharacterTypes applyableCharacter;
        private readonly HashSet<uint> targetEntities = new HashSet<uint>();

        public float fixedTickTime => 0.3f;
        public float currentTickTime { get; set; }
        
        public bool OnScanTime()
        {
            currentTickTime += Time.deltaTime;

            if (currentTickTime > fixedTickTime)
            {
                currentTickTime -= fixedTickTime;
                if (TryGetComponent(out AttackEntity attackEntity) == false)
                {
                    return false;
                }

                foreach (var egid in targetEntities)
                {
                    if (entityContainer.GetEntity(egid, out CharacterEntity characterEntity) == false
                        || characterEntity.waitbuffTickImplement.HasWaitBuffTick(attackEntity.EGID))
                    {
                        continue;
                    }
                    
                    var applyBuff = attackEntity.applyBuffImplement.applyBuffList;
                    foreach (var buff in applyBuff)
                    {
                        characterEntity.buffImplement.appliedBuff.Add(buff);
                    }

                    characterEntity.hitTintImplement.hitTintTriggerProperty.statusValue = true;
                    characterEntity.knockBackReceiveImplement.knockBackReceiveProperty.statusValue = attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue.CalcKnockBackReceiveDataSet(
                        entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue,
                        attackEntity.translateImplement.positionProperty.statusValue,
                        attackEntity.rigidBodyImplement.velocity,
                        characterEntity.translateImplement.positionProperty.statusValue
                        );

                }

                return true;
            }

            return false;
        }
        
        
        public void InitializeComponent()
        {
            currentTickTime = 0f;
            targetEntities.Clear();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (applyableCharacter == CharacterTypes.Enemy)
            {
                if (col.TryGetComponent(out EnemyCharacterEntity enemyCharacter))
                {
                    targetEntities.Add(enemyCharacter.EGID);    
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (applyableCharacter == CharacterTypes.Enemy)
            {
                if (other.TryGetComponent(out EnemyCharacterEntity enemyCharacter))
                {
                    targetEntities.Remove(enemyCharacter.EGID);
                }
            }
        }
        
        void Update()
        {
            OnScanTime();
        }

        
    }
}
