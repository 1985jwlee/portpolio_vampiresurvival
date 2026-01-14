using System.Collections.Generic;
using System.Linq;
using Game.Timeline;
using Reflex;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public enum ObjectGenAreaType
    {
        UL, UC, UR, CL, CC, CR, DL, DC, DR
    }

    public struct MonsterCreateCounter
    {
        public int maxGenCount;
        public int currentGenCount;
    }
    
    public class EnemyCharacterFactory : GameObjectFactory
        //, ITickable
    {
        private TableDataHolder tableDataHolder;
        private IEntityContainer entityContainer;
        private IGoldFeverTimeSystem goldFeverTimeSystem;
        private WeaponFactory weaponFactory;
        private TimelineManager timelineManager;

        public EnemyCharacterFactory(IEntityContainer _entityContainer, Container _container, WeaponFactory _weaponFactory, TableDataHolder _tableDataHolder, IGoldFeverTimeSystem _goldFeverTimeSystem, TimelineManager _timelineManager)
        {
            tableDataHolder = _tableDataHolder;
            container = _container;
            entityContainer = _entityContainer;
            weaponFactory = _weaponFactory;
            goldFeverTimeSystem = _goldFeverTimeSystem;
            timelineManager = _timelineManager;
        }
        
        public class MonsterCreateTimer
        {
            public float timer;
            public float executeTime;
            public EnemyDataEntity monsterDataEntity;
            public int minCreateCount;
            public int maxCreateCount;
            public int minEntireMonsterCount;
            public int maxEntireMonsterCount;
        }

        private readonly Dictionary<string, MonsterCreateCounter> monsterCreateCounter = new Dictionary<string, MonsterCreateCounter>();
        private readonly Dictionary<string, MonsterCreateTimer> monsterCreateTimers = new Dictionary<string, MonsterCreateTimer>();

        public void RefreshCreationData(List<MonsterCreateMarkerData> makedata)
        {
            monsterCreateTimers.Clear();
            foreach (var data in makedata)
            {
                tableDataHolder.EnemyCollection.TryGetEntity(data.monsterId, out var monsterDataEntity);
                monsterCreateTimers.Add(data.monsterId, new MonsterCreateTimer()
                {
                    monsterDataEntity = monsterDataEntity, executeTime = data.createCoolTime,
                    minCreateCount = data.minCreateCount, maxCreateCount = data.maxCreateCount,
                    minEntireMonsterCount = data.minEntireMonsterCount, maxEntireMonsterCount = data.maxEntireMonsterCount
                });
            }
        }

        protected override void OnCreateGameObject(Entity o, int pathHash)
        {
            var characterEntity = o.GetComponent<EnemyCharacterEntity>();
            characterEntity.weaponFactory = weaponFactory;
            characterEntity.SrcPathHashCode = pathHash;
            characterEntity.OnCreateEntity();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var area = GetObjectAreas(playerCharacterEntity.translateImplement.positionProperty.statusValue);
            var generateAreaTypes = GetAvailableMonsterGenArea();
            
            var boundsList = new List<Bounds2D>();
            foreach (var areaType in generateAreaTypes)
            {
                if (area.TryGetValue(areaType, out var bound))
                {
                    boundsList.Add(bound);
                }
            }
            
            var range = MonsterGenDistance(playerCharacterEntity.statusImplement.unitScaleProperty.statusValue);
            var monsterPosition = GetMonsterPosition( boundsList.Random(), playerCharacterEntity.translateImplement.positionProperty.statusValue, 6f, 11f);
            characterEntity.transformImplement.localPosition = monsterPosition;
        }

        public void Tick()
        {
            if (timelineManager.pause)
                return;

            var dTime = Time.deltaTime;
            foreach (var timer in monsterCreateTimers)
            {
                var creationTimer = timer.Value;
                creationTimer.timer += dTime;
                if (creationTimer.timer >= creationTimer.executeTime)
                {
                    var rnd = Random.Range(creationTimer.minCreateCount, creationTimer.maxCreateCount + 1);
                    
                    {
                        if (monsterCreateCounter.TryGetValue(creationTimer.monsterDataEntity.enemyId, out var data))
                        {
                            var monsterEntities = entityContainer.GetEntities<EnemyCharacterEntity>().Count(_x =>
                                _x.tableIndexDataImplement.tableDataIndexNoProperty.statusValue == int.Parse(creationTimer.monsterDataEntity.enemyId));
                            
                            data.currentGenCount = monsterEntities + rnd;
                            if (data.maxGenCount < data.currentGenCount)
                            {
                                data.maxGenCount = data.currentGenCount;
                            }
                            monsterCreateCounter[creationTimer.monsterDataEntity.enemyId] = data;
                        }
                        else
                        {
                            monsterCreateCounter.Add(creationTimer.monsterDataEntity.enemyId, new MonsterCreateCounter()
                            {
                                currentGenCount = rnd, maxGenCount = rnd
                            });
                        }
                    }

                    {
                        var additionalCount = 0;
                        if (monsterCreateCounter.TryGetValue(creationTimer.monsterDataEntity.enemyId, out var data))
                        {
                            if (data.maxGenCount > creationTimer.minEntireMonsterCount)
                            {
                                additionalCount = creationTimer.minEntireMonsterCount - data.currentGenCount;
                            }
                        }

                        if (additionalCount > 0)
                        {
                            if (additionalCount + data.currentGenCount > creationTimer.maxEntireMonsterCount)
                            {
                                additionalCount -= (creationTimer.maxEntireMonsterCount - data.currentGenCount);
                            }
                            rnd += additionalCount;
                        }
                    }
                    
                    for (int i = 0; i < rnd; ++i)
                    {
                        var newGameObject = CreateGameObject($"Prefabs/Char/{creationTimer.monsterDataEntity.prefabPath}");
                        if (newGameObject != null && newGameObject.TryGetComponent(out EnemyCharacterEntity characterEntity))
                        {
                            SetMonsterStatus(characterEntity, creationTimer.monsterDataEntity);
                        }
                    }
                    creationTimer.timer -= creationTimer.executeTime;
                }
            }
        }
        
        private Dictionary<ObjectGenAreaType, Bounds2D> GetObjectAreas(Vector2 position)
        {
            var ret = new Dictionary<ObjectGenAreaType, Bounds2D>();
            ret.Add(ObjectGenAreaType.CC, new Bounds2D(position, Vector2.one * 8f));
            ret.Add(ObjectGenAreaType.UL, new Bounds2D(new Vector2(position.x - 11f, position.y + 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.UC,new Bounds2D(new Vector2(position.x, position.y + 11f), new Vector2(8f, 14f)));
            ret.Add(ObjectGenAreaType.UR, new Bounds2D(new Vector2(position.x + 11f, position.y + 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.CL, new Bounds2D(new Vector2(position.x - 11f, position.y), new Vector2(14f, 8f)));
            ret.Add(ObjectGenAreaType.CR, new Bounds2D(new Vector2(position.x + 11f, position.y), new Vector2(14f, 8f)));
            ret.Add(ObjectGenAreaType.DL,  new Bounds2D(new Vector2(position.x - 11f, position.y - 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.DC, new Bounds2D(new Vector2(position.x, position.y - 11f), new Vector2(8f, 14f)));
            ret.Add(ObjectGenAreaType.DR, new Bounds2D(new Vector2(position.x + 11f, position.y - 11f), new Vector2(14f, 14f)));
            return ret;
        }

        private (float, float) MonsterGenDistance(UnitScaleType scaleType)
        {
            var rndValue = Random.value;
            
            return (0f, 0f);
        }

        private Vector2 GetMonsterPosition(Bounds2D bounds2Ds, Vector2 rootPosition, float minimalDistance, float maximalDistance)
        {
            var finding = true;
            var vec = Vector2.zero;
            while (finding)
            {
                var randomPosMaxDist = Random.insideUnitCircle * maximalDistance;
                if (Mathf.Pow(randomPosMaxDist.x, 2) + Mathf.Pow(randomPosMaxDist.y, 2) < Mathf.Pow(minimalDistance, 2))
                {
                    continue;
                }

                randomPosMaxDist += rootPosition;
                if (bounds2Ds.Contains(randomPosMaxDist))
                {
                    vec = randomPosMaxDist;
                    finding = false;
                }
            }
            return vec;
        }
        
        private List<ObjectGenAreaType> GetAvailableMonsterGenArea()
        {
            var generateAreaTypes = new List<ObjectGenAreaType>();
            var rndValue = Random.value;
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            switch (playerCharacterEntity.statusImplement.unitDirectionProperty.statusValue)
            {
                case UnitViewDirection.CR:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                    }

                    break;
                case UnitViewDirection.UR:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }

                    break;
                case UnitViewDirection.UC:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                    }

                    break;
                case UnitViewDirection.UL:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                    }

                    break;
                case UnitViewDirection.CL:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                    }

                    break;
                case UnitViewDirection.DL:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                    }

                    break;
                case UnitViewDirection.DC:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                    }

                    break;
                case UnitViewDirection.DR:
                    if (rndValue < 0.4f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.DC);
                        generateAreaTypes.Add(ObjectGenAreaType.DR);
                        generateAreaTypes.Add(ObjectGenAreaType.CR);
                    }
                    else if (rndValue > 0.75f)
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UC);
                        generateAreaTypes.Add(ObjectGenAreaType.UL);
                        generateAreaTypes.Add(ObjectGenAreaType.CL);
                    }
                    else
                    {
                        generateAreaTypes.Add(ObjectGenAreaType.UR);
                        generateAreaTypes.Add(ObjectGenAreaType.DL);
                    }

                    break;
            }

            return generateAreaTypes;
        }

        public void CreateWavesAtPlayerPosition(List<WaveCreateMarkerData> waveCreateMarkerData)
        {
            Vector2 playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            foreach (WaveCreateMarkerData data in waveCreateMarkerData)
                CreateWave(data, playerPosition);
        }

        public void CreateWave(WaveCreateMarkerData data, Vector2 startingPosition)
        {
            var positionList = data.type switch
            {
                WaveCreateMarkerData.Type.Circle => MonsterSpawnRule.GetCircleSpawnPoints(data.num, data.radius, data.angleOffset, data.colliderRadius),
                WaveCreateMarkerData.Type.Linear => MonsterSpawnRule.GetLinearSpawnPoints(data.num, data.distance, data.length, data.angle, data.colliderRadius),
                WaveCreateMarkerData.Type.Area => MonsterSpawnRule.GetAreaSpawnPoints(data.num, data.rect, data.colliderRadius),
                _ => new List<Vector2>(),
            };

            tableDataHolder.EnemyCollection.TryGetEntity(data.monsterId, out var enmeyDataEntity);
            foreach (var position in positionList)
                CreateEnemy(enmeyDataEntity, position + startingPosition);
        }

        public EnemyCharacterEntity CreateEnemy(string enemyId, Vector2 position)
        {
            tableDataHolder.EnemyCollection.TryGetEntity(enemyId, out var enmeyDataEntity);
            return CreateEnemy(enmeyDataEntity, position);
        }

        private EnemyCharacterEntity CreateEnemy(EnemyDataEntity enmeyDataEntity, Vector2 position)
        {
            var newGameObject = CreateGameObject($"Prefabs/Char/{enmeyDataEntity.prefabPath}");

            if (newGameObject.TryGetComponent(out EnemyCharacterEntity characterEntity))
            {
                characterEntity.transformImplement.localPosition = position;
                SetMonsterStatus(characterEntity, enmeyDataEntity);

                return characterEntity;
            }
            return null;
        }

        private void SetMonsterStatus(EnemyCharacterEntity enemyCharacterEntity, EnemyDataEntity enemyDataEntity)
        {
            enemyCharacterEntity.statusImplement.hitPointProperty.statusValue = enemyDataEntity.hitPoint;
            enemyCharacterEntity.monsterTypeImplement.monsterTypeDataProperty.statusValue = enemyDataEntity.monsterType;
            enemyCharacterEntity.monsterMaxHitPointImplement.maxHitPointProperty.statusValue = enemyDataEntity.hitPoint;
            enemyCharacterEntity.monsterDamageBaseValueImplement.DamageBaseValueProperty.statusValue = enemyDataEntity.damage;
            enemyCharacterEntity.translateImplement.velocityProperty.statusValue = enemyDataEntity.moveSpeed;
            enemyCharacterEntity.tableIndexDataImplement.tableDataIndexNoProperty.statusValue = int.Parse(enemyDataEntity.enemyId);
            SetOnKilledEventImplement(enemyCharacterEntity.onKilledEventImplement, enemyDataEntity);
            enemyCharacterEntity.hitTintImplement.hitTintTriggerProperty.statusValue = false;
            enemyCharacterEntity.spriterendererImplement.color = goldFeverTimeSystem.IsGoldFeverTime ? MaterialProperties.GoldColor : Color.white;
            enemyCharacterEntity.grayscaleImplement.enableGrayScale = goldFeverTimeSystem.IsGoldFeverTime ? true : false;

            var EGID = enemyCharacterEntity.EGID;
            var applyBuffImplement = enemyCharacterEntity.applyBuffImplement;
            applyBuffImplement.applyBuffList.Clear();
            applyBuffImplement.applyBuffList.Add(new Buff()
            {
                statusValue = new BuffData()
                {
                    buffDeviceId = "",
                    rootEntityId = EGID,
                    rootCharacter = new CharacterType() { statusValue = CharacterTypes.Enemy },
                    buffType = BuffType.Damage,
                    remainTime = -1,
                    buffValue = enemyDataEntity.damage
                }
            });

            if (enemyCharacterEntity is BossEnemyCharacterEntity)
            {
                MessageBroker.Default.Publish(new KeyValuePair<string, (uint, float)>("bossCharacterCreate", (enemyCharacterEntity.EGID, 150)));
            }

            if(enemyCharacterEntity.weaponInventoryImplement != null && tableDataHolder.EnemyWeaponCollection.TryGetEntity(enemyDataEntity.weaponId, out var weaponDataEntity))
            {
                enemyCharacterEntity.weaponInventoryImplement.ClearWeaponList();
                enemyCharacterEntity.weaponInventoryImplement.AddOrUpdateWeapon(ToWeaponData(weaponDataEntity));
            }
        }

        private void SetOnKilledEventImplement(OnKilledEventImplement onKilledEventImplement, EnemyDataEntity enemyDataEntity)
        {
            onKilledEventImplement.ExpToDropProperty.statusValue = enemyDataEntity.exp;

            foreach(var dropId in enemyDataEntity.itemDropIds)
            {
                if(tableDataHolder.ItemDropCollection.TryGetEntity(dropId, out var ItemDropDataEntity))
                {
                    var index = ExtensionFunction.PerTenThousandRandomIndex(ItemDropDataEntity.itemProps);
                    if (index == -1)
                        continue;

                    onKilledEventImplement.itemIdsToDrop.Add(new ItemIdToDrop() { statusValue = ItemDropDataEntity.itemIds[index] });
                }
            }
        }

        private static WeaponData ToWeaponData(EnemyWeaponDataEntity dataEntity)
        {
            return new WeaponData() { statusValue = new WeaponDataSet()
            {
                id = dataEntity.weaponId,
                prefabPath = $"Prefabs/Projectile/{dataEntity.fileName}",
                level = 1,
                damage = dataEntity.damage,
                coolTime = dataEntity.coolDown,
                isSingleCreation = dataEntity.singleCreation,
                isSummonCreation = dataEntity.summonCreation,
                createCount = dataEntity.projectile,
                multiCreationTick = dataEntity.projectileInterval,
                projectileSpeed = dataEntity.projectileSpeed,
                criticalRatio = 0,
                criticalProbability = 0,
                spearCount = dataEntity.pierce,

                knockBack = dataEntity.knockBack,
                attackType = AttackType.PHYSICS,

                area = dataEntity.area,
            } };
        }
    }
}
