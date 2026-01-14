using System.Collections.Generic;
using System.Linq;
using Client;

public struct SummonActiveDeviceData : ISubKeyData
{
    public string deviceId;
    public string slot;
    public string deviceLevel;
    public string damage;
    public string area;
    public string projectile;
    public string projectileSpeed;
    public string projectileInterval;
    public string coolDown;
    public string criticalChance;
    public string criticalMultiple;
    public string pierce;
    public string duration;
    public string knockBack;
    public string stun;
    public string singleCreation;
    public string mustHaveTarget;
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
        damage = reader[index++];
        area = reader[index++];
        projectile = reader[index++];
        projectileSpeed = reader[index++];
        projectileInterval = reader[index++];
        coolDown = reader[index++];
        criticalChance = reader[index++];
        criticalMultiple = reader[index++];
        pierce = reader[index++];
        duration = reader[index++];
        knockBack = reader[index++];
        stun = reader[index++];
        singleCreation = reader[index++];
        mustHaveTarget = reader[index++];
        fileName = reader[index++];
        deviceName = reader[index++];
        description = reader[index++];
    }
}


#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif
public class SummonActiveDeviceEntity : SubKeyEntityImpl, ILeveledArcaneDeviceInfo
{
    public string deviceId;
    public DeviceSlot slot;
    public int deviceLevel;
    public int damage;
    public float area;
    public int projectile;
    public float projectileSpeed;
    public float projectileInterval;
    public float coolDown;
    public float criticalChance;
    public float criticalMultiple;
    public int pierce;
    public float duration;
    public float knockBack;
    public float stun;
    public bool singleCreation;
    public bool mustHaveTarget;
    public string fileName;
    public string deviceName;
    public string description;

    public SummonActiveDeviceEntity(SummonActiveDeviceData src) : base(src)
    {
        deviceId = src.deviceId;
        System.Enum.TryParse(src.slot, out slot);
        int.TryParse(src.deviceLevel, out deviceLevel);
        int.TryParse(src.damage, out damage);
        float.TryParse(src.area, out area);
        int.TryParse(src.projectile, out projectile);
        float.TryParse(src.projectileSpeed, out projectileSpeed);
        float.TryParse(src.projectileInterval, out projectileInterval);
        float.TryParse(src.coolDown, out coolDown);
        float.TryParse(src.criticalChance, out criticalChance);
        float.TryParse(src.criticalMultiple, out criticalMultiple);
        int.TryParse(src.pierce, out pierce);
        float.TryParse(src.duration, out duration);
        float.TryParse(src.knockBack, out knockBack);
        float.TryParse(src.stun, out stun);
        bool.TryParse(src.singleCreation, out singleCreation);
        bool.TryParse(src.mustHaveTarget, out mustHaveTarget);
        fileName = src.fileName;
        deviceName = src.deviceName;
        description = src.description;
    }

    ArcaneDeviceType ILeveledArcaneDeviceInfo.DeviceType => ArcaneDeviceType.ACTIVE;
    string ILeveledArcaneDeviceInfo.DeviceId => deviceId;
    DeviceSlot ILeveledArcaneDeviceInfo.DeviceSlot => slot;
    int ILeveledArcaneDeviceInfo.DeviceLevel => deviceLevel;
    string ILeveledArcaneDeviceInfo.DeviceName => deviceName;
    string ILeveledArcaneDeviceInfo.FileName => fileName;
    string ILeveledArcaneDeviceInfo.Description => description;
    string ILeveledArcaneDeviceInfo.GetArcaneDeviceId => $"Active_{deviceId}";
    bool ILeveledArcaneDeviceInfo.MustHaveTarget => mustHaveTarget;
}


public class SummonActiveDeviceGroupEntity : GroupEntityImpl<SummonActiveDeviceEntity>, IArcaneDeviceInfo
{
    ArcaneDeviceType IArcaneDeviceInfo.DeviceType => ArcaneDeviceType.ACTIVE;
    string IArcaneDeviceInfo.DeviceId => MainKey;
    DeviceSlot IArcaneDeviceInfo.DeviceSlot => GroupEntity["1"].slot;
    string IArcaneDeviceInfo.DeviceName => GroupEntity["1"].deviceName;
    string IArcaneDeviceInfo.FileName => GroupEntity["1"].fileName;
    string IArcaneDeviceInfo.Description => GroupEntity["1"].description;
    int IArcaneDeviceInfo.MaxLevel => GroupEntity.Count;
    string IArcaneDeviceInfo.GetArcaneDeviceId => $"Active_{MainKey}";
    public AdvanceInfo? AdvanceInfo => null;

    public SummonActiveDeviceGroupEntity(string _key, IEnumerable<SummonActiveDeviceEntity> allSubKeys) : base(_key)
    {
        foreach (var subkey in allSubKeys)
        {
            GroupEntity.Add(subkey.SubKey, subkey);
        }
    }
}


public class SummonActiveDeviceCollection : EntityCollectionImpl<SummonActiveDeviceGroupEntity>
{
    public SummonActiveDeviceCollection(List<SummonActiveDeviceEntity> list)
    {
        var group = list.GroupBy(keySelector => keySelector.MainKey).ToList();
        foreach (var g in group)
        {
            var statusList = new List<SummonActiveDeviceEntity>();
            using (var enumerator = g.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    statusList.Add(enumerator.Current);
                }
            }
            entities.Add(g.Key, new SummonActiveDeviceGroupEntity(g.Key, statusList));
        }
    }
}
