using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemData : IMainKeyData
{
    public string itemId;
    public string itemName;
    public string prefabPath;

    public string KeyField => itemId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        itemId = reader[index++];
        itemName = reader[index++];
        prefabPath = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

public class ItemDataEntity : MainKeyEntityImpl
{
    public string itemId;
    public string prefabPath;

    public ItemDataEntity(ItemData src) : base(src)
    {
        itemId = src.itemId;
        prefabPath = src.prefabPath;
    }
}

public class ItemCollection : EntityCollectionImpl<ItemDataEntity>
{
    public ItemCollection(List<ItemDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}