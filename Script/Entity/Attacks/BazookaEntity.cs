using System.Linq;
using UnityEngine;
using Reflex.Scripts.Attributes;
using UniRx;

namespace Game.ECS
{
    public class BazookaEntity : AttackEntity
    {
        [Inject] protected WeaponFactory weaponFactory;
        [Inject] private TableDataHolder tableDataHolder;
        [Inject] private ISpearAttackSystem spearAttackSystem;

        public TableIndexDataImplement tableIndexDataImplement;
        public WallBounceImplement wallBounceImplement;
        private WeaponDataSet nextWeaponDataSet;


        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            
            TryGetComponent(out rigidBodyImplement);
            
            
            if (TryGetComponent(out wallBounceImplement))
            {
                Components.Add(wallBounceImplement);
            }

            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
            
        }
        
        

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            SetNextWeapon();
            var playerTranslate = entityContainer.playerCharacterEntity.translateImplement;
            transformImplement.position = playerTranslate.positionProperty.statusValue;
            var moveDir = playerTranslate.moveDirectionProperty.statusValue;
            if (moveDir.sqrMagnitude < 0.1f)
            {
                moveDir = Vector2.right;
            }
            translateImplement.moveDirectionProperty.statusValue = moveDir;

            wallBounceImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    spearAttackImplement.attackSpearCountProperty.statusValue -= 1;
                    spearAttackSystem.receiveEntityEGID.Execute(EGID);
                });
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            
            CreateWeapon(nextWeaponDataSet, translateImplement.positionProperty.statusValue, 1, 1);
        }

        protected override void Update()
        {
            base.Update();
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
            SyncRotateByMoveDirection(translateImplement, transformImplement);
        }

        private void SetNextWeapon()
        {
            if (tableDataHolder.ActiveDeviceCollection.TryGetEntity( tableIndexDataImplement.tableDataIndexNoProperty.statusValue.ToString(), out var group) == false) {return;}

            if (group.GroupEntity.Count > 0)
            {
                nextWeaponDataSet = DeviceInventory.ToWeaponDataSet(group.GroupEntity.First().Value);
            }
        }

        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity =  direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }
        
        private static void SyncRotateByMoveDirection(TranslateImplement translateImpl, Transform transformImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var angle = Vector2.Angle(direction, Vector2.right);
            if (direction.y < 0f)
            {
                angle *= -1f;
            }
            transformImpl.rotation = Quaternion.Euler(0f, 0, angle);
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
