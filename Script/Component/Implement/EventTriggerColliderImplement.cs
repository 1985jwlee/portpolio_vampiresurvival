using Reflex.Scripts.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IIsTriggered : IStatusValue<bool> { }
    public interface IShouldRemoveAfterTriggered : IStatusValue<bool> { }

    public struct IsTriggered : IIsTriggered
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }
    public struct ShouldRemoveAfterTriggered : IShouldRemoveAfterTriggered
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    public class EventTriggerColliderImplement : MonoBehaviour, IComponent
    {
        [Inject] TimelineManager timelineManager;
        [Inject] EnemyCharacterFactory enemyCharacterFactory;
        [Inject] DummyFactory dummyFactory;

        private IsTriggered isTriggered;
        private ShouldRemoveAfterTriggered shouldRemoveAfterTriggered;

        public ref IsTriggered IsTriggeredProperty => ref isTriggered;
        public ref ShouldRemoveAfterTriggered ShouldRemoveAfterTriggeredProperty => ref shouldRemoveAfterTriggered;

        public void InitializeComponent()
        {
            isTriggered.statusValue = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerCharacterEntity>(out var playerCharacterEntity))
            {
                if (isTriggered.statusValue)
                    return;
                isTriggered.statusValue = true;

                var eventTrigger = GetComponent<EventTriggerEntity>();

                List<uint> dummyEgid = new();
                foreach (var data in eventTrigger.dummySpawnImplement.dummySpawnDatas)
                {
                    var dummy = dummyFactory.CreateGameObject($"Prefabs/Dummy/Dummy_{data.statusValue.dummyId}");
                    if (dummy.TryGetComponent(out DummyEntity dummyEntity))
                    {
                        dummyEntity.transformImplement.position = data.statusValue.spawnPosition;
                        dummyEgid.Add(dummyEntity.EGID);
                    }
                }

                if (eventTrigger.bossSpawnImplement.BossSpawnDataProperty.statusValue.bossId != 0)
                {
                    timelineManager.pause = true;
                    playerCharacterEntity.RecycleEnemies(true, true);

                    var spawnData = eventTrigger.bossSpawnImplement.BossSpawnDataProperty.statusValue;
                    var enemy = enemyCharacterFactory.CreateEnemy(spawnData.bossId.ToString(), spawnData.spawnPosition);
                    if (enemy is BossEnemyCharacterEntity boss)
                    {
                        boss.bossImplement.IsFinalBossProperty.statusValue = spawnData.isFinalBoss;
                        boss.bossImplement.BossIconPositionProperty.statusValue = spawnData.iconPosition;

                        foreach (var egid in dummyEgid)
                            boss.onKilledEventImplement.dummyEgidToRemoves.Add(new DummyEgidToRemove() { statusValue = egid });
                    }
                }
                else
                {
                    foreach (var egid in dummyEgid)
                        eventTrigger.dummyManageImplement.spawnedDummyEgids.Add(new SpawnedDummyEgid() { statusValue = egid });
                }

                foreach (var spawnData in eventTrigger.enemySpawnImplement.enemySpawnDatas)
                    enemyCharacterFactory.CreateEnemy(spawnData.statusValue.enemyId.ToString(), spawnData.statusValue.spawnPosition);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerCharacterEntity>(out var playerCharacterEntity))
            {
                isTriggered.statusValue = false;
            }
        }
    }
}
