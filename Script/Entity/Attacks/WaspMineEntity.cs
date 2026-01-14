using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class WaspMineEntity : AttackEntity
    {
        [Inject] protected Camera mainCamera;
        public TableIndexDataImplement tableIndexDataImplement;
        public DelayAttackSecondImplement delayAttackSecondImplement;
        public GameObject settingObject;
        public GameObject activateObject;
        public CircleCollider2D attackCollider;
        
        [Inject] protected WeaponFactory weaponFactory;
        [Inject] protected TableDataHolder tableDataHolder;
        
        private WeaponDataSet nextWeaponDataSet;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            activateObject.TryGetComponent(out attackCollider);

            if (TryGetComponent(out delayAttackSecondImplement))
            {
                Components.Add(delayAttackSecondImplement);
            }
            
            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();

            attackLifetimeImplement.lifeTime = -1f;
            
            settingObject.SetActive(true);
            activateObject.SetActive(false);
            
            delayAttackSecondImplement.enableDelay = false;
            

            delayAttackSecondImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    attackLifetimeImplement.lifeTime = 0.1f;
                });

            transformImplement.position = ExtensionFunction.RandomCirclePosition(entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue, 8f);
            SyncTransform(transformImplement, translateImplement);
        }

        public override void OnApplyShootCountChanged()
        {
            SetNextWeapon();
            CheckAttackRange();
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            settingObject.SetActive(true);
            activateObject.SetActive(false);
            delayAttackSecondImplement.enableDelay = false;
            attackLifetimeImplement.lifeTime = -1f;
            
            CreateWeapon(nextWeaponDataSet, translateImplement.positionProperty.statusValue, 1, 1);
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

        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
            if (settingObject.activeInHierarchy)
            {
                foreach (var enemy in entityContainer.GetEntities<EnemyCharacterEntity>())
                {
                    if (Vector2.Distance(enemy.translateImplement.positionProperty.statusValue, translateImplement.positionProperty.statusValue) < transformImplement.localScale.x * attackCollider.radius)
                    {
                        settingObject.SetActive(false);
                        activateObject.SetActive(true);
                        delayAttackSecondImplement.enableDelay = true;
                        break;
                    }
                }
            }
        }

        protected override void CheckAttackRange()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var sizeMultiply = playerCharacterEntity.characterStatusImplement.buffedattackSizeRatioProperty.statusValue;
            foreach (var buff in applyBuffImplement.applyBuffList)
            {
                if (buff.statusValue.buffType == BuffType.AttackArea)
                {
                    sizeMultiply *= buff.statusValue.buffValue;
                }
            }
            transformImplement.localScale = sizeMultiply * Vector3.one;
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
                    buffType = d.attackType == AttackType.PHYSICS ? BuffType.Damage : BuffType.MagicDamage,
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
