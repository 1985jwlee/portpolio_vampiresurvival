using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    [System.Serializable]
    public struct EnemySpawnDataSet
    {
        public uint enemyId;
        public Vector2 spawnPosition;
    }

    public interface IEnemySpawnData : IStatusValue<EnemySpawnDataSet> { }

    [System.Serializable]
    public struct EnemySpawnData : IEnemySpawnData
    {
        [SerializeField] private EnemySpawnDataSet value;
        public EnemySpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    public class EnemySpawnImplement : MonoBehaviour, IComponent
    {
        public List<EnemySpawnData> enemySpawnDatas = new();

        public void InitializeComponent()
        {
        }
    }
}
