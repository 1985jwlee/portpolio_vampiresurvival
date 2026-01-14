using Client;
using System.Collections.Generic;

public struct ItemDropData : IMainKeyData
{
    public string itemDropId;
    public string itemDropName;
    public string NoItemProb;
    public string itemId01;
    public string itemProp01;
    public string itemId02;
    public string itemProp02;
    public string itemId03;
    public string itemProp03;
    public string itemId04;
    public string itemProp04;
    public string itemId05;
    public string itemProp05;
    public string itemId06;
    public string itemProp06;
    public string itemId07;
    public string itemProp07;
    public string itemId08;
    public string itemProp08;
    public string itemId09;
    public string itemProp09;
    public string itemId10;
    public string itemProp10;

    public string KeyField => itemDropId;

    public void OnReadData(CSVReader reader)
    {
        int index = 0;
        itemDropId = reader[index++];
        itemDropName = reader[index++];
        NoItemProb = reader[index++];
        itemId01 = reader[index++];
        itemProp01 = reader[index++];
        itemId02 = reader[index++];
        itemProp02 = reader[index++];
        itemId03 = reader[index++];
        itemProp03 = reader[index++];
        itemId04 = reader[index++];
        itemProp04 = reader[index++];
        itemId05 = reader[index++];
        itemProp05 = reader[index++];
        itemId06 = reader[index++];
        itemProp06 = reader[index++];
        itemId07 = reader[index++];
        itemProp07 = reader[index++];
        itemId08 = reader[index++];
        itemProp08 = reader[index++];
        itemId09 = reader[index++];
        itemProp09 = reader[index++];
        itemId10 = reader[index++];
        itemProp10 = reader[index++];
    }
}

#if UNITY_EDITOR
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

public class ItemDropDataEntity : MainKeyEntityImpl
{
    public string itemDropId;
    public List<uint> itemIds; // EnemyEntity 에 설정해주기 위해 uint로 정의함.
    public List<int> itemProps;

    public ItemDropDataEntity(ItemDropData src) : base(src)
    {
        itemDropId = src.itemDropId;


        string[] ids = new string[] { src.itemId01, src.itemId02, src.itemId03, src.itemId04, src.itemId05, src.itemId06, src.itemId07, src.itemId08, src.itemId09, src.itemId10 };

        itemIds = new List<uint>();
        foreach (var id in ids)
        {
            if (string.IsNullOrEmpty(id) == false)
            {
                if (uint.TryParse(id, out var parsedUint))
                    itemIds.Add(parsedUint);
            }
        }

        string[] props = new string[] { src.itemProp01, src.itemProp02, src.itemProp03, src.itemProp04, src.itemProp05, src.itemProp06, src.itemProp07, src.itemProp08, src.itemProp09, src.itemProp10 };

        itemProps = new List<int>();
        foreach(var prop in props)
        {
            if (string.IsNullOrEmpty(prop) == false)
            {
                if (int.TryParse(prop, out var parsedProp))
                    itemProps.Add(parsedProp);
            }
        }
    }
}

public class ItemDropCollection : EntityCollectionImpl<ItemDropDataEntity>
{
    public ItemDropCollection(List<ItemDropDataEntity> list)
    {
        foreach (var e in list)
        {
            entities.Add(e.MainKey, e);
        }
    }
}