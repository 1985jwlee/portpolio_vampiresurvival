using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum EventTriggerShape
    {
        Circle,
        Rect,
    }

    [System.Serializable]
    public struct TriggerSettingDataSet
    {
        public bool isEditing;
        public EventTriggerShape eventTriggerShape;
        public Vector2 position;
        public Vector2 rectXMaxYMin;
        public float radius;
        public bool shouldRemoveAfterTriggered;
    }
    public interface ITriggerSettingData : IStatusValue<TriggerSettingDataSet> { };
    [System.Serializable]
    public struct TriggerSettingData : ITriggerSettingData
    {
        [SerializeField] private TriggerSettingDataSet value;
        public TriggerSettingDataSet statusValue { get => value; set => this.value = value; }
    }

    public class EventMapTriggerSettingImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private IsEditing isEditing;

        [SerializeField] private TriggerSettingData triggerSetting;
        [SerializeField] private BossSpawnData bossSpawnSetting;
        public List<EnemySpawnData> enemySpawnSettings;
        public List<DummySpawnData> dummySpawnSettings;

        public ref IsEditing IsEditingProperty => ref isEditing;
        public ref TriggerSettingData TriggerSettingProperty => ref triggerSetting;
        public ref BossSpawnData BossSpawnSettingProperty => ref bossSpawnSetting;

        public void InitializeComponent()
        {
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(isEditing.statusValue)
            {
                if (triggerSetting.statusValue.isEditing)
                {
                    Gizmos.DrawIcon(triggerSetting.statusValue.position, $"Trigger0.png", true);
                }

                if (bossSpawnSetting.statusValue.bossId != 0)
                {
                    Gizmos.DrawIcon(bossSpawnSetting.statusValue.spawnPosition, $"BossSpawnPosition.png", true);
                    Gizmos.DrawIcon(bossSpawnSetting.statusValue.iconPosition, $"BossIconPosition.png", true);
                    Gizmos.DrawIcon(bossSpawnSetting.statusValue.teleportPosition, $"BossTeleportPosition.png", true);
                }

                for (int i = 0; i < enemySpawnSettings.Count; i++)
                {
                    Gizmos.DrawIcon(enemySpawnSettings[i].statusValue.spawnPosition, "EnemySpawnPosition.png", true);
                }

                for (int i = 0; i < dummySpawnSettings.Count; i++)
                {
                    Gizmos.DrawIcon(dummySpawnSettings[i].statusValue.spawnPosition, "BlockSpawnPosition.png", true);
                }
            }
        }
#endif
    }
}
