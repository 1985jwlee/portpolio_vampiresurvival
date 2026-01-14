using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerCharacterHPRecoveryImplement : MonoBehaviour, IComponent, ITickComponent
    {
        [Inject] private IEntityContainer entityContainer;
        
        public float fixedTickTime => 5f;
        public float currentTickTime { get; set; }
        
        public bool OnScanTime()
        {
            currentTickTime += Time.deltaTime;
            if ((currentTickTime > fixedTickTime) == false)
            {
                return false;
            }
            
            var playerCharacter = entityContainer.playerCharacterEntity;
            var maxHp = playerCharacter.characterStatusImplement.buffedmaxHitPointProperty.statusValue;
            var recovery = playerCharacter.characterStatusImplement.buffedhitpointRecoveryRateProperty.statusValue;
            var recoverHp = Mathf.CeilToInt(recovery * maxHp * fixedTickTime * 0.2f);
            var currentHP = recoverHp + playerCharacter.statusImplement.hitPointProperty.statusValue;
            currentHP = Mathf.Min(currentHP, maxHp);
            playerCharacter.statusImplement.hitPointProperty.statusValue = currentHP;
            if (recoverHp > 0)
            {
                MessageBroker.Default.Publish(new KeyValuePair<string, (uint, int, CharacterTypes)>("applyHeal", (playerCharacter.EGID, recoverHp, CharacterTypes.PlayerCharacter)));
            }
            currentTickTime -= fixedTickTime;
            return true;
        }
        public void InitializeComponent()
        {
            currentTickTime = 0f;
        }
        

        private void Update()
        {
            OnScanTime();
        }
    }
}
