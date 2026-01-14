using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class SpecialEnemyFunctionalities
    {
        public static void StartTimer(GeneralTimerManageImplement generalTimerManageImpl, int i)
        {
            var status = generalTimerManageImpl.timerStatuses[i].statusValue;
            status.timer = 0f;
            status.timerState = GeneralTimerState.Counting;
            generalTimerManageImpl.timerStatuses[i] = new GeneralTimerStatus() { statusValue = status };
        }

        public static void StartTimer(GeneralTimerManageImplement generalTimerManageImpl, int i, float duration)
        {
            var setting = generalTimerManageImpl.timerSettings[i].statusValue;
            setting.duration = duration;
            generalTimerManageImpl.timerSettings[i] = new GeneralTimerSetting() { statusValue = setting };

            StartTimer(generalTimerManageImpl, i);
        }

        public static void UpdateTimer(GeneralTimerManageImplement generalTimerManageImpl)
        {
            for (int i = 0; i < generalTimerManageImpl.timerStatuses.Count; i++)
            {
                var setting = generalTimerManageImpl.timerSettings[i].statusValue;
                var status = generalTimerManageImpl.timerStatuses[i].statusValue;

                switch (status.timerState)
                {
                    case GeneralTimerState.Waiting:
                        break;
                    case GeneralTimerState.Counting:
                        {
                            status.timer += Time.deltaTime;
                            if (status.timer > setting.duration)
                                status.timerState = GeneralTimerState.Finish;
                        }
                        break;
                    case GeneralTimerState.Finish:
                        status.timerState = GeneralTimerState.Waiting;
                        break;
                }

                generalTimerManageImpl.timerStatuses[i] = new GeneralTimerStatus() { statusValue = status };
            }
        }

        public static void StartTimer(AttackTimerManageImplement attackTimerManageImpl, int i)
        {
            var timerStatus = attackTimerManageImpl.attackTimerStatuses[i].statusValue;
            timerStatus.attackTimerState = AttackTimerState.Start;
            attackTimerManageImpl.attackTimerStatuses[i] = new AttackTimerStatus() { statusValue = timerStatus };
        }

        public static void UpdateAttackTimer(CharacterDeathImplement characterDeathImplement, AttackTimerManageImplement attackTimerManageImpl, AnimationImplement animationImplement)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;
            /// <see cref="CharacterEntity.UpdateAnimator"/> 와 <see cref="EnemyCharacterEntity.UpdateAnimation"/> 의 실행 순서를 이용해서 애니메이션 제어 중.
            /// 실행 순서:
            /// UpdateAnimator(현재 설정된 Hash 값을 이용해 Animator 의 Trigger 설정)
            /// ↓
            /// UpdateAnimation(Idle, Walk, Die 애니메이션 Hash 설정)
            /// ↓
            /// 현재 코드(보스만의 특수한 애니메이션 Hash 설정)
            /// 순서가 깨지면 동작이 잘못될 수 있어서, 개선 또는 순서 유지 필요.
            for (int i = 0; i < attackTimerManageImpl.attackTimerSettings.Count; i++)
            {
                var setting = attackTimerManageImpl.attackTimerSettings[i].statusValue;
                var status = attackTimerManageImpl.attackTimerStatuses[i].statusValue;
                switch (status.attackTimerState)
                {
                    case AttackTimerState.Waiting:
                        break;
                    case AttackTimerState.Start:
                        {
                            status.foreswingTimer = 0;
                            status.attackTimerState = setting.foreswingDuration > 0? AttackTimerState.Foreswing : AttackTimerState.StartImpact;
                        }
                        break;
                    case AttackTimerState.Foreswing:
                        {
                            animationImplement.animationStateProperty.statusValue = SlimeKingAnimationHashs.AllHashs[i].Before;
                            status.foreswingTimer += Time.deltaTime;
                            if (status.foreswingTimer > setting.foreswingDuration)
                                status.attackTimerState = AttackTimerState.StartImpact;
                        }
                        break;
                    case AttackTimerState.StartImpact:
                        {
                            animationImplement.animationStateProperty.statusValue = SlimeKingAnimationHashs.AllHashs[i].Attack;
                            status.impactTimer = 0;
                            status.attackTimerState = setting.impactDuration > 0? AttackTimerState.Impacting : AttackTimerState.FinishImpact;
                        }
                        break;
                    case AttackTimerState.Impacting:
                        {
                            animationImplement.animationStateProperty.statusValue = SlimeKingAnimationHashs.AllHashs[i].Attack;
                            status.impactTimer += Time.deltaTime;
                            if (status.impactTimer > setting.impactDuration)
                                status.attackTimerState = AttackTimerState.FinishImpact;
                        }
                        break;
                    case AttackTimerState.FinishImpact:
                        {
                            animationImplement.animationStateProperty.statusValue = SlimeKingAnimationHashs.AllHashs[i].Attack;
                            status.backswingTimer = 0;
                            status.attackTimerState = setting.backswingDuration > 0? AttackTimerState.Backswing : status.attackTimerState = AttackTimerState.Finish;
                        }
                        break;
                    case AttackTimerState.Backswing:
                        {
                            animationImplement.animationStateProperty.statusValue = SlimeKingAnimationHashs.AllHashs[i].After;
                            status.backswingTimer += Time.deltaTime;
                            if (status.backswingTimer > setting.backswingDuration)
                                status.attackTimerState = AttackTimerState.Finish;
                        }
                        break;
                    case AttackTimerState.Finish:
                        {
                            status.attackTimerState = AttackTimerState.Waiting;
                        }
                        break;
                }
                attackTimerManageImpl.attackTimerStatuses[i] = new AttackTimerStatus() { statusValue = status };
            }
        }

        public static void StartCharge(ChargeAttackManageImplement chargeAttackManageImpl, AttackTimerManageImplement attackTimerManageImpl, int timerIndex, Vector2 selfPosition, Vector2 playerPosition)
        {
            chargeAttackManageImpl.ChargeDirectionProperty.statusValue = (playerPosition - selfPosition).normalized;
            chargeAttackManageImpl.ChargeBlazeCounterProperty.statusValue = 0;
            StartTimer(attackTimerManageImpl, timerIndex);
        }

        public static void StartCharge(ChargeAttackManageImplement chargeAttackManageImpl, AttackTimerManageImplement attackTimerManageImpl, IndicatedAttackManageImplement indicatedAttackManageImpl, int timerIndex, Vector2 selfPosition, Vector2 playerPosition)
        {
            StartCharge(chargeAttackManageImpl, attackTimerManageImpl, timerIndex, selfPosition, playerPosition);

            var timerSetting = attackTimerManageImpl.attackTimerSettings[timerIndex].statusValue;
            var angle = Vector2.SignedAngle(playerPosition - selfPosition, Vector2.right);

            indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
            {
                statusValue = new IndicatedAttackDataSet()
                {
                    indicatorType = IndicatorType.Rect,
                    position = selfPosition,
                    scale = Vector2.right * (chargeAttackManageImpl.ChargeVelocityProperty.statusValue * timerSetting.impactDuration) + Vector2.up * 2,
                    rotation = angle,
                    chargeDuration = timerSetting.foreswingDuration,
                    weaponDataSet = new WeaponDataSet(),
                    weaponCount = 0,
                },
            });
        }

        public static void UpdateCharge(CharacterDeathImplement characterDeathImplement, ChargeAttackManageImplement chargeAttackManageImpl, AttackTimerManageImplement attackTimerManageImpl, Transform transformImpl, Rigidbody2D rigidbodyImpl, WeaponFactory weaponFactory, int timerIndex, uint EGID, WeaponDataSet weaponDataSet)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            Vector2 selfPosition = transformImpl.position;

            var attackTimer = attackTimerManageImpl.attackTimerStatuses[timerIndex].statusValue;

            switch (attackTimer.attackTimerState)
            {
                case AttackTimerState.Foreswing:
                    {
                        rigidbodyImpl.velocity = Vector2.zero;
                    }
                    break;
                case AttackTimerState.Impacting:
                    {
                        rigidbodyImpl.velocity = chargeAttackManageImpl.ChargeDirectionProperty.statusValue * chargeAttackManageImpl.ChargeVelocityProperty.statusValue;
                        if (chargeAttackManageImpl.HasChargeBlazeProperty.statusValue)
                        {
                            if (attackTimer.impactTimer > chargeAttackManageImpl.ChargeBlazeIntervalProperty.statusValue * chargeAttackManageImpl.ChargeBlazeCounterProperty.statusValue)
                            {
                                EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, selfPosition);
                                chargeAttackManageImpl.ChargeBlazeCounterProperty.statusValue++;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static void InstantiateIndicatedAttacks(CharacterDeathImplement characterDeathImplement, IndicatedAttackManageImplement indicatedAttackManageImpl, IndicatorFactory indicatorFactory, Vector2 playerPosition)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            foreach(var data in indicatedAttackManageImpl.indicatedAttackDatas)
            {
                var indicatorGo = data.statusValue.indicatorType switch
                {
                    IndicatorType.Circle => indicatorFactory.CreateGameObject("Prefabs/Indicator/EnemyCircleAttackIndicator"),
                    IndicatorType.Fan => indicatorFactory.CreateGameObject("Prefabs/Indicator/EnemyFanwiseAttackIndicator"),
                    IndicatorType.Rect => indicatorFactory.CreateGameObject("Prefabs/Indicator/EnemyRectAttackIndicator"),
                    _ => indicatorFactory.CreateGameObject("Prefabs/Indicator/EnemyCircleAttackIndicator"),
                };
                var indicatorEntity = indicatorGo.GetComponent<EnemyAttackIndicatorEntity>();
                indicatorEntity.transformImplement.position = data.statusValue.position;
                var angleOffset = data.statusValue.indicatorType switch
                {
                    IndicatorType.Fan => -45f,
                    _ => 0f
                };
                indicatorEntity.transformImplement.rotation = Quaternion.Euler(0, 0, data.statusValue.rotation * -1 + angleOffset);
                indicatorEntity.transformImplement.localScale = data.statusValue.scale;

                var angleBetweenWeapon = data.statusValue.indicatorType switch
                {
                    IndicatorType.Fan => 90 / (data.statusValue.weaponCount - 1),
                    _ => 0,
                };

                var fillInitialScale = data.statusValue.indicatorType switch
                {
                    IndicatorType.Rect => Vector2.up,
                    _ => Vector2.zero,
                };

                var fillInitialOffset = Vector2.zero;

                var fillScalePerAlpha = data.statusValue.indicatorType switch
                {
                    IndicatorType.Rect => Vector2.right,
                    _ => Vector2.one,
                };

                var fillOffsetPerAlpha = Vector2.zero;

                indicatedAttackManageImpl.instantiatedIndicatedAttackDatas.Add(new InstantiatedIndicatedAttackData()
                {
                    statusValue = new InstantiatedIndicatedAttackDataSet()
                    {
                        indicatorEgid = indicatorEntity.EGID,
                        indicatorChargeTimer = 0f,
                        indicatorChargeDuration = data.statusValue.chargeDuration,
                        weaponDataSet = data.statusValue.weaponDataSet,
                        weaponCount = data.statusValue.weaponCount,
                        angleBetweenWeapon = angleBetweenWeapon,
                        fillInitialScale = fillInitialScale,
                        fillInitialOffset = fillInitialOffset,
                        fillScalePerAlpha = fillScalePerAlpha,
                        fillOffsetPerAlpha = fillOffsetPerAlpha,
                    },
                });
            }

            indicatedAttackManageImpl.indicatedAttackDatas.Clear();
        }

        public  static void ManageIndicatedAttacks(IndicatedAttackManageImplement indicatedAttackManageImpl, Transform transformImpl, IEntityContainer entityContainer, IndicatorFactory indicatorFactory, WeaponFactory weaponFactory, uint EGID)
        {
            for (int i = 0; i < indicatedAttackManageImpl.instantiatedIndicatedAttackDatas.Count; i++)
            {
                var dataCopy = indicatedAttackManageImpl.instantiatedIndicatedAttackDatas[i];
                var statusCopy = dataCopy.statusValue;
                statusCopy.indicatorChargeTimer += Time.deltaTime;
                if (entityContainer.GetEntity<EnemyAttackIndicatorEntity>(statusCopy.indicatorEgid, out var enemyCircleAttackIndicatorEntity))
                {
                    var alpha = statusCopy.indicatorChargeTimer / statusCopy.indicatorChargeDuration;
                    enemyCircleAttackIndicatorEntity.fillSprite.transform.localScale = dataCopy.statusValue.fillInitialScale + (dataCopy.statusValue.fillScalePerAlpha * alpha);
                    enemyCircleAttackIndicatorEntity.fillSprite.transform.localPosition = dataCopy.statusValue.fillInitialOffset + (dataCopy.statusValue.fillOffsetPerAlpha * alpha);

                    if (statusCopy.indicatorChargeTimer >= statusCopy.indicatorChargeDuration)
                    {
                        dataCopy.shouldDelete = true;
                        for(int j = 0; j < dataCopy.statusValue.weaponCount; j++)
                        {
                            var indicatorRotation = enemyCircleAttackIndicatorEntity.transformImplement.rotation;
                            var weaponRotation = indicatorRotation * Quaternion.Euler(0, 0, dataCopy.statusValue.angleBetweenWeapon * j);
                            EnemyWeaponCreator.CreateWeapon(weaponFactory, statusCopy.weaponDataSet, EGID, enemyCircleAttackIndicatorEntity.transformImplement.position, weaponRotation);
                        }
                        indicatorFactory.EnqueRecycle(enemyCircleAttackIndicatorEntity, enemyCircleAttackIndicatorEntity.SrcPathHashCode);
                    }
                }
                else
                {
                    dataCopy.shouldDelete = true;
                }
                dataCopy.statusValue = statusCopy;
                indicatedAttackManageImpl.instantiatedIndicatedAttackDatas[i] = dataCopy;
            }

            indicatedAttackManageImpl.instantiatedIndicatedAttackDatas.RemoveAll(elmt => elmt.shouldDelete);

            if (indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue > 0)
                indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue -= Time.deltaTime;
        }
    }

    public static class EnemyWeaponCreator
    {
        public static void CreateWeapon(WeaponFactory weaponFactory, WeaponDataSet weaponData, uint EGID, Vector2 targetPosition)
        {
            CreateWeapon(weaponData, targetPosition, 1, 1, EGID, weaponFactory);
        }

        public static void CreateWeapon(WeaponFactory weaponFactory, WeaponDataSet weaponData, uint EGID, Vector2 targetPosition, Quaternion rotation)
        {
            CreateWeapon(weaponData, targetPosition, rotation, 1, 1, EGID, weaponFactory);
        }

        public static void CreateSuicideBombing(WeaponFactory weaponFactory, uint EGID, Vector2 targetPosition, int damage, float area)
        {
            CreateWeapon(new WeaponDataSet()
            {
                id = string.Empty,
                level = 1,
                damage = 1,
                coolTime = 0,
                isSingleCreation = false,
                isSummonCreation = false,
                prefabPath = $"Prefabs/Projectile/EnemySuicideBombing",
                createCount = 1,
                projectileSpeed = 0,
                multiCreationTick = 0,
                criticalRatio = 0,
                criticalProbability = 0,
                spearCount = 0,
                knockBack = 0,
                attackType = AttackType.PHYSICS,
                area = area,
                lifeDuration = 0.25f,
            }, targetPosition, 1, 1, EGID, weaponFactory);
        }

        public static WeaponDataSet GetCommonBlaze()
        {
            return new WeaponDataSet()
            {
                id = string.Empty,
                level = 1,
                damage = 1,
                coolTime = 0,
                isSingleCreation = false,
                isSummonCreation = false,
                prefabPath = $"Prefabs/Projectile/EnemyBlaze",
                createCount = 1,
                projectileSpeed = 0,
                multiCreationTick = 0,
                criticalRatio = 0,
                criticalProbability = 0,
                spearCount = 0,
                knockBack = 0,
                attackType = AttackType.PHYSICS,
                area = 1,
                lifeDuration = 1f,
            };
        }

        private static AttackEntity CreateWeaponInternal(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount, uint EGID, WeaponFactory weaponFactory)
        {
            var go = weaponFactory.CreateGameObject(d.prefabPath);
            if (go == null)
            {
                return null;
            }

            var attackEntity = go.GetComponent<AttackEntity>();
            attackEntity.transformImplement.position = targetPosition;

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
                        statusValue = CharacterTypes.Enemy
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
                            statusValue = CharacterTypes.Enemy
                        },
                        buffValue = d.area
                    }
                });
            }

            return attackEntity;
        }

        private static void CreateWeapon(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount, uint EGID, WeaponFactory weaponFactory)
        {
            var attackEntity = CreateWeaponInternal(d, targetPosition, cnt, maxCount, EGID, weaponFactory);

            attackEntity.OnApplyShootCountChanged();
        }

        private static void CreateWeapon(WeaponDataSet d, Vector2 targetPosition, Quaternion rotation, int cnt, int maxCount, uint EGID, WeaponFactory weaponFactory)
        {
            var attackEntity = CreateWeaponInternal(d, targetPosition, cnt, maxCount, EGID, weaponFactory);
            attackEntity.transformImplement.rotation = rotation;
            attackEntity.translateImplement.moveDirectionProperty.statusValue = attackEntity.transformImplement.rotation * Vector2.right;

            attackEntity.OnApplyShootCountChanged();
        }
    }
}
