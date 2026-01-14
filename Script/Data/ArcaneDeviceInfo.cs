public interface IArcaneDeviceInfo
{
    ArcaneDeviceType DeviceType { get; }
    string DeviceId { get; }
    DeviceSlot DeviceSlot { get; }
    string DeviceName { get; }
    string FileName { get; }
    string Description { get; }
    int MaxLevel { get; }
    string GetArcaneDeviceId { get; }
    AdvanceInfo? AdvanceInfo { get; }
}

public interface ICharacterOwnedArcaneDeviceInfo : IArcaneDeviceInfo
{
    string RootCharacterId { get; }
}

public interface ILeveledArcaneDeviceInfo
{
    ArcaneDeviceType DeviceType { get; }
    string DeviceId { get; }
    int DeviceLevel { get; }
    DeviceSlot DeviceSlot { get; }
    string DeviceName { get; }
    string FileName { get; }
    string Description { get; }
    string GetArcaneDeviceId { get; }
    bool MustHaveTarget { get; }
}

public interface ICharacterOwnedLeveledArcaneDeviceInfo : ILeveledArcaneDeviceInfo
{
    string RootCharacterId { get; }
}


public enum ArcaneDeviceType
{
    ACTIVE, PASSIVE, ADVANCED_ACTIVE, CHARACTER_OWNED_ACTIVE
}

public struct ArcaneDeviceInfo : IArcaneDeviceInfo
{
    public ArcaneDeviceType DeviceType { get; set; }
    public string DeviceId { get; set; }
    public DeviceSlot DeviceSlot { get; set; }
    public string DeviceName { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }
    public int MaxLevel { get; set; }

    public string GetArcaneDeviceId => $"{(DeviceType == ArcaneDeviceType.ACTIVE || DeviceType == ArcaneDeviceType.CHARACTER_OWNED_ACTIVE ? "Active" : DeviceType == ArcaneDeviceType.PASSIVE ? "Passive" : "AdvActive")}_{DeviceId}";

    public AdvanceInfo? AdvanceInfo { get; set; }

    public ArcaneDeviceInfo(IArcaneDeviceInfo info)
    {
        DeviceType = info.DeviceType;
        DeviceId = info.DeviceId;
        DeviceSlot = info.DeviceSlot;
        DeviceName = info.DeviceName;
        FileName = info.FileName;
        Description = info.Description;
        MaxLevel = info.MaxLevel;
        AdvanceInfo = info.AdvanceInfo;
    }
}

public struct LeveledArcaneDeviceInfo : ILeveledArcaneDeviceInfo
{
    public ArcaneDeviceType DeviceType { get; set; }
    public string DeviceId { get; set; }
    public DeviceSlot DeviceSlot { get; set; }
    public int DeviceLevel { get; set; }
    public string DeviceName { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }

    public string GetArcaneDeviceId => $"{(DeviceType == ArcaneDeviceType.ACTIVE || DeviceType == ArcaneDeviceType.CHARACTER_OWNED_ACTIVE ? "Active" : DeviceType == ArcaneDeviceType.PASSIVE ? "Passive" : "AdvActive")}_{DeviceId}";
    
    public bool MustHaveTarget { get; set; }

    public LeveledArcaneDeviceInfo(ILeveledArcaneDeviceInfo leveledInfo)
    {
        DeviceType = leveledInfo.DeviceType;
        DeviceId = leveledInfo.DeviceId;
        DeviceSlot = leveledInfo.DeviceSlot;
        DeviceLevel = leveledInfo.DeviceLevel;
        DeviceName = leveledInfo.DeviceName;
        FileName = leveledInfo.FileName;
        Description = leveledInfo.Description;
        MustHaveTarget = leveledInfo.MustHaveTarget;
    }
}

public enum DeviceSlot
{
    WeaponSlot,
    MagicSlot,
    LegacySlot
}

public struct AdvanceInfo
{
    public string MainActiveId;
    public string SubPassiveId;
    public string SubActiveId;
}

public interface IActiveDeviceDataEntity
{
    string DeviceId { get; }
    DeviceSlot Slot { get; }
    int DeviceLevel { get; }
    int Damage { get; }
    float Area { get; }
    int Projectile { get; }
    float ProjectileSpeed { get; }
    float ProjectileInterval { get; }
    float CoolDown { get; }
    float CriticalChance { get; }
    float CriticalMultiple { get; }
    int Pierce { get; }
    float Duration { get; }
    float KnockBack { get; }
    float Stun { get; }
    bool SummonCreation { get; }
    bool SingleCreation { get; }
    string FileName { get; }
    string SummonAttackDeviceId { get; }
    int GroupCount { get; }
    string DeviceName { get; }
    string Description { get; }
    bool MustHaveTarget { get; }
}

public interface ICharacterOwnedActiveDeviceDataEntity : IActiveDeviceDataEntity
{
    string RootCharacterId { get; }
}