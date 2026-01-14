using System;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS
{
    public class BossGaugeView : MonoBehaviour
    {
        public List<GameObject> disableGauges;
        public List<GameObject> enableGauges;
        public Slider hpGaugeSlider;
        public TextMeshProUGUI bossNameText;
        private int maxHitpoint;
        private uint? bossEGID;

        [Inject] private TableDataHolder tableDataHolder;
        [Inject] private IEntityContainer entityContainer;
         

        private void Awake()
        {
            MessageBroker.Default.Receive<KeyValuePair<string, (uint id, float offset)>>()
                .TakeUntilDestroy(gameObject)
                .Subscribe(_x =>
                {
                    switch (_x.Key)
                    {
                        case "bossCharacterCreate":
                            foreach (var go in disableGauges)
                            {
                                go.SetActive(false);
                            }
                            foreach (var go in enableGauges)
                            {
                                go.SetActive(true);
                            }

                            hpGaugeSlider.value = 1f;
                            bossEGID = _x.Value.id;
                            if (entityContainer.GetEntity<EnemyCharacterEntity, MonsterMaxHitpointImplement>(
                                    bossEGID.Value, out var entity, out var component))
                            {
                                maxHitpoint = component.maxHitPointProperty.statusValue;
                                if (tableDataHolder.EnemyCollection.TryGetEntity( entity.tableIndexDataImplement.tableDataIndexNoProperty.statusValue.ToString(), out var data))
                                {
                                    bossNameText.text = data.monsterName;
                                }
                            }
                            break;
                    }
                });
        }

        private void Update()
        {
            if (bossEGID.HasValue)
            {
                if (entityContainer.GetEntity<EnemyCharacterEntity, CommonStatusImplement>(bossEGID.Value, out _, out var component))
                {
                    var currentHpbar = component.hitPointProperty.statusValue;
                    if (currentHpbar > 0)
                    {
                        //게이지가 0.15 아래로 가면 짜부가 됨
                        hpGaugeSlider.value = Mathf.Max(0.15f, (float)currentHpbar / maxHitpoint );
                        return;
                    }
                    
                    foreach (var go in disableGauges)
                    {
                        go.SetActive(true);
                    }
                    foreach (var go in enableGauges)
                    {
                        go.SetActive(false);
                    }
                }
            }
        }
    }
}
