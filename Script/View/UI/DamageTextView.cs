using System;
using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DamageTextView : MonoBehaviour
    {
        public GameObject playerDamageTextSrc;
        public GameObject playerHealTextSrc;
        public GameObject enemyDamageTextSrc;
        public GameObject enemyDeathPenaltyTextSrc;
        public RectTransform rectTransform;
        public float textScale = 0.01f;
        public float randomPositionOffset = 0.5f;

        [Inject] private IEntityContainer entityContainer;

        private void Start()
        {
            MessageBroker.Default.Receive<KeyValuePair<string, (uint, DamageType, CharacterTypes)>>()
                .TakeUntilDestroy(gameObject)
                .Subscribe(_x =>
                {
                    switch (_x.Key)
                    {
                        case "applyDamage":
                            if (entityContainer.GetEntity(_x.Value.Item1, out CharacterEntity entity))
                            {
                                var damageData = _x.Value.Item2;
                                if (damageData.damage > 0 == false)
                                {
                                    return;
                                }
                                bool isPlayerCharacterDamage = _x.Value.Item3 == CharacterTypes.PlayerCharacter;

                                var textSrc = isPlayerCharacterDamage ? playerDamageTextSrc : enemyDamageTextSrc;
                                var instance = Instantiate(textSrc, rectTransform);
                                if (instance.TryGetComponent<DamageText>(out var damageText))
                                {
                                    SetDamageTextTransform(damageText, entity.translateImplement.positionProperty.statusValue);

                                    
                                    var text = isPlayerCharacterDamage? $"-{damageData.damage}" : damageData.damage.ToString();
                                    damageText.SetText(text, damageData.isCritical, damageData.attackType);
                                }
                            }
                            break;
                    }
                    
                })
                .AddTo(gameObject);
            
            MessageBroker.Default.Receive<KeyValuePair<string, (uint, int, CharacterTypes)>>()
                .TakeUntilDestroy(gameObject)
                .Subscribe(_x =>
                {
                    switch (_x.Key)
                    {
                        case "applyHeal":
                            if (entityContainer.GetEntity(_x.Value.Item1, out CharacterEntity entity))
                            {
                                var damageText = Instantiate(playerHealTextSrc, rectTransform).GetComponent<DamageText>();
                                SetDamageTextTransform(damageText, entity.translateImplement.positionProperty.statusValue);
                                damageText.SetHealText($"+{_x.Value.Item2}");
                            }
                            break;
                    }
                })
                .AddTo(gameObject);

            MessageBroker.Default.Receive<KeyValuePair<string, uint>>()
                .TakeUntilDestroy(gameObject)
                .Subscribe(_x =>
                {
                    switch (_x.Key)
                    {
                        case "applyDeath":
                            if (entityContainer.GetEntity(_x.Value, out CharacterEntity entity))
                            {
                                var damageText = Instantiate(enemyDeathPenaltyTextSrc, rectTransform).GetComponent<DamageText>();
                                SetDamageTextTransform(damageText, entity.translateImplement.positionProperty.statusValue);
                            }
                            break;
                    }
                })
                .AddTo(gameObject);
        }

        private void SetDamageTextTransform(DamageText damageText, Vector3 position)
        {
            damageText.myTransform.localScale = Vector3.one * textScale;
            damageText.myTransform.anchoredPosition = position + ((Vector3)UnityEngine.Random.insideUnitCircle * randomPositionOffset);
        }
    }
}
