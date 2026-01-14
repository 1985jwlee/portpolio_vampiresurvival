using Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.ECS;
using UnityEngine;

[CreateAssetMenu(fileName = "TableDataHolder", menuName = "Holders/TableDataHolder")]
public class TableDataHolder : ScriptableObject
{
    [SerializeField] private TextAsset characterData;
    [SerializeField] private TextAsset enemyData;
    [SerializeField] private TextAsset activeDeviceData;
    [SerializeField] private TextAsset characterOwnedActiveDeviceData;
    [SerializeField] private TextAsset advancedActiveDeviceData;
    [SerializeField] private TextAsset passiveDeviceData;
    [SerializeField] private TextAsset summonActiveDeviceData;
    [SerializeField] private TextAsset enemyWeaponData;
    [SerializeField] private TextAsset itemDropData;
    [SerializeField] private TextAsset itemData;
    [SerializeField] private TextAsset mapObjectData;
    [SerializeField] private List<TextAsset> stageData;

    public CharacterCollection CharacterCollection { get; private set; }
    public EnemyCollection EnemyCollection { get; private set; }
    public ActiveDeviceCollection ActiveDeviceCollection { get; private set; }
    public AdvancedActiveDeviceCollection AdvActiveDeviceCollection { get; private set; }
    public PassiveDeviceCollection PassiveDeviceCollection { get; private set; }
    public SummonActiveDeviceCollection SummonActiveDeviceCollection { get; private set; }
    public EnemyWeaponCollection EnemyWeaponCollection { get; private set; }
    public ItemDropCollection ItemDropCollection { get; private set; }
    public ItemCollection ItemCollection { get; private set; }
    public MapObjectCollection MapObjectCollection { get; private set; }

    public CharacterDependDeviceCollection CharacterOwnedDeviceCollection { get; private set; }

    public readonly Dictionary<string, IArcaneDeviceInfo> arcaneDeviceDict = new Dictionary<string, IArcaneDeviceInfo>();
    public readonly Dictionary<string, StageLevelTimelineCollection> stageTimelineCollections = new Dictionary<string, StageLevelTimelineCollection>();
    
    public void LoadCSV()
    {
        LoadCSV<EnemyData, EnemyDataEntity>(new EnemyData(), enemyData, output => EnemyCollection = new EnemyCollection(output));
        LoadCSV<CharacterData, CharacterDataEntity>(new CharacterData(), characterData, output => CharacterCollection = new CharacterCollection(output));

        LoadCSV<ActiveDeviceData, ActiveDeviceEntity>(new ActiveDeviceData(), activeDeviceData, output => ActiveDeviceCollection = new ActiveDeviceCollection(output));
        LoadCSV<PassiveDeviceData, PassiveDeviceEntity>(new PassiveDeviceData(), passiveDeviceData, output => PassiveDeviceCollection = new PassiveDeviceCollection(output));
        LoadCSV<SummonActiveDeviceData, SummonActiveDeviceEntity>(new SummonActiveDeviceData(), summonActiveDeviceData, output => SummonActiveDeviceCollection = new SummonActiveDeviceCollection(output));
        LoadCSV<EnemyWeaponData, EnemyWeaponDataEntity>(new EnemyWeaponData(), enemyWeaponData, output => EnemyWeaponCollection = new EnemyWeaponCollection(output));
        LoadCSV<AdvancedActiveDeviceData, AdvancedActiveDeviceEntity>(new AdvancedActiveDeviceData(), advancedActiveDeviceData, output => AdvActiveDeviceCollection = new AdvancedActiveDeviceCollection(output));
        LoadCSV<CharacterDependDeviceData, CharacterDependDeviceEntity>(new CharacterDependDeviceData(), characterOwnedActiveDeviceData, output => CharacterOwnedDeviceCollection = new CharacterDependDeviceCollection(output));

        LoadCSV<ItemDropData, ItemDropDataEntity>(new ItemDropData(), itemDropData, output => ItemDropCollection = new ItemDropCollection(output));
        LoadCSV<ItemData, ItemDataEntity>(new ItemData(), itemData, output => ItemCollection = new ItemCollection(output));

        LoadCSV<MapObjectData, MapObjectDataEntity>(new MapObjectData(), mapObjectData, output => MapObjectCollection = new MapObjectCollection(output));

        var activeDeviceList = ActiveDeviceCollection.GetIterator().Select(groupEntity => new ArcaneDeviceInfo(groupEntity));
        var passiveDeviceList = PassiveDeviceCollection.GetIterator().Select(groupEntity => new ArcaneDeviceInfo(groupEntity));
        var advancedActiveDeviceList = AdvActiveDeviceCollection.GetIterator().Select(groupEntity => new ArcaneDeviceInfo(groupEntity));
        var characterOwnedActiveDeviceList = CharacterOwnedDeviceCollection.GetIterator().Select(groupEntity => new ArcaneDeviceInfo(groupEntity));

        foreach (var device in activeDeviceList)
            arcaneDeviceDict.Add($"Active_{device.DeviceId}", device);
        foreach (var device in passiveDeviceList)
            arcaneDeviceDict.Add($"Passive_{device.DeviceId}", device);
        foreach(var device in advancedActiveDeviceList)
            arcaneDeviceDict.Add($"AdvActive_{device.DeviceId}", device);
        foreach (var device in characterOwnedActiveDeviceList)
            arcaneDeviceDict.Add($"OwnActive_{device.DeviceId}", device);

        foreach (TextAsset asset in stageData)
            LoadCSV<StageLevelTimelineData, StageLevelTimelineEntity>(new StageLevelTimelineData(), asset, output => { stageTimelineCollections.Add(asset.name, new StageLevelTimelineCollection(output)); });
    }

    private static void LoadCSV<T, V>(T dataType, TextAsset asset, System.Action<List<V>> onExecute) where T : IKeyData, new() where V : Client.IEntity
    {
        var output = new List<V>();

        var entityType = typeof(V);

        using (StringReader textReader = new(asset.text))
        {
            CSVReader csvReader = new(textReader);
            if (dataType is ISubKeyData subKeyData)
            {
                var lineCount = 0;
                while (csvReader.Read())
                {
                    if (lineCount++ < 1)
                    {
                        continue;
                    }
                    subKeyData.OnReadData(csvReader);
                    if (string.IsNullOrEmpty(subKeyData.KeyField) && string.IsNullOrEmpty(subKeyData.SubKeyField))
                    {
                        continue;
                    }
                    V v = (V)System.Activator.CreateInstance(entityType, subKeyData);
                    output.Add(v);
                }
            }
            else if (dataType is IDualKeyData dualKeyData)
            {
                var lineCount = 0;
                while (csvReader.Read())
                {
                    if (lineCount++ < 1)
                    {
                        continue;
                    }
                    dualKeyData.OnReadData(csvReader);
                    if (string.IsNullOrEmpty(dualKeyData.KeyFields.Item1) && string.IsNullOrEmpty(dualKeyData.KeyFields.Item2))
                    {
                        continue;
                    }
                    V v = (V)System.Activator.CreateInstance(entityType, dualKeyData);
                    output.Add(v);
                }
            }
            else if (dataType is IMainKeyData mainKeyData)
            {
                var lineCount = 0;
                while (csvReader.Read())
                {
                    if (lineCount++ < 1)
                    {
                        continue;
                    }
                    mainKeyData.OnReadData(csvReader);
                    if (string.IsNullOrEmpty(mainKeyData.KeyField))
                    {
                        continue;
                    }
                    V v = (V)System.Activator.CreateInstance(entityType, mainKeyData);
                    output.Add(v);
                }
            }
            onExecute.Invoke(output);
        }
    }
}
