using Client;
using System;
using System.Collections.Generic;
using System.Linq;

public struct CharacterData : IMainKeyData
{
    public string characterId;

    public string characterName;
    public string prefabName;

    public string hitpoint;
    public string recoveryRate;
    public string damageReduction;
    public string speedRatio;
    public string additionalMagicDamageRatio;
    public string attackSizeRatio;
    public string additionalProjectileCount;
    public string projectileSpeedRatio;
    public string fastCooldownRatio;
    public string additionalCriticalRatio;

    public string weaponSlotNum;
    public string magicSlotNum;
    public string legacySlotNum;

    public string supporterSlotType;
    public string startDeviceId;

    public string startText;
    public string levelUpText;
    public string gameOverText;
    public string stageClearText;
    public string bossEncounterText;
    public string foodItemText;

    public string KeyField => characterId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        characterId = reader[index++];
        characterName = reader[index++];
        prefabName = reader[index++];

        hitpoint = reader[index++];
        recoveryRate = reader[index++];
        damageReduction = reader[index++];
        speedRatio = reader[index++];
        additionalMagicDamageRatio = reader[index++];
        attackSizeRatio = reader[index++];
        additionalProjectileCount = reader[index++];
        projectileSpeedRatio = reader[index++];
        fastCooldownRatio = reader[index++];
        additionalCriticalRatio = reader[index++];

        weaponSlotNum = reader[index++];
        magicSlotNum = reader[index++];
        legacySlotNum = reader[index++];

        supporterSlotType = reader[index++];
        startDeviceId = reader[index++];

        startText = reader[index++];
        levelUpText = reader[index++];
        gameOverText = reader[index++];
        stageClearText = reader[index++];
        bossEncounterText = reader[index++];
        foodItemText = reader[index++];
    }
}


#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif
[System.Serializable]
public class CharacterDataEntity : SerializableMainKeyEntityImpl
{
    public string characterId;
    public int health;
    public float healthRegen;
    public float damageReduction;
    public float moveSpeedIncrement;
    public float magicDamageIncrement;
    public float areaIncrement;
    public int projectileIncrement;
    public float projectileSpeedIncrement;
    public float cooldownReduction;
    public float criticalChanceIncrement;

    public int weaponSlotNum;
    public int magicSlotNum;
    public int legacySlotNum;

    public DeviceSlot supporterSlotType;
    public string startDeviceId;

    public string prefabName;

    public string characterName;
    public string startText;
    public string levelUpText;
    public string gameOverText;
    public string stageClearText;
    public string bossEncounterText;
    public string foodItemText;

    public CharacterDataEntity() : base(new CharacterData() { characterId = "" })
    {

    }

    public CharacterDataEntity(CharacterData src) : base(src)
    {
        characterId = src.characterId;
        characterName = src.characterName;
        prefabName = src.prefabName;

        int.TryParse(src.hitpoint, out health);
        float.TryParse(src.recoveryRate, out healthRegen);
        float.TryParse(src.damageReduction, out damageReduction);
        float.TryParse(src.speedRatio, out moveSpeedIncrement);
        float.TryParse(src.additionalMagicDamageRatio, out magicDamageIncrement);
        float.TryParse(src.attackSizeRatio, out areaIncrement);
        int.TryParse(src.additionalProjectileCount, out projectileIncrement);
        float.TryParse(src.projectileSpeedRatio, out projectileSpeedIncrement);
        float.TryParse(src.fastCooldownRatio, out cooldownReduction);
        float.TryParse(src.additionalCriticalRatio, out criticalChanceIncrement);

        int.TryParse(src.weaponSlotNum, out weaponSlotNum);
        int.TryParse(src.magicSlotNum, out magicSlotNum);
        int.TryParse(src.legacySlotNum, out legacySlotNum);

        Enum.TryParse(src.supporterSlotType, out supporterSlotType);
        startDeviceId = src.startDeviceId;

        startText = src.startText;
        levelUpText = src.levelUpText;
        gameOverText = src.gameOverText;
        stageClearText = src.stageClearText;
        bossEncounterText = src.bossEncounterText;
        foodItemText = src.foodItemText;
    }

    public int GetSlotNum(DeviceSlot deviceSlot) => deviceSlot switch
    {
        DeviceSlot.WeaponSlot => weaponSlotNum,
        DeviceSlot.MagicSlot => magicSlotNum,
        DeviceSlot.LegacySlot => legacySlotNum,
        _ => 0
    };

    public static CharacterDataEntity GetTeeste => new CharacterDataEntity()
    {
        prefabName = "Char_PC_Teeste",
        health = 100,
        healthRegen = 0.02f,
        damageReduction = 0f,
        moveSpeedIncrement = 0f,
        magicDamageIncrement = 0f,
        areaIncrement = 1f,
        projectileIncrement = 0,
        projectileSpeedIncrement = 1f,
        cooldownReduction = 1f,
        criticalChanceIncrement = 0f,

        weaponSlotNum = 4,
        magicSlotNum = 3,
        legacySlotNum = 3,

        startDeviceId = null,

        characterName = "티스테",
        startText = "안녕하세요, 원장님. 세계수반 담임 티스테입니다!",
        levelUpText = "델리아 선생님, 끝나고 요르구트 한 잔 어때요?",
    };
}

public class CharacterCollection : EntityCollectionImpl<CharacterDataEntity>
{
    public CharacterCollection(List<CharacterDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}