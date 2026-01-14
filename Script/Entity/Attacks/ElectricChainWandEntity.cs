using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class ElectricChainWandEntity : LightningWandEntity
    {
        [Inject] protected WeaponFactory weaponFactory;
        [Inject] protected TableDataHolder tableDataHolder;

        public TableIndexDataImplement tableIndexDataImplement;
        private WeaponDataSet nextWeaponDataSet;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
        }

        public override void ApplyComponentData()
        {
            SetNextWeapon();
            base.ApplyComponentData();
        }

        protected override EnemyCharacterEntity SetAttackPoint()
        {
            var ememy = base.SetAttackPoint();
            var enemyPosition = ememy.translateImplement.positionProperty.statusValue;
            
            var list = entityContainer.GetEntities<EnemyCharacterEntity>().Where(_x => _x.EGID != ememy.EGID && Vector3.Distance(enemyPosition, _x.translateImplement.positionProperty.statusValue) < 2f).ToList();
            if (list.Count > 0)
            {
                var random = list.Random();
                var randomPos = random.translateImplement.positionProperty.statusValue;
                CreateWeapon(nextWeaponDataSet, randomPos, 1, 1).Forget();
                return random;
            }
            return ememy;
        }
        
        private void SetNextWeapon()
        {
            if (tableDataHolder.AdvActiveDeviceCollection.TryGetEntity( tableIndexDataImplement.tableDataIndexNoProperty.statusValue.ToString(), out var group) == false) {return;}

            if (group.GroupEntity.Count > 0)
            {
                nextWeaponDataSet = DeviceInventory.ToWeaponDataSet(group.GroupEntity.First().Value);
            }
        }
        
        
        
        public async UniTask  CreateWeapon(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount)
        {
            var cancellationtoken = new CancellationTokenSource();
            var copy = CancellationTokenSource.CreateLinkedTokenSource(cancellationtoken.Token);
            await UniTask.Delay((int)(200), DelayType.DeltaTime, PlayerLoopTiming.Update, copy.Token);
            
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
            cancellationtoken.Cancel();
        }
    }
}
