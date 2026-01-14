using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IExpToDrop : IStatusValue<int> { };
    public interface IItemIdToDrop : IStatusValue<uint> { };
    public interface IMonsterSpawnNum : IStatusValue<int> { };
    public interface IMonsterSpawnId : IStatusValue<uint> { };
    public interface IMonsterSpawnRadius : IStatusValue<float> { };
    public interface IShouldExplode : IStatusValue<bool> { };
    public interface IDummyEgidToRemove : IStatusValue<uint> { };

    [Serializable]
    public struct ExpToDrop : IExpToDrop
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ItemIdToDrop : IItemIdToDrop
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct MonsterSpawnNum : IMonsterSpawnNum
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct MonsterSpawnId : IMonsterSpawnId
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct MonsterSpawnRadius : IMonsterSpawnRadius
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ShouldExplode : IShouldExplode
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct DummyEgidToRemove : IDummyEgidToRemove
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }


    public class OnKilledEventImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private ExpToDrop expToDrop;

        [SerializeField] private MonsterSpawnNum monsterSpawnNum;
        [SerializeField] private MonsterSpawnId monsterSpawnId;
        [SerializeField] private MonsterSpawnRadius monsterSpawnRadius;

        [SerializeField] private ShouldExplode shouldExplode;
        [SerializeField] private WeaponData weaponToSpawn;

        public ref ExpToDrop ExpToDropProperty => ref expToDrop;
       
        public ref MonsterSpawnNum MonsterSpawnNumProperty => ref monsterSpawnNum;
        public ref MonsterSpawnId MonsterSpawnIdProperty => ref monsterSpawnId;
        public ref MonsterSpawnRadius MonsterSpawnRadiusProperty => ref monsterSpawnRadius;
        public ref ShouldExplode ShouldExplodeProperty => ref shouldExplode;
        public ref WeaponData WeaponToSpawnProperty => ref weaponToSpawn;

        public List<ItemIdToDrop> itemIdsToDrop = new List<ItemIdToDrop>();
        public List<DummyEgidToRemove> dummyEgidToRemoves = new List<DummyEgidToRemove>();

        public void InitializeComponent()
        {
            itemIdsToDrop.Clear();
        }
    }
}
