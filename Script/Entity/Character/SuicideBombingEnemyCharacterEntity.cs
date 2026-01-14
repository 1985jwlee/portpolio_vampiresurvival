using Reflex.Scripts.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class SuicideBombingEnemyCharacterEntity : SeekTargetEnemyCharacterEntity
    {
        [Inject] protected IndicatorFactory indicatorFactory;

        public SuicideBombingEnemyPatternImplement patternImplement;
        public LinkedEgidImplement linkedEgidImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            patternImplement = GetComponent<SuicideBombingEnemyPatternImplement>();
            linkedEgidImplement = GetComponent<LinkedEgidImplement>();

            Components.Add(patternImplement);
            Components.Add(linkedEgidImplement);
        }

        protected override void Update()
        {
            base.Update();
            UpdatePattern();
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();

            if (entityContainer.GetEntity(linkedEgidImplement.linkedEgidProperty.statusValue, out SeekingEnemyCircleAttackIndicatorEntity indicatorEntity))
            {
                indicatorFactory.EnqueRecycle(indicatorEntity, indicatorEntity.SrcPathHashCode);
            }
        }

        public void UpdatePattern()
        {
            var playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            UpdatePattern(patternImplement, transformImplement, buffImplement, statusImplement, linkedEgidImplement, entityContainer, weaponFactory, indicatorFactory, playerPosition, EGID);

            static void UpdatePattern(SuicideBombingEnemyPatternImplement patternImpl, Transform transformImpl, BuffImplement buffImpl, CommonStatusImplement statusImpl, LinkedEgidImplement linkedEgidImpl, IEntityContainer entityContainer, WeaponFactory weaponFactory, IndicatorFactory indicatorFactory, Vector2 playerPosition, uint EGID)
            {
                Vector2 selfPosition = transformImpl.position;

                switch (patternImpl.StateProperty.statusValue)
                {
                    case SuicideBombingState.Idle:
                        {
                            if(Vector2.Distance(selfPosition, entityContainer.playerCharacterEntity.rigidBodyImplement.ClosestPoint(selfPosition)) < patternImpl.DetectionRadiusProperty.statusValue)
                            {
                                var indicatorGo = indicatorFactory.CreateGameObject("Prefabs/Indicator/SeekTargetEnemyCircleAttackIndicator");
                                var indicatorEntity = indicatorGo.GetComponent<SeekingEnemyCircleAttackIndicatorEntity>();
                                indicatorEntity.transformImplement.position = transformImpl.position;
                                linkedEgidImpl.linkedEgidProperty.statusValue = indicatorEntity.EGID;
                                indicatorEntity.seekingTargetImplement.seekingTargetProperty.seekTargetEGID = EGID;
                                indicatorEntity.seekingTargetImplement.seekingTargetProperty.seekingTime = 0f;
                                indicatorEntity.seekingTargetImplement.seekingTargetProperty.startSeek = true;
                                indicatorEntity.transform.localScale = patternImpl.ExplosionRadiusProperty.statusValue * 2 * Vector3.one;

                                patternImpl.StateProperty.statusValue = SuicideBombingState.Charging;
                                break;
                            }
                        }
                        break;
                    case SuicideBombingState.Charging:
                        {
                            patternImpl.ExplosionDelayFillProperty.statusValue += Time.deltaTime;

                            if (entityContainer.GetEntity(linkedEgidImpl.linkedEgidProperty.statusValue, out SeekingEnemyCircleAttackIndicatorEntity indicatorEntity))
                            {
                                indicatorEntity.fillSprite.transform.localScale = Mathf.Clamp01(patternImpl.ExplosionDelayFillProperty.statusValue / patternImpl.ExplosionDelayProperty.statusValue) * Vector3.one;
                            }

                            if(patternImpl.ExplosionDelayFillProperty.statusValue >= patternImpl.ExplosionDelayProperty.statusValue)
                                patternImpl.StateProperty.statusValue = SuicideBombingState.Explode;
                        }
                        break;
                    case SuicideBombingState.Explode:
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
                                area = patternImpl.ExplosionRadiusProperty.statusValue * 2,
                            }, selfPosition, 1, 1, EGID, weaponFactory);
                            buffImpl.appliedBuff.Add(new Buff()
                            {
                                statusValue = new BuffData()
                                {
                                    buffDeviceId = string.Empty,
                                    buffType = BuffType.Damage,
                                    buffValue = statusImpl.hitPointProperty.statusValue,
                                    remainTime = -1,
                                    rootCharacter = new CharacterType()
                                    {
                                        statusValue = CharacterTypes.Enemy
                                    },
                                    rootEntityId = EGID,
                                }
                            });
                            if (entityContainer.GetEntity(linkedEgidImpl.linkedEgidProperty.statusValue, out SeekingEnemyCircleAttackIndicatorEntity indicatorEntity))
                            {
                                indicatorFactory.EnqueRecycle(indicatorEntity, indicatorEntity.SrcPathHashCode);
                            }

                            patternImpl.StateProperty.statusValue = SuicideBombingState.None;
                        }
                        break;
                }
            }
        }

        public static void CreateWeapon(WeaponDataSet d, Vector2 targetPosition, int cnt, int maxCount, uint EGID, WeaponFactory weaponFactory)
        {
            var go = weaponFactory.CreateGameObject(d.prefabPath);
            if (go == null)
            {
                return;
            }

            var attackEntity = go.GetComponent<AttackEntity>();
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

            attackEntity.OnApplyShootCountChanged();
        }
    }
}