using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    [System.Serializable]
    public struct BossSpawnDataSet
    {
        public uint bossId;
        public Vector2 spawnPosition;
        public Vector2 iconPosition;
        public Vector2 teleportPosition;
        public bool isFinalBoss;
    }

    public interface IBossSpawnData : IStatusValue<BossSpawnDataSet> { }

    [System.Serializable]
    public struct BossSpawnData : IBossSpawnData
    {
        [SerializeField] private BossSpawnDataSet value;
        public BossSpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    public class BossSpawnImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private BossSpawnData bossSpawnData = new();
        public ref BossSpawnData BossSpawnDataProperty => ref bossSpawnData;

        public void InitializeComponent()
        {
            bossSpawnData.statusValue = new BossSpawnDataSet() { bossId = 0 };
        }
    }
}
