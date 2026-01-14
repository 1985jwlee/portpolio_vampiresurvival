using System;
using Client;
using System.Collections.Generic;

public struct EnemyData : IMainKeyData
{
    public string monsterId;
    public string monsterName;
    public string monsterType;
    public string health;
    public string damage;
    public string moveSpeed;
    public string experience;
    public string weaponId;
    public string prefabPath;
    public string itemDropId01;
    public string itemDropId02;
    public string itemDropId03;
    public string itemDropId04;
    public string itemDropId05;

    public string KeyField => monsterId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        monsterId = reader[index++];
        monsterName = reader[index++];
        monsterType = reader[index++];
        health = reader[index++];
        damage = reader[index++];
        moveSpeed = reader[index++];
        experience = reader[index++];
        weaponId = reader[index++];
        prefabPath = reader[index++];
        itemDropId01 = reader[index++];
        itemDropId02 = reader[index++];
        itemDropId03 = reader[index++];
        itemDropId04 = reader[index++];
        itemDropId05 = reader[index++];
    }
}

public enum MonsterType
{
    Common, Elite, Boss
}


#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

public class EnemyDataEntity : MainKeyEntityImpl
{
    public string enemyId;
    public string monsterName;
    public MonsterType monsterType;
    public int hitPoint;
    public int damage;
    public float moveSpeed;
    public int exp;
    public string weaponId;
    public string prefabPath;
    public List<string> itemDropIds;

    public EnemyDataEntity(EnemyData src) : base(src)
    {
        enemyId = src.monsterId;
        int.TryParse(src.health, out hitPoint);
        int.TryParse(src.damage, out damage);
        float.TryParse(src.moveSpeed, out moveSpeed);
        int.TryParse(src.experience, out exp);
        Enum.TryParse(src.monsterType, out monsterType);
        
        weaponId = src.weaponId;
        prefabPath = src.prefabPath;
        monsterName = src.monsterName;


        string[] ids = new string[] { src.itemDropId01, src.itemDropId02, src.itemDropId03, src.itemDropId04, src.itemDropId05 };

        itemDropIds = new List<string>();
        foreach (var id in ids)
        {
            if (string.IsNullOrEmpty(id) == false)
                itemDropIds.Add(id);
        }
    }
}
public class EnemyCollection : EntityCollectionImpl<EnemyDataEntity>
{
    public EnemyCollection(List<EnemyDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}