using Client;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

public enum StatusType
{

}

public struct PassiveDeviceData : ISubKeyData
{
    public string deviceId;
    public string slot;
    public string deviceLevel;
    public string status_1;
    public string status_1_value;
    public string status_2;
    public string status_2_value;
    public string status_3;
    public string status_3_value;
    public string fileName;
    public string deviceName;
    public string description;

    public string KeyField => deviceId;

    public string SubKeyField => deviceLevel;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        deviceId = reader[index++];
        slot = reader[index++];
        deviceLevel = reader[index++];
        status_1 = reader[index++];
        status_1_value = reader[index++];
        status_2 = reader[index++];
        status_2_value = reader[index++];
        status_3 = reader[index++];
        status_3_value = reader[index++];
        fileName = reader[index++];
        deviceName = reader[index++];
        description = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif
public class PassiveDeviceEntity : SubKeyEntityImpl, ILeveledArcaneDeviceInfo
{
    public string deviceId;
    public DeviceSlot slot;
    public int deviceLevel;
    public List<PassiveDeviceBuffInfo> buffInfos = new();
    public string fileName;
    public string deviceName;
    public string description;

    public PassiveDeviceEntity(PassiveDeviceData src) : base(src)
    {
        deviceId = src.deviceId;
        Enum.TryParse(src.slot, out slot);
        int.TryParse(src.deviceLevel, out deviceLevel);

        var buffInfo_RawDatas = new (string statusType, string value)[]
        {
            (src.status_1, src.status_1_value),
            (src.status_2, src.status_2_value),
            (src.status_3, src.status_3_value)
        };

        foreach(var rawData in buffInfo_RawDatas)
        {
            if(string.IsNullOrEmpty(rawData.statusType) == false)
            {
                Enum.TryParse(rawData.statusType, out BuffType statusType);
                float.TryParse(rawData.value, out var value);
                buffInfos.Add(new PassiveDeviceBuffInfo { statusType = statusType, value = value });
            }
        }

        fileName = src.fileName;
        deviceName = src.deviceName;
        description = src.description;
    }

    ArcaneDeviceType ILeveledArcaneDeviceInfo.DeviceType => ArcaneDeviceType.PASSIVE;
    string ILeveledArcaneDeviceInfo.DeviceId => deviceId;
    DeviceSlot ILeveledArcaneDeviceInfo.DeviceSlot => slot;
    int ILeveledArcaneDeviceInfo.DeviceLevel => deviceLevel;
    string ILeveledArcaneDeviceInfo.DeviceName => deviceName;
    string ILeveledArcaneDeviceInfo.FileName => fileName;
    string ILeveledArcaneDeviceInfo.Description => description;

    public struct PassiveDeviceBuffInfo
    {
        public BuffType statusType;
        public float value;
    }

    public string GetArcaneDeviceId => $"Passive_{deviceId}";
    public bool MustHaveTarget => false;
}

public class PassiveDeviceGroupEntity : GroupEntityImpl<PassiveDeviceEntity>, IArcaneDeviceInfo
{
    ArcaneDeviceType IArcaneDeviceInfo.DeviceType => ArcaneDeviceType.PASSIVE;
    string IArcaneDeviceInfo.DeviceId => MainKey;
    DeviceSlot IArcaneDeviceInfo.DeviceSlot => GroupEntity["1"].slot;
    string IArcaneDeviceInfo.DeviceName => GroupEntity["1"].deviceName;
    string IArcaneDeviceInfo.FileName => GroupEntity["1"].fileName;
    string IArcaneDeviceInfo.Description => GroupEntity["1"].description;
    int IArcaneDeviceInfo.MaxLevel => GroupEntity.Count;
    string IArcaneDeviceInfo.GetArcaneDeviceId => $"Passive_{MainKey}";

    public AdvanceInfo? AdvanceInfo => null;

    public PassiveDeviceGroupEntity(string _key, IEnumerable<PassiveDeviceEntity> allSubKeys) : base(_key)
    {
        foreach (var subkey in allSubKeys)
        {
            GroupEntity.Add(subkey.SubKey, subkey);
        }
    }
}

public class PassiveDeviceCollection : EntityCollectionImpl<PassiveDeviceGroupEntity>
{
    public PassiveDeviceCollection(List<PassiveDeviceEntity> list)
    {
        var group = list.GroupBy(keySelector => keySelector.MainKey).ToList();
        foreach (var g in group)
        {
            var statusList = new List<PassiveDeviceEntity>();
            using (var enumerator = g.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    statusList.Add(enumerator.Current);
                }
            }
            entities.Add(g.Key, new PassiveDeviceGroupEntity(g.Key, statusList));
        }
    }
}