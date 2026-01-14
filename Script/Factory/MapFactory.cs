using Reflex;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class MapFactory : GameObjectFactory
    {
        private MainGameSceneContextModel mainGameSceneContextModel;
        private EventTriggerFactory eventTriggerFactory;
        private TokenFactory tokenFactory;
        private InteractableFactory interactableFactory;
        private TableDataHolder tableDataHolder;
        private CustomRenderTexture crt;
        private Camera minimapCamera;

        public MapFactory(Container _container, MainGameSceneContextModel _mainGameSceneContextModel, EventTriggerFactory _eventTriggerFactory, TokenFactory _tokenFactory, InteractableFactory _interactableFactory, TableDataHolder _tableDataHolder)
        {
            container = _container;
            mainGameSceneContextModel = _mainGameSceneContextModel;
            eventTriggerFactory = _eventTriggerFactory;
            tokenFactory = _tokenFactory;
            interactableFactory = _interactableFactory;
            tableDataHolder = _tableDataHolder;
            var mapScope = _container.Children.First(child => child.Name == "Map");
            crt = mapScope.Resolve<CustomRenderTexture>();
            minimapCamera = mapScope.Resolve<Camera>();
        }

        protected override void OnCreateGameObject(Entity o, int pathHash)
        {
            o.SrcPathHashCode = pathHash;
            o.OnCreateEntity();
        }

        public void CreateMap()
        {
            var map = GameSettings.GetRandomMapLayout();

            var rows = map.GetLength(0);
            var cols = map.GetLength(1);
            var mapChunkSize = GameSettings.MapChunkSize;

            var defaultMaps = GameSettings.DefaultMapChunkNames.ToList();
            var startMaps = GameSettings.StartMapChunkNames.ToList();
            var teleportMaps = GameSettings.TeleportMapChunkNames.ToList();
            var eventMaps = GameSettings.EventMapChunkNames.ToList();
            var middleBossMaps = GameSettings.MiddleBossMapChunkNames.ToList();
            var finalBossMaps = GameSettings.FinalBossMapChunkNames.ToList();

            List<MapEntity> mapEntities = new List<MapEntity>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var mapPool = map[i, j] switch
                    {
                        0 => defaultMaps,
                        1 => startMaps,
                        2 => teleportMaps,
                        3 => eventMaps,
                        4 => middleBossMaps,
                        5 => finalBossMaps,
                        _ => null,
                    };
                    string mapName = mapPool.Random();
                    mapPool.Remove(mapName);

                    var gameObject = CreateGameObject($"WorldTileMap/{mapName}");

                    var mapEntity = gameObject.GetComponent<MapEntity>();

                    float positionX = (-mapChunkSize * cols / 2f) + (mapChunkSize * j) + (mapChunkSize / 2f);
                    float positionY = (mapChunkSize * rows / 2f) - (mapChunkSize * i) - (mapChunkSize / 2f);
                    Vector2 position = new Vector2(positionX, positionY);

                    mapEntity.transformImplement.position = position;

                    mapEntities.Add(mapEntity);
                }
            }

            minimapCamera.Render();

            foreach (var mapEntity in mapEntities)
            {
                if (mapEntity.startMapInfoImplement != null)
                {
                    mainGameSceneContextModel.startPosition = (Vector2)mapEntity.transformImplement.position + mapEntity.startMapInfoImplement.StartPositionProperty.statusValue;
                }

                if (mapEntity.settingImplement != null)
                {
                    SpawnItems(mapEntity.transformImplement.position, mapEntity.settingImplement);
                    SpawnTeleport(mapEntity.transformImplement.position, mapEntity.settingImplement);
                    SpawnSanctum(mapEntity.transformImplement.position, mapEntity.settingImplement);
                }

                if (mapEntity.triggerSettingImplement != null)
                {
                    SpawnTrigger(mapEntity.transformImplement.position, mapEntity.triggerSettingImplement);

                    if (mapEntity.triggerSettingImplement.BossSpawnSettingProperty.statusValue.bossId != 0)
                    {
                        
                        if (mapEntity.triggerSettingImplement.BossSpawnSettingProperty.statusValue.isFinalBoss)
                        {
                            mainGameSceneContextModel.bossTeleportPosition = (Vector2)mapEntity.transformImplement.position + mapEntity.triggerSettingImplement.BossSpawnSettingProperty.statusValue.teleportPosition;
                            mainGameSceneContextModel.mainBossIconPosition = (Vector2)mapEntity.transformImplement.position + mapEntity.triggerSettingImplement.BossSpawnSettingProperty.statusValue.iconPosition;
                        }
                        else
                        {
                            mainGameSceneContextModel.bossIconPositions.Add((Vector2)mapEntity.transformImplement.position + mapEntity.triggerSettingImplement.BossSpawnSettingProperty.statusValue.iconPosition);
                        }
                    }
                        
                }
            }

            mainGameSceneContextModel.mapBound = new Bounds(Vector3.zero, new Vector3(mapChunkSize * cols, mapChunkSize * rows));
        }

        private void SpawnTeleport(Vector2 mapPosition, EventMapSettingImplement settingImplement)
        {
            if(settingImplement.TeleportSpawnDataProperty.statusValue.shouldSpawnTeleport)
            {
                var go = interactableFactory.CreateGameObject(GameSettings.InteractableTeleportPath);
                if(go.TryGetComponent(out InteractableEntity interactable))
                {
                    var position = mapPosition + settingImplement.TeleportSpawnDataProperty.statusValue.spawnPosition;
                    interactable.transformImplement.position = position;
                    mainGameSceneContextModel.teleportIconPositions.Add(position);
                }
            }
        }

        private void SpawnSanctum(Vector2 mapPosition, EventMapSettingImplement settingImplement)
        {
            foreach (var sanctumSpawnSetting in settingImplement.sanctumSpawnSettings)
            {
                var path = sanctumSpawnSetting.statusValue.SanctumType switch
                {
                    SanctumType.Experience => GameSettings.InteractableSanctumExperiencePath,
                    SanctumType.Speed => GameSettings.InteractableSanctumSpeedPath,
                    SanctumType.Heal => GameSettings.InteractableSanctumHealPath,
                    _=> GameSettings.InteractableSanctumExperiencePath,
                };
                var go = interactableFactory.CreateGameObject(path);
                if (go.TryGetComponent(out InteractableEntity interactable))
                {
                    interactable.transformImplement.position = mapPosition + sanctumSpawnSetting.statusValue.spawnPosition;
                }
            }
        }

        private void SpawnItems(Vector2 mapPosition, EventMapSettingImplement settingImplement)
        {
            foreach (var expSpawnSetting in settingImplement.expSpawnSettings)
            {
                string path = GameSettings.GetExpTokenPrefabPath(expSpawnSetting.statusValue.value);
                var gameObejct = tokenFactory.CreateGameObject(path);

                var entity = gameObejct.GetComponent<ExpTokenEntity>();
                entity.transformImplement.position = mapPosition + expSpawnSetting.statusValue.spawnPosition;

                entity.applyBuffImplement.applyBuffList.Clear();
                entity.applyBuffImplement.applyBuffList.Add(new Buff
                {
                    statusValue = new BuffData
                    {
                        buffType = BuffType.ExpUp,
                        buffValue = expSpawnSetting.statusValue.value,
                        buffDeviceId = string.Empty,
                        remainTime = -1
                    }
                });
            }

            foreach (var itemSpawnData in settingImplement.itemSpawnSettings)
            {
                if(tableDataHolder.ItemCollection.TryGetEntity(itemSpawnData.statusValue.itemId.ToString(), out var itemDataEntity))
                {
                    var tokenGo = tokenFactory.CreateGameObject(itemDataEntity.prefabPath);
                    var tokenEntity = tokenGo.GetComponent<TokenEntity>();
                    tokenEntity.transformImplement.position = mapPosition + itemSpawnData.statusValue.spawnPosition;
                }
            }
        }

        private void SpawnTrigger(Vector2 mapPosition, EventMapTriggerSettingImplement triggerSettingImplement)
        {
            var triggerSetting = triggerSettingImplement.TriggerSettingProperty.statusValue;
            string resourceName = triggerSetting.eventTriggerShape switch
            {
                EventTriggerShape.Rect => "RectEventTrigger",
                EventTriggerShape.Circle => "CircleEventTrigger",
                _ => "RectEventTrigger",
            };

            var triggerGo = eventTriggerFactory.CreateGameObject($"Prefabs/EventTrigger/{resourceName}");
            if (triggerGo != null && triggerGo.TryGetComponent<EventTriggerEntity>(out var trigger))
            {
                trigger.transformImplement.position = mapPosition + triggerSetting.position;
                trigger.eventTriggerColliderImplement.ShouldRemoveAfterTriggeredProperty.statusValue = triggerSetting.shouldRemoveAfterTriggered;

                if (trigger.boxCollider != null)
                    trigger.boxCollider.size = triggerSetting.rectXMaxYMin * 2;

                if (trigger.circleCollider != null)
                    trigger.circleCollider.radius = triggerSetting.radius;

                trigger.enemySpawnImplement.enemySpawnDatas.Clear();

                if (triggerSettingImplement.BossSpawnSettingProperty.statusValue.bossId != 0)
                {
                    var bossSpawnSetting = triggerSettingImplement.BossSpawnSettingProperty.statusValue;
                    bossSpawnSetting.spawnPosition += mapPosition;
                    bossSpawnSetting.iconPosition += mapPosition;
                    trigger.bossSpawnImplement.BossSpawnDataProperty.statusValue = bossSpawnSetting;
                }

                foreach (var spawnSetting in triggerSettingImplement.enemySpawnSettings)
                {
                    trigger.enemySpawnImplement.enemySpawnDatas.Add(new EnemySpawnData()
                    {
                        statusValue = new EnemySpawnDataSet()
                        {
                            enemyId = spawnSetting.statusValue.enemyId,
                            spawnPosition = mapPosition + spawnSetting.statusValue.spawnPosition,
                        }
                    });
                }

                trigger.dummySpawnImplement.dummySpawnDatas.Clear();
                foreach (var spawnSetting in triggerSettingImplement.dummySpawnSettings)
                {
                    trigger.dummySpawnImplement.dummySpawnDatas.Add(new DummySpawnData()
                    {
                        statusValue = new DummySpawnDataSet()
                        {
                            dummyId = spawnSetting.statusValue.dummyId,
                            spawnPosition = mapPosition + spawnSetting.statusValue.spawnPosition,
                        }
                    });
                }
            }
        }
    }
}
