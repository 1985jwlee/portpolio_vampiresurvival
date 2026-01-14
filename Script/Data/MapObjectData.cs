using System;
using Client;
using System.Collections.Generic;

public struct MapObjectData : IMainKeyData
{
    public string mapObjectId;
    public string mapObjectName;
    public string health;
    public string prefabPath;
    public string itemDropId01;
    public string itemDropId02;
    public string itemDropId03;
    public string itemDropId04;
    public string itemDropId05;

    public string KeyField => mapObjectId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        mapObjectId = reader[index++];
        mapObjectName = reader[index++];
        health = reader[index++];
        prefabPath = reader[index++];
        itemDropId01 = reader[index++];
        itemDropId02 = reader[index++];
        itemDropId03 = reader[index++];
        itemDropId04 = reader[index++];
        itemDropId05 = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

public class MapObjectDataEntity : MainKeyEntityImpl
{
    public string mapObjectId;
    public string mapObjectName;
    public int hitPoint;
    public string prefabPath;
    public List<string> itemDropIds;

    public MapObjectDataEntity(MapObjectData src) : base(src)
    {
        mapObjectId = src.mapObjectId;
        int.TryParse(src.health, out hitPoint);
        prefabPath = src.prefabPath;

        string[] ids = new string[] { src.itemDropId01, src.itemDropId02, src.itemDropId03, src.itemDropId04, src.itemDropId05 };

        itemDropIds = new List<string>();
        foreach(var id in ids)
        {
            if (string.IsNullOrEmpty(id) == false)
                itemDropIds.Add(id);
        }
    }
}
public class MapObjectCollection : EntityCollectionImpl<MapObjectDataEntity>
{
    public MapObjectCollection(List<MapObjectDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}