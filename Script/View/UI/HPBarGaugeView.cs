using System;
using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS
{
    public class HPBarGaugeView : MonoBehaviour
    {
        public RectTransform myTransform;
        public GameObject background;
        public uint targetEntityId;
        public int maxHitPoint;
        public Image gauge;
        
        
        [Inject] private IEntityContainer entityContainer;

        public ReactiveProperty<float> gaugePercentile = new ReactiveProperty<float>(1f);

        private void Start()
        {
            SetMaxHP();
        }

        private void OnEnable()
        {
            gaugePercentile
                .TakeUntilDisable(this)
                .Subscribe(value =>
                {
                    gauge.fillAmount = value;
                })
                .AddTo(gameObject);

            MessageBroker.Default.Receive<string>()
                .TakeUntilDisable(this)
                .Subscribe(key =>
                {
                    switch (key)
                    {
                        case "OnApplyPassiveArcaneDevice":
                            SetMaxHP();
                            break;
                    }
                }).AddTo(gameObject);
        }
        
        private void Update()
        {
            SetCurrentHP();
        }

        private void SetMaxHP()
        {
            if (entityContainer.playerCharacterEntity.EGID == targetEntityId)
            {
                maxHitPoint = entityContainer.playerCharacterEntity.characterStatusImplement.buffedmaxHitPointProperty.statusValue;
                return;
            }
            
            if (entityContainer.GetEntity(targetEntityId, out BossEnemyCharacterEntity bossEntity))
            {
                maxHitPoint = bossEntity.monsterMaxHitPointImplement.maxHitPointProperty.statusValue;
            }
        }

        private void SetCurrentHP()
        {
            if (entityContainer.playerCharacterEntity.EGID == targetEntityId)
            {
                gaugePercentile.Value = (entityContainer.playerCharacterEntity.statusImplement.hitPointProperty.statusValue / (float)maxHitPoint);
                return;
            }
            
            if (entityContainer.GetEntity(targetEntityId, out BossEnemyCharacterEntity bossEntity))
            {
                gaugePercentile.Value = (bossEntity.statusImplement.hitPointProperty.statusValue / (float)maxHitPoint);
            }
        }

        
    }
}
