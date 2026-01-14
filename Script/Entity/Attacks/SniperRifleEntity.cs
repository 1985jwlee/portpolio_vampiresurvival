using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class SniperRifleEntity : AttackEntity
    {
        public SeekingTargetImplement seekingTargetImplement;
        public TableIndexDataImplement tableIndexDataImplement;
        
        [Inject] protected WeaponFactory weaponFactory;
        [Inject] protected TableDataHolder tableDataHolder;
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        
        private WeaponDataSet nextWeaponDataSet;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
            }

            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
            
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }
        
        
        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            CreateWeapon(nextWeaponDataSet, translateImplement.positionProperty.statusValue, 1, 1);
            seekingTargetSystem.UnRegistComponent(this);
            
        }
        
        protected void SetNextWeapon()
        {
            var tableValue = tableIndexDataImplement.tableDataIndexNoProperty.statusValue;
            if (tableDataHolder.ActiveDeviceCollection.TryGetEntity( tableValue.ToString(), out var group) == false) {return;}

            if (group.GroupEntity.Count <= 0) {return;}
            var level = weaponDataSetImplement.weaponDataSetProperty.statusValue.level;
            if (group.GroupEntity.TryGetValue(level.ToString(), out var deviceEntity))
            {
                nextWeaponDataSet = DeviceInventory.ToWeaponDataSet(deviceEntity);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            attackLifetimeImplement.lifeTime = weaponDataSetImplement.weaponDataSetProperty.statusValue.lifeDuration /
                                               entityContainer.playerCharacterEntity.characterStatusImplement
                                                   .buffedprojectileSpeedRatioProperty.statusValue;

            
        }

        public override void OnApplyShootCountChanged()
        {
            SetNextWeapon();
            
            var enemies = entityContainer.GetEntities<EnemyCharacterEntity>();
            var getEntities = entityContainer.GetEntities<SniperRifleEntity>();
            getEntities.Remove(this);
            enemies.RemoveAll(_x => _x.characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living);
            for (int i = 0, len = getEntities.Count; i < len; ++i)
            {
                enemies.RemoveAll(_x => _x.EGID == getEntities[i].seekingTargetImplement.seekingTargetProperty.seekTargetEGID);
            }

            if (enemies.Count > 0 == false)
            {
                return;
            }

            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = enemies.OrderByDescending(_x => _x.statusImplement.hitPointProperty.statusValue).First().EGID;
            seekingTargetImplement.seekingTargetProperty.seekingTime = 0f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            
            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    if (entityContainer.GetEntity<CharacterEntity>(seekingTargetImplement.seekingTargetProperty.seekTargetEGID, out var entity))
                    {
                        transformImplement.position = entity.translateImplement.positionProperty.statusValue;
                    }
                });
        }
        
        public void CreateWeapon(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount)
        {
            var go = weaponFactory.CreateGameObject(d.prefabPath);
            if (go == null || go.TryGetComponent(out AttackEntity attackEntity) == false)
            {
                return;
            }

            attackEntity.transformImplement.position = new Vector3(targetPosition.x, targetPosition.y, 0f);

            if (attackEntity.spearAttackImplement != null)
            {
                attackEntity.spearAttackImplement.attackSpearCountProperty.statusValue = d.spearCount;
            }

            attackEntity.weaponDataSetImplement.weaponDataSetProperty.statusValue = d;

            attackEntity.translateImplement.velocityProperty.statusValue = d.projectileSpeed;
            attackEntity.attackTypeImplement.attackTypeProperty.statusValue = d.attackType;

            if (d.isSingleCreation)
            {
                attackEntity.refreshSingleAttackImplement = attackEntity.GetOrAddComponent<RefreshSingleInstanceAttackImplement>();
                attackEntity.refreshSingleAttackImplement.attackableId = d.id;
                attackEntity.refreshSingleAttackImplement.attackableEGID = attackEntity.EGID;
            }
            
            if (attackEntity.TryGetComponent(out ExchangeAttackType exchangeAttackType))
            {
                if (exchangeAttackType.initAttackTypeProperty.statusValue == d.attackType)
                {
                    d.attackType = exchangeAttackType.exchangeAttackTypeProperty.statusValue;
                }
            }

            attackEntity.attackLifetimeImplement.lifeTime = d.lifeDuration;
            attackEntity.shootCounterImplement.shootCountProperty.statusValue = cnt;
            attackEntity.shootCounterImplement.maxShootCountProperty.statusValue = maxCount;
            attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue = new KnockBackSendDataSet()
            {
                direction = attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue.direction,
                distance = GameSettings.DefaultKnockBackDistance * d.knockBack
            };
            attackEntity.applyBuffImplement.applyBuffList.Clear();
            attackEntity.applyBuffImplement.applyBuffList.Add(new Buff
            {
                statusValue = new BuffData
                {
                    buffType = d.attackType switch
                    {
                        AttackType.INSTANT_DEATH => BuffType.DeathPenalty,
                        AttackType.PHYSICS => BuffType.Damage,
                        _ => BuffType.MagicDamage
                    },
                    remainTime = -1,
                    rootEntityId = EGID,
                    rootCharacter = new CharacterType()
                    {
                        statusValue = CharacterTypes.PlayerCharacter
                    },
                    buffValue = d.damage
                }
            });

            if (d.area > 0f)
            {
                attackEntity.applyBuffImplement.applyBuffList.Add(new Buff
                {
                    statusValue = new BuffData
                    {
                        buffType = BuffType.AttackArea,
                        remainTime = -1,
                        rootEntityId = EGID,
                        rootCharacter = new CharacterType()
                        {
                            statusValue = CharacterTypes.PlayerCharacter
                        },
                        buffValue = d.area
                    }
                });
            }

            attackEntity.OnApplyShootCountChanged();
        }
    }
}
