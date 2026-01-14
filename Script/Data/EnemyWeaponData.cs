using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyWeaponData : IMainKeyData
{
    public string deviceId;
    public string damage;
    public string area;
    public string projectile;
    public string projectileSpeed;
    public string projectileInterval;
    public string coolDown;
    public string pierce;
    public string duration;
    public string knockBack;
    public string stun;
    public string summonCreation;
    public string singleCreation;
    public string fileName;

    public string KeyField => deviceId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        deviceId = reader[index++];
        damage = reader[index++];
        area = reader[index++];
        projectile = reader[index++];
        projectileSpeed = reader[index++];
        projectileInterval = reader[index++];
        coolDown = reader[index++];
        pierce = reader[index++];
        duration = reader[index++];
        knockBack = reader[index++];
        stun = reader[index++];
        summonCreation = reader[index++];
        singleCreation = reader[index++];
        fileName = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif
public class EnemyWeaponDataEntity : MainKeyEntityImpl
{
    public string weaponId;
    public int damage;
    public float area;
    public int projectile;
    public float projectileSpeed;
    public float projectileInterval;
    public float coolDown;
    public int pierce;
    public float duration;
    public float knockBack;
    public float stun;
    public bool summonCreation;
    public bool singleCreation;
    public string fileName;

    public EnemyWeaponDataEntity(EnemyWeaponData src) : base(src)
    {
        weaponId = src.deviceId;
        int.TryParse(src.damage, out damage);
        float.TryParse(src.area, out area);
        int.TryParse(src.projectile, out projectile);
        float.TryParse(src.projectileSpeed, out projectileSpeed);
        float.TryParse(src.projectileInterval, out projectileInterval);
        float.TryParse(src.coolDown, out coolDown);
        int.TryParse(src.pierce, out pierce);
        float.TryParse(src.duration, out duration);
        float.TryParse(src.knockBack, out knockBack);
        float.TryParse(src.stun, out stun);
        bool.TryParse(src.summonCreation, out summonCreation);
        bool.TryParse(src.singleCreation, out singleCreation);
        fileName = src.fileName;
    }
}

public class EnemyWeaponCollection : EntityCollectionImpl<EnemyWeaponDataEntity>
{
    public EnemyWeaponCollection(List<EnemyWeaponDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}