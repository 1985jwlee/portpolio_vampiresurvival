using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    [System.Serializable]
    public struct DummySpawnDataSet
    {
        public uint dummyId;
        public Vector2 spawnPosition;
    }

    public interface IDummySpawnData : IStatusValue<DummySpawnDataSet> { }

    [System.Serializable]
    public struct DummySpawnData : IDummySpawnData
    {
        [SerializeField] private DummySpawnDataSet value;
        public DummySpawnDataSet statusValue { get => value; set => this.value = value; }
    }

    public class DummySpawnImplement : MonoBehaviour, IComponent
    {
        public List<DummySpawnData> dummySpawnDatas = new();

        public void InitializeComponent()
        {
        }
    }
}
