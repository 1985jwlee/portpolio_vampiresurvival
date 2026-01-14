using System;
using System.Collections.Generic;
using System.Linq;
using Client;

public struct AdvancedActiveDeviceData : ISubKeyData
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
    public string summonCreation;
    public string singleCreation;
    public string mustHaveTarget;
    public string fileName;
    public string summonAttackDeviceId;
    public string advancedMainActiveId;
    public string advancedSubPassiveId;
    public string advancedSubActiveId;
    public string groupCount;
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
        summonCreation = reader[index++];
        singleCreation = reader[index++];
        mustHaveTarget = reader[index++];
        fileName = reader[index++];
        summonAttackDeviceId = reader[index++];
        advancedMainActiveId = reader[index++];
        advancedSubPassiveId = reader[index++];
        advancedSubActiveId = reader[index++];
        groupCount = reader[index++];
        deviceName = reader[index++];
        description = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif
public class AdvancedActiveDeviceEntity : SubKeyEntityImpl, ILeveledArcaneDeviceInfo, IActiveDeviceDataEntity
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
    public bool summonCreation;
    public bool singleCreation;
    public bool mustHaveTarget;
    public string fileName;
    public string summonAttackDeviceId;
    public string advancedMainActiveId;
    public string advancedSubPassiveId;
    public string advancedSubActiveId;
    public int groupCount;
    public string deviceName;
    public string description;

    public AdvancedActiveDeviceEntity(AdvancedActiveDeviceData src) : base(src)
    {
        deviceId = src.deviceId;
        Enum.TryParse(src.slot, out slot);
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
        bool.TryParse(src.summonCreation, out summonCreation);
        bool.TryParse(src.mustHaveTarget, out mustHaveTarget);
        int.TryParse(src.groupCount, out groupCount);
        fileName = src.fileName;
        deviceName = src.deviceName;
        description = src.description;
        summonAttackDeviceId = src.summonAttackDeviceId;
        advancedMainActiveId = src.advancedMainActiveId;
        advancedSubPassiveId = src.advancedSubPassiveId;
        advancedSubActiveId = src.advancedSubActiveId;
    }

#region Implements_ILeveledArcaneDeviceInfo
    ArcaneDeviceType ILeveledArcaneDeviceInfo.DeviceType => ArcaneDeviceType.ADVANCED_ACTIVE;
    string ILeveledArcaneDeviceInfo.DeviceId => deviceId;
    DeviceSlot ILeveledArcaneDeviceInfo.DeviceSlot => slot;
    int ILeveledArcaneDeviceInfo.DeviceLevel => deviceLevel;
    string ILeveledArcaneDeviceInfo.DeviceName => deviceName;
    string ILeveledArcaneDeviceInfo.FileName => fileName;
    string ILeveledArcaneDeviceInfo.Description => description;
    
    bool ILeveledArcaneDeviceInfo.MustHaveTarget => mustHaveTarget;
    string ILeveledArcaneDeviceInfo.GetArcaneDeviceId => $"AdvActive_{deviceId}";
    #endregion

#region Implements_IActiveDeviceDataEntity
    string IActiveDeviceDataEntity.DeviceId => deviceId;
    DeviceSlot IActiveDeviceDataEntity.Slot => slot;
    int IActiveDeviceDataEntity.DeviceLevel => deviceLevel;
    int IActiveDeviceDataEntity.Damage => damage;
    float IActiveDeviceDataEntity.Area => area;
    int IActiveDeviceDataEntity.Projectile => projectile;
    float IActiveDeviceDataEntity.ProjectileSpeed => projectileSpeed;
    float IActiveDeviceDataEntity.ProjectileInterval => projectileInterval;
    float IActiveDeviceDataEntity.CoolDown => coolDown;
    float IActiveDeviceDataEntity.CriticalChance => criticalChance;
    float IActiveDeviceDataEntity.CriticalMultiple => criticalMultiple;
    int IActiveDeviceDataEntity.Pierce => pierce;
    float IActiveDeviceDataEntity.Duration => duration;
    float IActiveDeviceDataEntity.KnockBack => knockBack;
    float IActiveDeviceDataEntity.Stun => stun;
    bool IActiveDeviceDataEntity.SummonCreation => summonCreation;
    bool IActiveDeviceDataEntity.SingleCreation => singleCreation;

    bool IActiveDeviceDataEntity.MustHaveTarget => mustHaveTarget;
    
    string IActiveDeviceDataEntity.FileName => fileName;
    string IActiveDeviceDataEntity.SummonAttackDeviceId => summonAttackDeviceId;
    int IActiveDeviceDataEntity.GroupCount => groupCount;
    string IActiveDeviceDataEntity.DeviceName => deviceName;
    string IActiveDeviceDataEntity.Description => description;
#endregion
}

public class AdvancedActiveDeviceGroupEntity : GroupEntityImpl<AdvancedActiveDeviceEntity>, IArcaneDeviceInfo
{
    public ArcaneDeviceType DeviceType => ArcaneDeviceType.ADVANCED_ACTIVE;
    public string DeviceId => MainKey;
    public DeviceSlot DeviceSlot => GroupEntity["1"].slot;
    public string DeviceName => GroupEntity["1"].deviceName;
    public string FileName => GroupEntity["1"].fileName;
    public string Description => GroupEntity["1"].description;
    public int MaxLevel => GroupEntity.Count;
    public string GetArcaneDeviceId => $"AdvActive_{MainKey}";

    public AdvanceInfo? AdvanceInfo => new AdvanceInfo() { 
        MainActiveId = GroupEntity["1"].advancedMainActiveId,
        SubPassiveId= GroupEntity["1"].advancedSubPassiveId,
        SubActiveId = GroupEntity["1"].advancedSubActiveId,
    };

    public AdvancedActiveDeviceGroupEntity(string _key, IEnumerable<AdvancedActiveDeviceEntity> allSubKeys) : base(_key)
    {
        foreach (var subkey in allSubKeys)
        {
            GroupEntity.Add(subkey.SubKey, subkey);
        }
    }

}

public class AdvancedActiveDeviceCollection : EntityCollectionImpl<AdvancedActiveDeviceGroupEntity>
{
    public AdvancedActiveDeviceCollection(List<AdvancedActiveDeviceEntity> list)
    {
        var group = list.GroupBy(keySelector => keySelector.MainKey).ToList();
        foreach (var g in group)
        {
            var statusList = new List<AdvancedActiveDeviceEntity>();
            using (var enumerator = g.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    statusList.Add(enumerator.Current);
                }
            }
            entities.Add(g.Key, new AdvancedActiveDeviceGroupEntity(g.Key, statusList));
        }
    }
}