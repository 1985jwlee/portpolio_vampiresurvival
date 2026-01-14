using System.Collections.Generic;
using System.Linq;
using Client;

public class CharacterDependDeviceData : ISubKeyData
{
    public string deviceId;
    public string slot;
    public string rootCharacterId;
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
        rootCharacterId = reader[index++];
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
        groupCount = reader[index++];
        deviceName = reader[index++];
        description = reader[index++];
    }

    public ActiveDeviceData ToActiveDeviceData()
    {
        var ret = new ActiveDeviceData
        {
            deviceId = deviceId,
            slot = slot,
            deviceLevel = deviceLevel,
            damage = damage,
            area = area,
            projectile = projectile,
            projectileSpeed = projectileSpeed,
            projectileInterval = projectileInterval,
            coolDown = coolDown,
            criticalChance = criticalChance,
            criticalMultiple = criticalMultiple,
            pierce = pierce,
            duration = duration,
            knockBack = knockBack,
            stun = stun,
            summonCreation = summonCreation,
            singleCreation = singleCreation,
            mustHaveTarget = mustHaveTarget,
            fileName = fileName,
            summonAttackDeviceId = summonAttackDeviceId,
            groupCount = groupCount,
            deviceName = deviceName,
            description = description
        };
        return ret;
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

public class CharacterDependDeviceEntity : ActiveDeviceEntity, ICharacterOwnedLeveledArcaneDeviceInfo, ICharacterOwnedActiveDeviceDataEntity
{
    public string rootCharacterId;

    public CharacterDependDeviceEntity(CharacterDependDeviceData src) : base(src.ToActiveDeviceData())
    {
        rootCharacterId = src.rootCharacterId;
    }

#region Implements_ILeveledArcaneDeviceInfo
    ArcaneDeviceType ILeveledArcaneDeviceInfo.DeviceType => ArcaneDeviceType.CHARACTER_OWNED_ACTIVE;
    string ILeveledArcaneDeviceInfo.DeviceId => deviceId;
    DeviceSlot ILeveledArcaneDeviceInfo.DeviceSlot => slot;
    int ILeveledArcaneDeviceInfo.DeviceLevel => deviceLevel;
    string ILeveledArcaneDeviceInfo.DeviceName => deviceName;
    string ILeveledArcaneDeviceInfo.FileName => fileName;
    string ILeveledArcaneDeviceInfo.Description => description;
    string ILeveledArcaneDeviceInfo.GetArcaneDeviceId => $"OwnActive_{deviceId}";
    bool ILeveledArcaneDeviceInfo.MustHaveTarget => mustHaveTarget;
    
    string ICharacterOwnedLeveledArcaneDeviceInfo.RootCharacterId => rootCharacterId;

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
    string IActiveDeviceDataEntity.FileName => fileName;
    string IActiveDeviceDataEntity.SummonAttackDeviceId => summonAttackDeviceId;
    int IActiveDeviceDataEntity.GroupCount => groupCount;
    string IActiveDeviceDataEntity.DeviceName => deviceName;
    string IActiveDeviceDataEntity.Description => description;
    bool IActiveDeviceDataEntity.MustHaveTarget => mustHaveTarget;
    string ICharacterOwnedActiveDeviceDataEntity.RootCharacterId => rootCharacterId;
#endregion
}

public class CharacterDependDeviceGroupEntity : GroupEntityImpl<CharacterDependDeviceEntity>, ICharacterOwnedArcaneDeviceInfo
{
    ArcaneDeviceType IArcaneDeviceInfo.DeviceType => ArcaneDeviceType.CHARACTER_OWNED_ACTIVE;
    string IArcaneDeviceInfo.DeviceId => MainKey;
    DeviceSlot IArcaneDeviceInfo.DeviceSlot => GroupEntity["1"].slot;
    string IArcaneDeviceInfo.DeviceName => GroupEntity["1"].deviceName;
    string IArcaneDeviceInfo.FileName => GroupEntity["1"].fileName;
    string IArcaneDeviceInfo.Description => GroupEntity["1"].description;
    int IArcaneDeviceInfo.MaxLevel => GroupEntity.Count;

    public string GetArcaneDeviceId => $"Active_{MainKey}";

    public AdvanceInfo? AdvanceInfo => null;

    public string RootCharacterId => GroupEntity["1"].rootCharacterId;

    public CharacterDependDeviceGroupEntity(string _key, IEnumerable<CharacterDependDeviceEntity> allSubKeys) : base(_key)
    {
        foreach (var subkey in allSubKeys)
        {
            GroupEntity.Add(subkey.SubKey, subkey);
        }
    }
}

public class CharacterDependDeviceCollection : EntityCollectionImpl<CharacterDependDeviceGroupEntity>
{
    public CharacterDependDeviceCollection(List<CharacterDependDeviceEntity> list)
    {
        var group = list.GroupBy(keySelector => keySelector.MainKey).ToList();
        foreach (var g in group)
        {
            var statusList = new List<CharacterDependDeviceEntity>();
            using (var enumerator = g.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    statusList.Add(enumerator.Current);
                }
            }
            entities.Add(g.Key, new CharacterDependDeviceGroupEntity(g.Key, statusList));
        }
    }
}