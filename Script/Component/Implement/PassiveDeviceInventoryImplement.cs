using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class PassiveDeviceInventoryImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private List<PassiveDeviceEquipData> passiveDevices = new List<PassiveDeviceEquipData>();

        public IReadOnlyList<PassiveDeviceEquipData> PassiveDeviceDatas => passiveDevices;

        public void AddOrUpdatePassiveDevice(PassiveDeviceEquipData newData)
        {
            var index = passiveDevices.FindIndex(elmt => elmt.statusValue.id == newData.statusValue.id);
            if (index == -1)
            {
                passiveDevices.Add(newData);
            }
            else
            {
                passiveDevices[index] = newData;
            }
        }

        public void InitializeComponent()
        {
            
        }
    }
}
