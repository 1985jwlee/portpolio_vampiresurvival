using System;
using UnityEngine;

namespace Game.ECS
{
    [Serializable]
    public struct PassiveDeviceEquipDataSet
    {
        public string id;
        public int level;
    }

    public interface IPassiveDeviceEquipDataSet : IStatusValue<PassiveDeviceEquipDataSet>
    {
    }

    [Serializable]
    public struct PassiveDeviceEquipData : IPassiveDeviceEquipDataSet
    {
        [SerializeField] private PassiveDeviceEquipDataSet value;
        public PassiveDeviceEquipDataSet statusValue { get => value; set => this.value = value; }
    }
}
