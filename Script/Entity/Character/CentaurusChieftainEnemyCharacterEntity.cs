using Cysharp.Threading.Tasks.Triggers;
using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.ECS
{
    public class CentaurusChieftainEnemyCharacterEntity : BossEnemyCharacterEntity
    {
        [Inject] IndicatorFactory indicatorFactory;

        public CentaurusChieftainEnemyPatternImplement patternImplement;
        public IndicatedAttackManageImplement indicatedAttackManageImplement;
        public ChargeAttackManageImplement chargeAttackManageImplement;
        public AttackTimerManageImplement attackTimerManageImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if (TryGetComponent(out patternImplement))
                Components.Add(patternImplement);

            if (TryGetComponent(out indicatedAttackManageImplement))
                Components.Add(indicatedAttackManageImplement);

            if (TryGetComponent(out chargeAttackManageImplement))
                Components.Add(chargeAttackManageImplement);

            if (TryGetComponent(out attackTimerManageImplement))
                Components.Add(attackTimerManageImplement);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();

            foreach (var data in indicatedAttackManageImplement.instantiatedIndicatedAttackDatas)
            {
                if (entityContainer.GetEntity<EnemyAttackIndicatorEntity>(data.statusValue.indicatorEgid, out var enemyCircleAttackIndicatorEntity))
                {
                    indicatorFactory.EnqueRecycle(enemyCircleAttackIndicatorEntity, enemyCircleAttackIndicatorEntity.SrcPathHashCode);
                }
            }
            indicatedAttackManageImplement.instantiatedIndicatedAttackDatas.Clear();
        }

        protected override void Update()
        {
            base.Update();
            UpdatePattern();
            SpecialEnemyFunctionalities.UpdateAttackTimer(characterDeathImplement, attackTimerManageImplement, animationImplement);
            SpecialEnemyFunctionalities.InstantiateIndicatedAttacks(characterDeathImplement, indicatedAttackManageImplement, indicatorFactory, entityContainer.playerCharacterEntity.transformImplement.position);
            SpecialEnemyFunctionalities.ManageIndicatedAttacks(indicatedAttackManageImplement, transformImplement, entityContainer, indicatorFactory, weaponFactory, EGID);
            int damage = Mathf.FloorToInt(monsterDamageBaseValueImplement.DamageBaseValueProperty.statusValue * patternImplement.parameter.statusValue.attack1_DamageMultiplier);
            SpecialEnemyFunctionalities.UpdateCharge(characterDeathImplement, chargeAttackManageImplement, attackTimerManageImplement, transformImplement, rigidBodyImplement, weaponFactory, 1, EGID, BlazeWeaponDataSet(damage, patternImplement.parameter.statusValue.attack2_ProjectileLifeTime));
        }

        public void UpdatePattern()
        {
            UpdatePattern(patternImplement, chargeAttackManageImplement, indicatedAttackManageImplement, attackTimerManageImplement, monsterDamageBaseValueImplement, characterDeathImplement, transformImplement, rigidBodyImplement, translateImplement, entityContainer);
            static void UpdatePattern(CentaurusChieftainEnemyPatternImplement patternImpl, ChargeAttackManageImplement chargeAttackManageImpl, IndicatedAttackManageImplement indicatedAttackManageImpl, AttackTimerManageImplement attackTimerManageImpl, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl, IEntityContainer entityContainer)
            {
                if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                    return;

                int shootingAttackTimerIndex = 0;
                int chargeAttackTimerIndex = 1;

                if (indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue > 0)
                    return;

                var shootingTimerSetting = attackTimerManageImpl.attackTimerSettings[shootingAttackTimerIndex].statusValue;
                var chargeTimer = attackTimerManageImpl.attackTimerStatuses[chargeAttackTimerIndex].statusValue;

                if (chargeTimer.attackTimerState != AttackTimerState.Waiting)
                    return;

                var playerPosition = entityContainer.playerCharacterEntity.transform.position;
                if (patternImpl.CoolDownTimerProperty.statusValue > 0)
                {
                    float distance = Vector2.Distance(playerPosition, transformImpl.position);
                    var toPlayerDirection = (transformImpl.position - playerPosition).normalized;

                    var direction = new Vector2(toPlayerDirection.y, -toPlayerDirection.x);
                    if (distance < patternImpl.DistanceMinToMaintainProperty.statusValue)
                    {
                        direction = toPlayerDirection;
                    }
                    else if(distance > patternImpl.DistanceMaxToMaintainProperty.statusValue)
                    {
                        direction = toPlayerDirection * -1;
                    }

                    rigidbodyImpl.velocity = direction * translateImpl.velocityProperty.statusValue;
                    translateImpl.moveDirectionProperty.statusValue = direction;

                    patternImpl.CoolDownTimerProperty.statusValue -= Time.deltaTime;
                }
                else
                {
                    rigidbodyImpl.velocity = Vector2.zero;

                    var fanwiseProbability = patternImpl.FanwiseShootingProbabilityProperty.statusValue;
                    var chargeProbability = patternImpl.ChargeProbabilityProperty.statusValue;
                    if (Random.value < fanwiseProbability/(fanwiseProbability + chargeProbability))
                    {
                        var angle = Vector2.SignedAngle(playerPosition - transformImpl.position, Vector2.right);

                        var damage = Mathf.FloorToInt(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * patternImpl.parameter.statusValue.attack1_DamageMultiplier);
                        var projectileSpeed = patternImpl.parameter.statusValue.attack1_ProjectileSpeed;
                        var projectileLifeTime = patternImpl.parameter.statusValue.attack1_ProjectileLifeTime;

                        indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
                        {
                            statusValue = new IndicatedAttackDataSet()
                            {
                                indicatorType = IndicatorType.Fan,
                                position = transformImpl.position,
                                scale = Vector2.one * 8f,
                                rotation = angle,
                                chargeDuration = shootingTimerSetting.foreswingDuration,
                                weaponDataSet = BowWeaponDataSet(damage, projectileSpeed, projectileLifeTime),
                                weaponCount = 5,
                            },
                        });

                        indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue = shootingTimerSetting.foreswingDuration;
                        SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, shootingAttackTimerIndex);
                    }
                    else
                    {
                        SpecialEnemyFunctionalities.StartCharge(chargeAttackManageImpl, attackTimerManageImpl, indicatedAttackManageImpl, chargeAttackTimerIndex, transformImpl.position, playerPosition);
                    }

                    patternImpl.CoolDownTimerProperty.statusValue = Random.Range(patternImpl.CoolDownMinProperty.statusValue, patternImpl.CoolDownMaxProperty.statusValue);
                }
            }
        }

        private static WeaponDataSet BowWeaponDataSet(int damage, float projectileSpeed,  float lifeTime) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLinearProjectileDiyapblo",
            createCount = 1,
            projectileSpeed = projectileSpeed,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 1,
            lifeDuration = lifeTime,
        };

        private static WeaponDataSet BlazeWeaponDataSet(int damage, float lifeTime) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyBlazeDiyapblo",
            createCount = 1,
            projectileSpeed = 0,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 1,
            lifeDuration = lifeTime,
        };
    }
}
