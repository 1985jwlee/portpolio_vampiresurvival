using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.ECS
{
    public class DeviceInventoryManager
    {
        private readonly TableDataHolder tableDataHolder;
        private readonly IEntityContainer entityContainer;
        private readonly MainGameSceneContextModel sceneModel;
        private readonly SummonObjectFactory summonObjectFactory;

        private DeviceInventory _deviceInventory;

        private DeviceInventory DeviceInventory
        {
            get
            {
                if (_deviceInventory == null)
                    _deviceInventory = new DeviceInventory(entityContainer, sceneModel.characterDataEntity, sceneModel.supporterCharacter0DataEntity, sceneModel.supporterCharacter1DataEntity, tableDataHolder, summonObjectFactory);
                return _deviceInventory;
            }
        }

        public DeviceInventoryManager(IEntityContainer _entityContainer, TableDataHolder _tableDataHolder, MainGameSceneContextModel _sceneModel, SummonObjectFactory _summonObjectFactory)
        {
            tableDataHolder = _tableDataHolder;
            entityContainer = _entityContainer;
            sceneModel = _sceneModel;
            summonObjectFactory = _summonObjectFactory;
        }

        public bool Containes(ILeveledArcaneDeviceInfo arcaneDeviceInfo) => DeviceInventory.Contains(arcaneDeviceInfo);

        public void AddOrUpdateArcaneDevice(IArcaneDeviceInfo device, int level) => AddOrUpdateArcaneDevice(GetLeveledArcaneDeviceInfo(device, level));

        public void AddOrUpdateArcaneDevice(ILeveledArcaneDeviceInfo chosenDevice)
        {
            if(chosenDevice.DeviceType == ArcaneDeviceType.ADVANCED_ACTIVE && chosenDevice.DeviceLevel == 1)
            {
                if(tableDataHolder.arcaneDeviceDict.TryGetValue(chosenDevice.GetArcaneDeviceId, out var arcaneDeviceInfo))
                {
                    if(arcaneDeviceInfo.AdvanceInfo != null)
                    {
                        var advanceInfo = arcaneDeviceInfo.AdvanceInfo;

                        if (advanceInfo.HasValue)
                        {
                            DeviceInventory.ReplaceDevice(GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advanceInfo.Value.MainActiveId), chosenDevice);
                            DeviceInventory.RemoveDevice(GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advanceInfo.Value.SubActiveId));
                        }
                        else
                        {
                            DeviceInventory.AddOrUpdateDevice(chosenDevice);
                        }
                    }
                }
            }
            else
            {
                DeviceInventory.AddOrUpdateDevice(chosenDevice);
            }
        }

        public ILeveledArcaneDeviceInfo GetDeviceAtSlotIndex(int slotIndex) => DeviceInventory.GetDeviceAtSlotIndex(slotIndex);
        public ILeveledArcaneDeviceInfo GetDeviceAtTrimIndex(int trimIndex) => DeviceInventory.GetDeviceAtTrimIndex(trimIndex);

        public int GetSlotNum() => DeviceInventory.GetSlotNum();
        public DeviceSlot GetSlotType(int slotIndex) => DeviceInventory.GetSlotType(slotIndex);

        public int GetDeviceNum() => DeviceInventory.GetDeviceNum();

        public int GetMaxLevelDeviceNum()
        {
            bool Predict(ILeveledArcaneDeviceInfo leveledDeviceInfo)
            {
                tableDataHolder.arcaneDeviceDict.TryGetValue(leveledDeviceInfo.GetArcaneDeviceId, out var deviceInfo);
                return leveledDeviceInfo.DeviceLevel == deviceInfo.MaxLevel;
            }
            return DeviceInventory.GetDeviceNum(Predict);
        }

        public List<ILeveledArcaneDeviceInfo> GetChoosableDeviceList(int choosableNum, bool includeAdd, bool includeAdvanced)
        {
            List<ILeveledArcaneDeviceInfo> possibleDeviceList = new();
            List<float> probabilities = new();

            foreach (var device in tableDataHolder.arcaneDeviceDict.Values)
            {
                var infoAndProbability = GetNextLevelAndPossibility(device, includeAdd, includeAdvanced);
                if (infoAndProbability.nextLevelDeviceInfo != null && infoAndProbability.probability != 0)
                {
                    possibleDeviceList.Add(infoAndProbability.nextLevelDeviceInfo);
                    probabilities.Add(infoAndProbability.probability);
                }
            }

            List<ILeveledArcaneDeviceInfo> result = new();
            int num = Mathf.Min(possibleDeviceList.Count, choosableNum);
            for (int i = 0; i < num; i++)
            {
                var randomIndex = probabilities.WeightedRandomIndex();
                var selectedArcaneDevice = possibleDeviceList[randomIndex];

                possibleDeviceList.RemoveAt(randomIndex);
                probabilities.RemoveAt(randomIndex);

                result.Add(selectedArcaneDevice);
            }

            return result;
        }

        public Sprite LoadDeviceIcon(IArcaneDeviceInfo deviceInfo) => Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Com_Device_165x165").GetSprite($"UI_Icon_Device_{deviceInfo.FileName}");
        public Sprite LoadDeviceIcon(ILeveledArcaneDeviceInfo deviceInfo) => Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Com_Device_165x165").GetSprite($"UI_Icon_Device_{deviceInfo.FileName}");

        private (ILeveledArcaneDeviceInfo nextLevelDeviceInfo, float probability) GetNextLevelAndPossibility(IArcaneDeviceInfo device, bool includeAdd, bool includeAdvanced)
        {
            var foundedDevice = DeviceInventory.FindDevice(device);
            if (foundedDevice != null)
            {
                if (foundedDevice.DeviceLevel == device.MaxLevel)
                    return (null, 0);

                float probability = GameSettings.OwnedDeviceProbability;
                int level = foundedDevice.DeviceLevel + 1;

                return (GetLeveledArcaneDeviceInfo(device, level), probability);
            }

            // ToDo: 전용 디바이스 처리, 현재 빌드 2차 스펙 상 모든 전용 디바이스가 나오므로 이를 처리하지 않음.
            //if (device.DeviceType == ArcaneDeviceType.CHARACTER_OWNED_ACTIVE)
            //    return (null, 0);

            // ToDo: 지울 것. 임시 처리.
            if (device.DeviceType == ArcaneDeviceType.ACTIVE && int.TryParse(device.DeviceId, out var idNum) && idNum >= 20)
                return (null, 0);

            if (includeAdvanced && device.DeviceType == ArcaneDeviceType.ADVANCED_ACTIVE && IsUpgradeDeviceRequirementAchieved(device))
                return (GetLeveledArcaneDeviceInfo(device, 1), GameSettings.AdvanceDeviceProbability);
            if (includeAdd && device.DeviceType != ArcaneDeviceType.ADVANCED_ACTIVE && DeviceInventory.CanAdd(device.DeviceSlot) && (HasUsedAsAdvanceRequirement(device) == false))
                return (GetLeveledArcaneDeviceInfo(device, 1), GameSettings.DefaultDeviceProbability);

            return (null, 0);
        }

        public IArcaneDeviceInfo GetArcaneDeviceInfo(ArcaneDeviceType deviceType, string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return null;
            }
            
            string keyPrefix = ExtensionFunction.GetPrefixStringByArcaneDeviceType(deviceType);
            string key = $"{keyPrefix}_{deviceId}";

            if (tableDataHolder.arcaneDeviceDict.TryGetValue(key, out var output))
                return output;
            if (deviceType == ArcaneDeviceType.ACTIVE && tableDataHolder.arcaneDeviceDict.TryGetValue($"OwnActive_{deviceId}", out var ownOutput))
                return ownOutput;
           
            return null;
        }

        public ILeveledArcaneDeviceInfo GetLeveledArcaneDeviceInfo(IArcaneDeviceInfo deviceInfo, int deviceLevel)
        {
            switch (deviceInfo.DeviceType)
            {
                case ArcaneDeviceType.CHARACTER_OWNED_ACTIVE:
                    {
                        tableDataHolder.CharacterOwnedDeviceCollection.TryGetEntity(deviceInfo.DeviceId, out var deviceGroupEntity);
                        deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity);
                        return activeDeviceEntity;
                    }
                case ArcaneDeviceType.ACTIVE:
                    {
                        tableDataHolder.ActiveDeviceCollection.TryGetEntity(deviceInfo.DeviceId, out var deviceGroupEntity);
                        deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity);
                        return activeDeviceEntity;
                    }
                case ArcaneDeviceType.PASSIVE:
                    {
                        tableDataHolder.PassiveDeviceCollection.TryGetEntity(deviceInfo.DeviceId, out var deviceGroupEntity);
                        deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var passiveDeviceEntity);
                        return passiveDeviceEntity;
                    }
                case ArcaneDeviceType.ADVANCED_ACTIVE:
                    {
                        tableDataHolder.AdvActiveDeviceCollection.TryGetEntity(deviceInfo.DeviceId, out var deviceGroupEntity);
                        deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity);
                        return activeDeviceEntity;
                    }
            }
            
            return null;
        }

        private bool IsUpgradeDeviceRequirementAchieved(IArcaneDeviceInfo advencedDevice)
        {
            var advenceInfo = advencedDevice.AdvanceInfo;
            if (advenceInfo.HasValue)
            {
                var activeMain = GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advenceInfo.Value.MainActiveId);
                var activeMainInInventory = DeviceInventory.FindDevice(activeMain);
                if (activeMainInInventory == null || activeMain.MaxLevel != activeMainInInventory.DeviceLevel)
                    return false;
                
                if (string.IsNullOrEmpty(advenceInfo.Value.SubPassiveId) == false)
                {
                    var passive = GetArcaneDeviceInfo(ArcaneDeviceType.PASSIVE, advenceInfo.Value.SubPassiveId);
                    return DeviceInventory.Contains(passive);
                }
                
                if (string.IsNullOrEmpty(advenceInfo.Value.SubActiveId) == false)
                {
                    var activeSub = GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advenceInfo.Value.SubActiveId);
                    var activeSubInInventory = DeviceInventory.FindDevice(activeSub);
                    if (activeSub.MaxLevel == activeSubInInventory.DeviceLevel)
                        return true;
                }
            }

            return false;
        }

        private bool HasUsedAsAdvanceRequirement(IArcaneDeviceInfo device)
        {
            int deviceNum = DeviceInventory.GetDeviceNum();

            for(int i = 0; i < deviceNum; i++)
            {
                var deviceInInventory = DeviceInventory.GetDeviceAtTrimIndex(i);
                if(deviceInInventory.DeviceType == ArcaneDeviceType.ADVANCED_ACTIVE)
                {
                    var deviceInfo = GetArcaneDeviceInfo(deviceInInventory.DeviceType, deviceInInventory.DeviceId);
                    var advanceInfo = deviceInfo.AdvanceInfo;
                    if (advanceInfo.HasValue)
                    {
                        if (advanceInfo.Value.MainActiveId == device.DeviceId || advanceInfo.Value.SubActiveId == device.DeviceId)
                            return true;    
                    }
                }
            }

            return false;
        }
    }

    class DeviceInventory
    {
        private TableDataHolder tableDataHolder;
        private SummonObjectFactory summonObjectFactory;
        private IEntityContainer entityContainer;

        private readonly Dictionary<DeviceSlot, SlotSet> slots = new();

        public DeviceInventory(IEntityContainer _entityContainer, CharacterDataEntity characterDataEntity, CharacterDataEntity supporter0DataEntity, CharacterDataEntity supporter1DataEntity, TableDataHolder _tableDataHolder, SummonObjectFactory _summonObjectFactory)
        {
            entityContainer = _entityContainer;
            tableDataHolder = _tableDataHolder;
            summonObjectFactory = _summonObjectFactory;
            var slotTypes = GetSlotTypes();
            foreach (var slotType in slotTypes)
            {
                int slotNum = characterDataEntity.GetSlotNum(slotType);
                if(supporter0DataEntity.supporterSlotType == slotType)
                    slotNum++;
                if (supporter1DataEntity.supporterSlotType == slotType)
                    slotNum++;

                slots.Add(slotType, new SlotSet() { capacity = slotNum, slots = new() });
            }
        }

        public IEnumerable<DeviceSlot> GetSlotTypes() => System.Enum.GetValues(typeof(DeviceSlot)).Cast<DeviceSlot>();

        public DeviceSlot GetSlotType(int slotIndex)
        {
            var allSlots = System.Enum.GetValues(typeof(DeviceSlot)).Cast<DeviceSlot>();
            foreach (var slot in allSlots)
            {
                int slotNum = GetSlotNum(slot);
                if (slotIndex < slotNum)
                    return slot;
                slotIndex -= slotNum;
            }

            return DeviceSlot.WeaponSlot;
        }

        public int GetSlotNum()
        {
            var allSlots = System.Enum.GetValues(typeof(DeviceSlot)).Cast<DeviceSlot>();
            int sum = 0;
            foreach (var slot in allSlots)
            {
                sum += GetSlotNum(slot);
            }
            return sum;
        }

        public int GetSlotNum(DeviceSlot slot) => slots[slot].capacity;

        public ILeveledArcaneDeviceInfo FindDevice(IArcaneDeviceInfo device) => slots[device.DeviceSlot].GetDevice(device);

        public bool CanAdd(DeviceSlot slot) => slots[slot].CanAdd;

        public bool Contains(IArcaneDeviceInfo chosenDevice) => slots[chosenDevice.DeviceSlot].Contains(chosenDevice);

        public bool Contains(ILeveledArcaneDeviceInfo leveledChosenDevice) => slots[leveledChosenDevice.DeviceSlot].Contains(leveledChosenDevice);

        public ILeveledArcaneDeviceInfo GetDeviceAtSlotIndex(int slotIndex)
        {
            int counter = 0;
            foreach (var slotSet in slots.Values)
            {
                if (slotIndex < counter + slotSet.capacity)
                    return slotSet.GetDeviceAt(slotIndex - counter);
                counter += slotSet.capacity;
            }
            return null;
        }

        public ILeveledArcaneDeviceInfo GetDeviceAtTrimIndex(int trimIndex)
        {
            int counter = 0;
            foreach (var slotSet in slots.Values)
            {
                if (trimIndex < counter + slotSet.slots.Count)
                    return slotSet.GetDeviceAt(trimIndex - counter);
                counter += slotSet.slots.Count;
            }
            return null;
        }

        public int GetDeviceNum()
        {
            int num = 0;

            var slotTypes = GetSlotTypes();
            foreach (var slotType in slotTypes)
                num += slots[slotType].slots.Count;

            return num;
        }

        public int GetDeviceNum(System.Func<ILeveledArcaneDeviceInfo, bool> predict)
        {
            int num = 0;

            var slotTypes = GetSlotTypes();
            foreach (var slotType in slotTypes)
                num += slots[slotType].slots.Count(predict);

            return num;
        }

        public void AddOrUpdateDevice(ILeveledArcaneDeviceInfo chosenDevice)
        {
            slots[chosenDevice.DeviceSlot].AddOrUpdate(chosenDevice);
            AddOrUpdateToImpl(chosenDevice);
        }

        public void AddOrUpdateToImpl(ILeveledArcaneDeviceInfo chosenDevice)
        {
            var deviceId = chosenDevice.DeviceId;
            var deviceLevel = chosenDevice.DeviceLevel;
            
            switch (chosenDevice.DeviceType)
            {
                case ArcaneDeviceType.PASSIVE:
                    {
                        tableDataHolder.PassiveDeviceCollection.TryGetEntity(deviceId, out var deviceGroupEntity);
                        deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var passiveDeviceEntity);
                        var passiveDeviceData = new PassiveDeviceEquipData() { statusValue = ToPassiveDeviceDataSet(passiveDeviceEntity) };
                        entityContainer.playerCharacterEntity.passiveDeviceInventoryImplement.AddOrUpdatePassiveDevice(passiveDeviceData);

                        entityContainer.playerCharacterEntity.buffImplement.appliedBuff.RemoveAll(elmt => elmt.statusValue.buffDeviceId == passiveDeviceEntity.deviceId);
                        foreach (var buffInfo in passiveDeviceEntity.buffInfos)
                        {
                            var statusValue = new BuffData() { buffDeviceId = passiveDeviceEntity.deviceId, buffType = buffInfo.statusType, buffValue = buffInfo.value, remainTime = float.MaxValue };
                            entityContainer.playerCharacterEntity.buffImplement.appliedBuff.Add(new Buff() { statusValue = statusValue });
                        }
                        entityContainer.playerCharacterEntity.OnApplyPassiveArcaneDevice();
                    }
                    break;
                case ArcaneDeviceType.ADVANCED_ACTIVE:
                    {
                        tableDataHolder.AdvActiveDeviceCollection.TryGetEntity(deviceId, out var deviceGroupEntity);
                        if (deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity))
                            AddActiveDevice(activeDeviceEntity);
                    }
                    break;
                case ArcaneDeviceType.ACTIVE:
                    {
                        tableDataHolder.ActiveDeviceCollection.TryGetEntity(deviceId, out var deviceGroupEntity);
                        if (deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity))
                            AddActiveDevice(activeDeviceEntity);
                    }
                    break;
                case ArcaneDeviceType.CHARACTER_OWNED_ACTIVE:
                    {
                        tableDataHolder.CharacterOwnedDeviceCollection.TryGetEntity(deviceId, out var deviceGroupEntity);
                        if (deviceGroupEntity.GroupEntity.TryGetValue(deviceLevel.ToString(), out var activeDeviceEntity))
                            AddActiveDevice(activeDeviceEntity);
                    }
                    break;
            }
        }

        private void AddActiveDevice(IActiveDeviceDataEntity activeDeviceEntity)
        {
            entityContainer.playerCharacterEntity.weaponInventoryImplement.AddOrUpdateWeapon(new WeaponData() { statusValue = ToWeaponDataSet(activeDeviceEntity) });
            if (activeDeviceEntity.SummonCreation)
            {
                ArcaneDeviceSummonEntity arcaneDeviceSummonEntity = null;
                var summonEntityChanged = false;
                if (activeDeviceEntity.DeviceLevel > 1)
                {
                    if (int.TryParse(activeDeviceEntity.DeviceId, out var tableIndex))
                    {
                        var list = entityContainer.GetEntities<ArcaneDeviceSummonEntity, TableIndexDataImplement>();
                        foreach (var e in list)
                        {
                            if (e.tableIndexDataImplement.tableDataIndexNoProperty.statusValue == tableIndex)
                            {
                                arcaneDeviceSummonEntity = e;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var summonObject = summonObjectFactory.CreateGameObject($"Prefabs/Summon/{activeDeviceEntity.FileName}");
                    summonObject.TryGetComponent(out arcaneDeviceSummonEntity);
                    if (arcaneDeviceSummonEntity != null && int.TryParse(activeDeviceEntity.DeviceId, out var tableIndex))
                    {
                        arcaneDeviceSummonEntity.tableIndexDataImplement.tableDataIndexNoProperty.statusValue = tableIndex;
                        summonEntityChanged = true;
                    }
                }

                if (arcaneDeviceSummonEntity != null)
                {
                    var splitAttackDeviceArray = activeDeviceEntity.SummonAttackDeviceId.Split(':');
                    foreach (var deviceId in splitAttackDeviceArray)
                    {
                        if (tableDataHolder.SummonActiveDeviceCollection.TryGetEntity(deviceId, out var summonDeviceGroup)
                            && summonDeviceGroup.GroupEntity.TryGetValue(activeDeviceEntity.DeviceLevel.ToString(), out var summonAttackEntity))
                        {
                            arcaneDeviceSummonEntity.weaponInventoyImplement.AddOrUpdateWeapon(new WeaponData() { statusValue = ToSummonWeaponDataSet(summonAttackEntity) });
                        }
                    }

                    if (summonEntityChanged)
                    {
                        arcaneDeviceSummonEntity.OnApplyEntityComponent();
                    }     
                }
            }
        }

        public bool RemoveDevice(IArcaneDeviceInfo deviceToRemove)
        {
            if (deviceToRemove == null)
            {
                return false;
            }

            if (slots[deviceToRemove.DeviceSlot].Remove(deviceToRemove) == false)
            {
                return false;
            }

            if (RemoveFromImpl(deviceToRemove) == false)
            {
                return false;
            }

            return true;
        }

        public bool RemoveFromImpl(IArcaneDeviceInfo arcaneDeviceInfo)
        {
            switch (arcaneDeviceInfo.DeviceType)
            {
                case ArcaneDeviceType.CHARACTER_OWNED_ACTIVE:
                case ArcaneDeviceType.ACTIVE:
                case ArcaneDeviceType.ADVANCED_ACTIVE:
                    {
                        var weaponData = entityContainer.playerCharacterEntity.weaponInventoryImplement.WeaponDatas.First(elmt => elmt.statusValue.id == arcaneDeviceInfo.DeviceId);
                        
                        if (weaponData.statusValue.isSummonCreation)
                        {
                            if (int.TryParse(arcaneDeviceInfo.DeviceId, out var tableIndex))
                            {
                                var list = entityContainer.GetEntities<ArcaneDeviceSummonEntity, TableIndexDataImplement>();
                                foreach (var e in list)
                                {
                                    if (e.tableIndexDataImplement.tableDataIndexNoProperty.statusValue == tableIndex)
                                    {
                                        var splitAttackDeviceArray = weaponData.statusValue.summonAttackDeviceId.Split(':');
                                        foreach (var deviceId in splitAttackDeviceArray)
                                        {
                                            e.OnRemoveEntity();
                                            summonObjectFactory.EnqueRecycle(e, e.SrcPathHashCode);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        return entityContainer.playerCharacterEntity.weaponInventoryImplement.RemoveWeapon(arcaneDeviceInfo.DeviceId);
                    }
                default:
                    return false;
            }
        }

        internal bool ReplaceDevice(IArcaneDeviceInfo deviceToRemove, ILeveledArcaneDeviceInfo deviceToAdd)
        {
            if (slots[deviceToRemove.DeviceSlot].Replace(deviceToRemove, deviceToAdd) == false)
            {
                return false;
            }

            if(RemoveFromImpl(deviceToRemove) == false)
            {
                return false;
            }

            AddOrUpdateToImpl(deviceToAdd);

            return true;
        }

        public static WeaponDataSet ToWeaponDataSet(IActiveDeviceDataEntity entity)
        {
            return new WeaponDataSet()
            {
                id = entity.DeviceId,
                level = entity.DeviceLevel,
                damage = entity.Damage,
                coolTime = entity.CoolDown,
                isSingleCreation = entity.SingleCreation,
                isSummonCreation = entity.SummonCreation,
                summonAttackDeviceId = entity.SummonAttackDeviceId,
                prefabPath = $"Prefabs/Projectile/{entity.FileName}",
                createCount = entity.Projectile,
                projectileSpeed = entity.ProjectileSpeed,
                multiCreationTick = entity.ProjectileInterval,
                criticalRatio = entity.CriticalMultiple,
                criticalProbability = entity.CriticalChance,
                spearCount = entity.Pierce,
                knockBack = entity.KnockBack,
                attackType = entity.Slot == DeviceSlot.WeaponSlot ? AttackType.PHYSICS : AttackType.MAGIC,
                area = entity.Area,
                isGroupAttack =  entity.GroupCount > 0,
                groupCount = entity.GroupCount,
                mustHaveTarget = entity.MustHaveTarget,
                lifeDuration = entity.Duration
            };
        }
        
        public static WeaponDataSet ToSummonWeaponDataSet(SummonActiveDeviceEntity entity)
        {
            return new WeaponDataSet()
            {
                id = entity.deviceId,
                level = entity.deviceLevel,
                damage = entity.damage,
                coolTime = entity.coolDown,
                isSingleCreation = entity.singleCreation,
                isSummonCreation = false,
                prefabPath = $"Prefabs/Projectile/{entity.fileName}",
                createCount = entity.projectile,
                projectileSpeed = entity.projectileSpeed,
                multiCreationTick = entity.projectileInterval,
                criticalRatio = entity.criticalMultiple,
                criticalProbability = entity.criticalChance,
                spearCount = entity.pierce,
                knockBack = entity.knockBack,
                attackType = entity.slot == DeviceSlot.WeaponSlot ? AttackType.PHYSICS : AttackType.MAGIC,
                area = entity.area,
                mustHaveTarget = entity.mustHaveTarget,
                lifeDuration = entity.duration
            };
        }

        public static PassiveDeviceEquipDataSet ToPassiveDeviceDataSet(PassiveDeviceEntity entity)
        {
            return new PassiveDeviceEquipDataSet()
            {
                id = entity.deviceId,
                level = entity.deviceLevel,
            };
        }

        struct SlotSet
        {
            public int capacity;
            public List<ILeveledArcaneDeviceInfo> slots;

            public void AddOrUpdate(ILeveledArcaneDeviceInfo device)
            {
                int index = slots.FindIndex(elmt => elmt.DeviceType == device.DeviceType && elmt.DeviceId == device.DeviceId);
                if (index != -1)
                {
                    slots[index] = device;
                    return;
                }
                slots.Add(device);
            }

            public bool Remove(IArcaneDeviceInfo device)
            {
                int index = slots.FindIndex(elmt => elmt.DeviceType == device.DeviceType && elmt.DeviceId == device.DeviceId);
                if (index != -1)
                {
                    slots.RemoveAt(index);
                    return true;
                }

                return false;
            }

            public bool Replace(IArcaneDeviceInfo deviceToRemove, ILeveledArcaneDeviceInfo deviceToAdd)
            {
                int index = slots.FindIndex(elmt => elmt.DeviceType == deviceToRemove.DeviceType && elmt.DeviceId == deviceToRemove.DeviceId);
                if (index != -1)
                {
                    slots[index] = deviceToAdd;
                    return true;
                }

                return false;
            }

            public bool CanAdd => slots.Count < capacity;

            public bool Contains(IArcaneDeviceInfo device) => slots.Any(elmt => elmt.DeviceType == device.DeviceType && elmt.DeviceId == device.DeviceId);

            public bool Contains(ILeveledArcaneDeviceInfo device) => slots.Any(elmt => elmt.DeviceType == device.DeviceType && elmt.DeviceId == device.DeviceId);

            public ILeveledArcaneDeviceInfo GetDevice(IArcaneDeviceInfo device) => slots.Find(elmt => elmt.DeviceType == device.DeviceType && elmt.DeviceId == device.DeviceId);

            public ILeveledArcaneDeviceInfo GetDeviceAt(int index)
            {
                return index < slots.Count ? slots[index] : null;
            }

        }
    }
}
