using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ISpawnedDummyEgid : IStatusValue<uint> { }

    [System.Serializable]
    public struct SpawnedDummyEgid : ISpawnedDummyEgid
    {
        [SerializeField] uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }


    public class DummyManageImplement : MonoBehaviour, IComponent
    {
        public List<SpawnedDummyEgid> spawnedDummyEgids = new List<SpawnedDummyEgid>();

        public void InitializeComponent()
        {
            spawnedDummyEgids.Clear();
        }
    }
}
