using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public struct AttackDroneIndicatorData
    {
        public uint indicatorEGID;
        public WeaponDataSet weaponDataSet;
        public Color appliedColor;
    }
    
    public class AttackDroneEntity : ArcaneDeviceSummonEntity
    {
        [Inject] protected WeaponFactory weaponFactory;
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        

        [Inject] protected IndicatorFactory indicatorFactory;
        
        protected List<AttackDroneIndicatorData> indicatorWeaponTarget = new List<AttackDroneIndicatorData>();
        
        public SeekingPositionImplement seekingTargetImplement;
        public Rigidbody2D rigidBodyImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            OwnerEGID = entityContainer.playerCharacterEntity.EGID;

            TryGetComponent(out rigidBodyImplement);

            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);    
            }
            
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
            indicatorWeaponTarget.Clear();
        }
        
        
        public override void ApplyComponentData()
        {
            var cnt = entityContainer.GetEntities<AttackDroneEntity>().Count;
            transformImplement.position = GetSeekingPosition(cnt);
            seekingTargetImplement.seekingTargetProperty.seekTargetPosition = GetSeekingPosition(cnt);
           
            seekingTargetImplement.seekingTargetProperty.seekingTime = 0.5f + (cnt - 1) * 0.3f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            transformImplement.rotation = Quaternion.identity;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    seekingTargetImplement.seekingTargetProperty.seekTargetPosition = GetSeekingPosition(cnt);
                });
        }

        public override void OnApplyEntityComponent()
        {
            CreateIndicator();
        }


        protected virtual void CreateIndicator()
        {
            
        }

        protected Vector3 GetSeekingPosition(int count)
        {
            var src = entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue;
            var characterViewDir = entityContainer.playerCharacterEntity.statusImplement.unitDirectionProperty.statusValue;
            switch (characterViewDir)
            {
                case UnitViewDirection.CR:
                case UnitViewDirection.DR:
                case UnitViewDirection.UR:
                    src += new Vector3(-0.5f * count, 1f, 0f);
                    break;
                case UnitViewDirection.CL:
                case UnitViewDirection.DL:
                case UnitViewDirection.UL:
                default:
                    src += new Vector3(0.5f * count, 1f, 0f);
                    break;
            }

            return src;
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            foreach (var i in indicatorWeaponTarget)
            {
                if (entityContainer.GetEntity<Entity>(i.indicatorEGID, out var entity))
                {
                    indicatorFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                }
            }
            seekingTargetSystem.UnRegistComponent(this);
        }
        
        protected override void Update()
        {
            base.Update();
            UpdateSpeedByDistanceSeekingPosition();
            CooldownWeapon();
            CreateWeaponList();
        }

        private void CreateWeaponList()
        {
            foreach (var d in weaponSets)
            {
                if (string.IsNullOrEmpty(d.prefabPath))
                {
                    continue;
                }
                
                var createCount = d.createCount;
                if (d.isSingleCreation == false)
                {
                    createCount += entityContainer.playerCharacterEntity.characterStatusImplement.buffedaddProjCountProperty.statusValue;
                }

                for (int i = 0; i < indicatorWeaponTarget.Count; ++i)
                {
                    if (entityContainer.GetEntity(indicatorWeaponTarget[i].indicatorEGID, out DroneAttackIndicatorEntity indicatorEntity))
                    {
                        var data = indicatorWeaponTarget[i];
                        data.weaponDataSet = d;
                        indicatorWeaponTarget[i] = data;
                        indicatorEntity.PopRandomAttackPosition(createCount);
                    }
                }
            }
            
            weaponSets.Clear();
        }


        public void CooldownWeapon()
        {
            weaponInventoyImplement.CheckWeaponCooldown(entityContainer.playerCharacterEntity.characterStatusImplement.buffedcooldownRatioProperty.statusValue);
            weaponInventoyImplement.FireWeapon(out var output);
            weaponSets.AddRange(output);
        }

        public async UniTask CreateWeapon(int count, List<Vector2> targetPosition, WeaponDataSet weaponDataSet)
        {
            var cnt = 0;
            var cancellationtoken = new CancellationTokenSource();
            while (cnt < count)
            {
                CreateWeapon(weaponDataSet, targetPosition[cnt], ++cnt, count);
                
                var copy = CancellationTokenSource.CreateLinkedTokenSource(cancellationtoken.Token);
                if(weaponDataSet.multiCreationTick > 0)
                    await UniTask.Delay((int)(weaponDataSet.multiCreationTick * 1000f), DelayType.DeltaTime, PlayerLoopTiming.Update, copy.Token);
            }
            cancellationtoken.Cancel();
        }

        public void CreateWeapon(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount)
        {
            var go = weaponFactory.CreateGameObject(d.prefabPath);
            if (go == null)
            {
                return;
            }

            go.TryGetComponent<AttackEntity>(out var attackEntity);

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

        private void UpdateSpeedByDistanceSeekingPosition()
        {
            var currentPosition = translateImplement.positionProperty.statusValue;
            var targetPosition = seekingTargetImplement.seekingTargetProperty.seekTargetPosition;
            var distance = Vector3.Distance(targetPosition, currentPosition);
            var direction = ((Vector2)(targetPosition - currentPosition)).normalized;
            rigidBodyImplement.velocity = direction * distance;
        }

        protected void OnPopAttackablePositions(List<Vector2> vec, uint indicatorEGID)
        {
            var createCount = vec.Count;
            for (int i = 0; i < indicatorWeaponTarget.Count; ++i)
            {
                if (indicatorWeaponTarget[i].indicatorEGID == indicatorEGID)
                {
                    var weaponDataSet = indicatorWeaponTarget[i].weaponDataSet;
                    if (createCount > 1)
                    {
                        CreateWeapon(createCount, vec, weaponDataSet).Forget();
                    }
                    else
                    {
                        CreateWeapon(weaponDataSet, vec[0], 1, 1);
                    }

                    break;
                }
            }
        }

    }
}
