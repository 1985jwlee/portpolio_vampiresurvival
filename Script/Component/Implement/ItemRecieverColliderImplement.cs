using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class ItemRecieverColliderImplement : MonoBehaviour, IComponent
    {
        private float initialSize;
        private CircleCollider2D recieverCollider;

        [Inject] private IEntityContainer entityContainer;

        
        private void Awake()
        {
            recieverCollider = GetComponent<CircleCollider2D>();
            initialSize = recieverCollider.radius;
        }

        public void ApplyPickAreaSize()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var multiplyScale = playerCharacterEntity.characterStatusImplement.buffedpickAreaSizeMultipleProperty.statusValue;
            recieverCollider.radius = (1f + multiplyScale) * initialSize;
        }

        private void OnEnable()
        {
            MessageBroker.Default.Receive<string>()
                .TakeUntilDisable(this)
                .Subscribe(key =>
                {
                    switch (key)
                    {
                        case "OnApplyPassiveArcaneDevice":
                            ApplyPickAreaSize();
                            break;
                    }
                }).AddTo(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var tokenEntity = col.GetComponent<TokenEntity>();
            if (tokenEntity != null)
            {
                var applyBuffImpl = tokenEntity.applyBuffImplement;
                var playerCharacterEntity = entityContainer.playerCharacterEntity;
                for (int i = 0; i < applyBuffImpl.applyBuffList.Count; ++i)
                {
                    var buff = applyBuffImpl.applyBuffList[i];
                    if (buff.statusValue.buffType == BuffType.ExpUp)
                    {
                        var addExpStatus = buff.statusValue;
                        addExpStatus.buffValue *= (1f + playerCharacterEntity.characterStatusImplement.buffedexpMultipleProperty.statusValue);

                        buff.statusValue = addExpStatus;
                    }
                    playerCharacterEntity.buffImplement.appliedBuff.Add(buff);
                }

                tokenEntity.tokenStateImplement.tokenStateProperty.statusValue = TokenStateType.Catched;

                if(tokenEntity.tokenSpeechImplement != null)
                    MessageBroker.Default.Publish(tokenEntity.tokenSpeechImplement.CharacterSpeechTypeProperty.statusValue);
            }
        }

        public void InitializeComponent()
        {
            
        }
    }
}
