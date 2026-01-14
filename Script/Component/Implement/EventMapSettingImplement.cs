using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{

    [System.Serializable]
    public struct TeleportSpawnDataSet
    {
        public bool shouldSpawnTeleport;
        public Vector2 spawnPosition;
    }

    [System.Serializable]
    public struct SanctumSpawnDataSet
    {
        public SanctumType SanctumType;
        public Vector2 spawnPosition;
    }

    [System.Serializable]
    public struct ExpSpawnDataSet
    {
        public int value;
        public Vector2 spawnPosition;
    }

    [System.Serializable]
    public struct GoldSpawnDataSet
    {
        public uint value;
        public Vector2 spawnPosition;
    }

    [System.Serializable]
    public struct ItemSpawnDataSet
    {
        public uint itemId;
        public Vector2 spawnPosition;
    }

    public interface ITeleportSpawnData : IStatusValue<TeleportSpawnDataSet> { }
    public interface ISanctumSpawnData : IStatusValue<SanctumSpawnDataSet> { }
    public interface IIsEditing : IStatusValue<bool> { }
    public interface IExpSpawnData : IStatusValue<ExpSpawnDataSet> { }
    public interface IGoldSpawnData : IStatusValue<GoldSpawnDataSet> { }
    public interface IItemSpawnData : IStatusValue<ItemSpawnDataSet> { }

    [System.Serializable]
    public struct IsEditing : IIsEditing
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct ExpSpawnData : IExpSpawnData
    {
        [SerializeField] private ExpSpawnDataSet value;
        public ExpSpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct GoldSpawnData : IGoldSpawnData
    {
        [SerializeField] private GoldSpawnDataSet value;
        public GoldSpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct ItemSpawnData : IItemSpawnData
    {
        [SerializeField] private ItemSpawnDataSet value;
        public ItemSpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct TeleportSpawnData : ITeleportSpawnData
    {
        [SerializeField] private TeleportSpawnDataSet value;
        public TeleportSpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct SanctumSpawnData : ISanctumSpawnData
    {
        [SerializeField] private SanctumSpawnDataSet value;
        public SanctumSpawnDataSet statusValue { get => value; set => this.value = value; }
    }


    public class EventMapSettingImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private IsEditing isEditing;
        [SerializeField] private TeleportSpawnData teleportSpawnData;

        public ref IsEditing IsEditingProperty => ref isEditing;
        public ref TeleportSpawnData TeleportSpawnDataProperty => ref teleportSpawnData;

        public List<SanctumSpawnData> sanctumSpawnSettings = new List<SanctumSpawnData>();

        public List<ExpSpawnData> expSpawnSettings = new List<ExpSpawnData>();
        public List<GoldSpawnData> goldSpawnSettings = new List<GoldSpawnData>();
        public List<ItemSpawnData> itemSpawnSettings = new List<ItemSpawnData>();

        public void InitializeComponent()
        {
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(IsEditingProperty.statusValue)
            {
                if(teleportSpawnData.statusValue.shouldSpawnTeleport)
                {
                    Gizmos.DrawIcon(teleportSpawnData.statusValue.spawnPosition, "TeleportSpawnPosition.png", true);
                }
                for (int i = 0; i < sanctumSpawnSettings.Count; i++)
                {
                    Gizmos.DrawIcon(sanctumSpawnSettings[i].statusValue.spawnPosition, "SanctumSpawnPosition.png", true);
                }
                for (int i = 0; i < expSpawnSettings.Count; i++)
                {
                    Gizmos.DrawIcon(expSpawnSettings[i].statusValue.spawnPosition, "JewelSpawnPosition.png", true);
                }
                for (int i = 0; i < itemSpawnSettings.Count; i++)
                {
                    Gizmos.DrawIcon(itemSpawnSettings[i].statusValue.spawnPosition, "ItemSpawnPosition.png", true);
                }
            }
        }
#endif
    }
}
