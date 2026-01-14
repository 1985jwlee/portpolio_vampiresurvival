using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ICandidateRemoveEntitySystem : ITickGameSystem
    {

    }

    public class CandidateRemoveEntitySystem : ICandidateRemoveEntitySystem
    {
        public int ExcuteOrder => 2000;

        private readonly TickGameSystemManager tickGameSystemManager;

        private readonly IEntityContainer entityContainer;

        private readonly EnemyCharacterFactory enemyCharacterFactory;
        private readonly TokenFactory tokenFactory;
        private readonly WeaponFactory weaponFactory;
        private readonly DummyFactory dummyFactory;
        private readonly TableDataHolder tableDataHolder;

        private readonly IGoldFeverTimeSystem goldFeverTimeSystem;
        private readonly IMonsterKillAchivementSystem monsterKillAchivementSystem;

        private readonly HashSet<uint> egids = new HashSet<uint>();
        private readonly List<EnemyCharacterEntity> candidateRemoveEnemyEntity = new List<EnemyCharacterEntity>();
        private readonly List<TokenEntity> candidateRemoveTokenEntity = new List<TokenEntity>();

        public CandidateRemoveEntitySystem(TickGameSystemManager _tickGameSystemManager, IEntityContainer _entityContainer, IMonsterKillAchivementSystem monsterKillAchivementSystem,
            EnemyCharacterFactory _factory, TokenFactory _tokenFactory, WeaponFactory _weaponFactory, IGoldFeverTimeSystem _goldFeverTimeSystem, DummyFactory _dummyFactory,
            TableDataHolder _tableDataHolder)
        {
            tickGameSystemManager = _tickGameSystemManager;
            entityContainer = _entityContainer;
            enemyCharacterFactory = _factory;
            tokenFactory = _tokenFactory;
            weaponFactory = _weaponFactory;
            goldFeverTimeSystem = _goldFeverTimeSystem;
            dummyFactory = _dummyFactory;
            tableDataHolder = _tableDataHolder;
            this.monsterKillAchivementSystem = monsterKillAchivementSystem;
        }

        public void InitGameSystem()
        {
            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            egids.Add(sourceEntity.EGID);
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            egids.Remove(sourceEntity.EGID);
        }

        public void OnUpdate()
        {
            candidateRemoveEnemyEntity.Clear();
            candidateRemoveTokenEntity.Clear();
            
            foreach (var egid in egids)
            {
                {
                    if (entityContainer.GetEntity<EnemyCharacterEntity>(egid, out var entity))
                    {
                        if (entity.characterDeathImplement.DeathStateProperty.statusValue == DeathState.Dead)
                        {
                            candidateRemoveEnemyEntity.Add(entity);
                        }
                    }
                }
                {
                    if (entityContainer.GetEntity<TokenEntity>(egid, out var entity))
                    {
                        if (entity.tokenStateImplement.tokenStateProperty.statusValue == TokenStateType.Applied)
                        {
                            candidateRemoveTokenEntity.Add(entity);
                        }
                    }
                }
            }
            
            foreach (var e in candidateRemoveTokenEntity)
            {
                tokenFactory.EnqueRecycle(e, e.SrcPathHashCode);
            }

            foreach (var e in candidateRemoveEnemyEntity)
            {
                var position = e.translateImplement.positionProperty.statusValue;
                e.translateImplement.moveDirectionProperty.statusValue = Vector2.zero;
                e.rigidBodyImplement.velocity = Vector2.zero;

                if (e.onKilledEventImplement.ExpToDropProperty.statusValue != 0)
                    DropExp(position, e.onKilledEventImplement);

                DropTokens(position, e.onKilledEventImplement);
                
                if (goldFeverTimeSystem.IsGoldFeverTime)
                {
                    var tokenGo = tokenFactory.CreateGameObject(GameSettings.GoldFeverRewardTokenPrefabPath);
                    var tokenEntity = tokenGo.GetComponent<TokenEntity>();
                    tokenEntity.transformImplement.position = position + (Vector3)Random.insideUnitCircle;
                }

                if (e.onKilledEventImplement.MonsterSpawnNumProperty.statusValue > 0 && e.onKilledEventImplement.MonsterSpawnIdProperty.statusValue != 0)
                {
                    var monsterId = e.onKilledEventImplement.MonsterSpawnIdProperty.statusValue;
                    var num = e.onKilledEventImplement.MonsterSpawnNumProperty.statusValue;
                    var radius = e.onKilledEventImplement.MonsterSpawnRadiusProperty.statusValue;

                    var wave = Timeline.WaveCreateMarkerData.Circle(monsterId.ToString(), num, radius);
                    enemyCharacterFactory.CreateWave(wave, position);
                }

                if(e.onKilledEventImplement.ShouldExplodeProperty.statusValue)
                {
                    var damage = Mathf.FloorToInt(e.monsterDamageBaseValueImplement.DamageBaseValueProperty.statusValue);
                    var scale = (e.transformImplement.localScale.x + e.transformImplement.localScale.y) / 2;
                    EnemyWeaponCreator.CreateSuicideBombing(weaponFactory, e.EGID, e.transformImplement.position, damage, scale);
                }

                if(string.IsNullOrEmpty(e.onKilledEventImplement.WeaponToSpawnProperty.statusValue.prefabPath) == false)
                {
                }

                foreach(var dummyEgid in e.onKilledEventImplement.dummyEgidToRemoves)
                {
                    if(entityContainer.GetEntity<DummyEntity>(dummyEgid.statusValue, out var entity))
                    {
                        dummyFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                    }
                }

                enemyCharacterFactory.EnqueRecycle(e, e.SrcPathHashCode);
            }

            if (candidateRemoveEnemyEntity.Count > 0)
            {
                monsterKillAchivementSystem.reactiveProperty.Value += candidateRemoveEnemyEntity.Count;    
            }
        }

        private void DropTokens(Vector3 position, OnKilledEventImplement onKilledEventImpl)
        {
            foreach (var id in onKilledEventImpl.itemIdsToDrop)
            {
                tableDataHolder.ItemCollection.TryGetEntity(id.statusValue.ToString(), out var itemDataEntity);

                var tokenGo = tokenFactory.CreateGameObject(itemDataEntity.prefabPath);
                var tokenEntity = tokenGo.GetComponent<TokenEntity>();
                tokenEntity.transformImplement.position = position + (Vector3)Random.insideUnitCircle;
            }
        }

        private void DropExp(Vector3 position, OnKilledEventImplement onKilledEventData)
        {
            var prefabPath = GameSettings.GetExpTokenPrefabPath(onKilledEventData.ExpToDropProperty.statusValue);

            var o = tokenFactory.CreateGameObject(prefabPath);
            var entity = o.GetComponent<TokenEntity>();
            entity.transformImplement.position = position;

            entity.applyBuffImplement.applyBuffList.Clear();
            entity.applyBuffImplement.applyBuffList.Add(new Buff
            {
                statusValue = new BuffData
                {
                    buffType = BuffType.ExpUp,
                    buffValue = onKilledEventData.ExpToDropProperty.statusValue,
                    buffDeviceId = string.Empty,
                    remainTime = -1
                }
            });
        }
    }
}
