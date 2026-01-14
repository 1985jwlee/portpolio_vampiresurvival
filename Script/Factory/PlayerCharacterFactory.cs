using System.Collections.Generic;
using Reflex;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerCharacterFactory : GameObjectFactory
    {
        private WeaponFactory weaponFactory;
        private TableDataHolder tableDataHolder;
        private MainGameSceneContextModel sceneModel;
        private IEntityContainer entityContainer;
        private DeviceInventoryManager inventoryManager;
        public PlayerCharacterFactory(Container _container, IEntityContainer _entityContainer, WeaponFactory _weaponFactory, TableDataHolder _tableDataHolder, MainGameSceneContextModel _sceneModel, DeviceInventoryManager _inventoryManager)

        {
            weaponFactory = _weaponFactory;
            tableDataHolder = _tableDataHolder;
            container = _container;
            sceneModel = _sceneModel;
            entityContainer = _entityContainer;
            inventoryManager = _inventoryManager;
        }

        protected override void OnCreateGameObject(Entity o, int pathHash)
        {
            var characterEntity = o.GetOrAddComponent<PlayerCharacterEntity>();
            
            characterEntity.weaponFactory = weaponFactory;
            characterEntity.SrcPathHashCode = pathHash;

            var selectedCharacterStat = sceneModel.characterDataEntity;
            characterEntity.OnCreateEntity();
            characterEntity.ApplyDefaultStatusData(sceneModel.characterDataEntity);

            if(string.IsNullOrEmpty(selectedCharacterStat.startDeviceId) == false)
            {
                var deviceBaseInfo = inventoryManager.GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, selectedCharacterStat.startDeviceId);
                var leveledDeviceInfo = inventoryManager.GetLeveledArcaneDeviceInfo(deviceBaseInfo, 1);
                inventoryManager.AddOrUpdateArcaneDevice(leveledDeviceInfo);
            }

            MessageBroker.Default.Publish(new KeyValuePair<string, (uint, float)>("playerCharacterCreate", (characterEntity.EGID, 55f)));
        }
    }
}
