using Reflex.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.ECS
{
    public class RelicGolemEnemyCharacterEntity : BossEnemyCharacterEntity
    {
        [Inject] IndicatorFactory indicatorFactory;

        public RelicGolemEnemyPatternImplement patternImplement;
        public AttackTimerManageImplement attackTimerManageImplement;
        public GeneralTimerManageImplement generalTimerManageImplement;
        public IndicatedAttackManageImplement indicatedAttackManageImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if (TryGetComponent(out patternImplement))
                Components.Add(patternImplement);

            if (TryGetComponent(out attackTimerManageImplement))
                Components.Add(attackTimerManageImplement);

            if (TryGetComponent(out generalTimerManageImplement))
                Components.Add(generalTimerManageImplement);

            if (TryGetComponent(out indicatedAttackManageImplement))
                Components.Add(indicatedAttackManageImplement);
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
            SpecialEnemyFunctionalities.UpdateTimer(generalTimerManageImplement);
            SpecialEnemyFunctionalities.InstantiateIndicatedAttacks(characterDeathImplement, indicatedAttackManageImplement, indicatorFactory, entityContainer.playerCharacterEntity.transformImplement.position);
            SpecialEnemyFunctionalities.ManageIndicatedAttacks(indicatedAttackManageImplement, transformImplement, entityContainer, indicatorFactory, weaponFactory, EGID);
        }
        public void UpdatePattern()
        {
            Vector2 playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            UpdatePattern(patternImplement, attackTimerManageImplement, generalTimerManageImplement, indicatedAttackManageImplement, characterDeathImplement, transformImplement, rigidBodyImplement, playerPosition);
            UpdateAttack1(patternImplement, attackTimerManageImplement, monsterDamageBaseValueImplement, characterDeathImplement, transformImplement, weaponFactory, playerPosition, EGID);
            UpdateAttack2(patternImplement, attackTimerManageImplement, monsterDamageBaseValueImplement, characterDeathImplement, transformImplement, weaponFactory, playerPosition, EGID);
            UpdateAttack3(patternImplement, attackTimerManageImplement, monsterDamageBaseValueImplement, characterDeathImplement,transformImplement, weaponFactory, playerPosition, EGID);
            UpdateAttack4(patternImplement, attackTimerManageImplement, monsterDamageBaseValueImplement, characterDeathImplement, transformImplement, weaponFactory, playerPosition, EGID);

            static void UpdatePattern(RelicGolemEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, GeneralTimerManageImplement generalTimerManageImpl, IndicatedAttackManageImplement indicatedAttackManageImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, Rigidbody2D rigidbodyImpl, Vector2 playerPosition)
            {
                if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                    return;

                var attack1TimerIndex = 0;
                var attack2TimerIndex = 1;
                var attack3TimerIndex = 2;
                var attack4TimerIndex = 3;
                var coolDownTimerIndex = 0;

                var attack1TimerSetting = attackTimerManageImpl.attackTimerSettings[attack1TimerIndex].statusValue;
                var attack4TimerSetting = attackTimerManageImpl.attackTimerSettings[attack4TimerIndex].statusValue;

                var attack1Timer = attackTimerManageImpl.attackTimerStatuses[attack1TimerIndex].statusValue;
                var attack2Timer = attackTimerManageImpl.attackTimerStatuses[attack2TimerIndex].statusValue;
                var attack3Timer = attackTimerManageImpl.attackTimerStatuses[attack3TimerIndex].statusValue;
                var attack4Timer = attackTimerManageImpl.attackTimerStatuses[attack4TimerIndex].statusValue;
                var coolDownTimer = generalTimerManageImpl.timerStatuses[coolDownTimerIndex].statusValue;

                rigidbodyImpl.velocity = Vector2.zero;

                if (attack1Timer.attackTimerState != AttackTimerState.Waiting) return;
                if (attack2Timer.attackTimerState != AttackTimerState.Waiting) return;
                if (attack3Timer.attackTimerState != AttackTimerState.Waiting) return;
                if (attack4Timer.attackTimerState != AttackTimerState.Waiting) return;

                switch(coolDownTimer.timerState)
                {
                    case GeneralTimerState.Waiting:
                        {
                            SpecialEnemyFunctionalities.StartTimer(generalTimerManageImpl, coolDownTimerIndex, patternImpl.parameter.statusValue.coolTime);
                        }
                        break;
                    case GeneralTimerState.Finish:
                        {
                            float attack1 = patternImpl.parameter.statusValue.attack1Probability;
                            float attack2 = patternImpl.parameter.statusValue.attack2Probability;
                            float attack3 = patternImpl.parameter.statusValue.attack3Probability;
                            if (Vector2.Distance(transformImpl.position, playerPosition) < patternImpl.parameter.statusValue.nearPlayerDistance)
                            {
                                attack1 = patternImpl.parameter.statusValue.nearPlayerAttack1Probability;
                                attack2 = patternImpl.parameter.statusValue.nearPlayerAttack2Probability;
                                attack3 = patternImpl.parameter.statusValue.nearPlayerAttack3Probability;
                            }
                            var random = UnityEngine.Random.value;

                            if (random < attack1)
                                StartAttack1();
                            else if(random < attack1 + attack2)
                                StartAttack2();
                            else if (random < attack1 + attack2 + attack3)
                                StartAttack3();
                            else
                                StartAttack4();
                        }
                        break;
                }

                void StartAttack1()
                {
                    indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
                    {
                        statusValue = new IndicatedAttackDataSet()
                        {
                            indicatorType = IndicatorType.Circle,
                            position = transformImpl.position,
                            scale = Vector2.one * patternImpl.parameter.statusValue.attack1_Range,
                            rotation = 0,
                            chargeDuration = attack1TimerSetting.foreswingDuration,
                            weaponDataSet = new WeaponDataSet(),
                            weaponCount = 0,
                        },
                    });

                    indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue = attack1TimerSetting.foreswingDuration;
                    SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, attack1TimerIndex);
                }
                void StartAttack2()
                {
                    SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, attack2TimerIndex);
                }
                void StartAttack3()
                {
                    SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, attack3TimerIndex);
                }
                void StartAttack4()
                {
                    var indicatorData = new IndicatedAttackDataSet()
                    {
                        indicatorType = IndicatorType.Rect,
                        position = transformImpl.position + new Vector3(1, 0),
                        scale = new Vector2(5, 1),
                        rotation = 0,
                        chargeDuration = attack4TimerSetting.foreswingDuration,
                        weaponDataSet = new WeaponDataSet(),
                        weaponCount = 0,
                    };
                    indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
                    {
                        statusValue = indicatorData,
                    });

                    indicatorData.position = transformImpl.position + new Vector3(-1, 0);
                    indicatorData.rotation = 180;

                    indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
                    {
                        statusValue = indicatorData,
                    });

                    indicatedAttackManageImpl.IndicatedAttackDurationTimerProperty.statusValue = attack4TimerSetting.foreswingDuration;
                    SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, attack4TimerIndex);
                }
            }
        }

        private static void UpdateAttack1(RelicGolemEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, WeaponFactory weaponFactory, Vector2 playerPosition, uint EGID)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            var parameter = patternImpl.parameter.statusValue;
            var executionContext = patternImpl.executionContext.statusValue;

            var attack1Timer = attackTimerManageImpl.attackTimerStatuses[0].statusValue;

            switch (attack1Timer.attackTimerState)
            {
                case AttackTimerState.Start:
                    {
                        executionContext.attack1_isImpacted = false;
                    }
                    break;
                case AttackTimerState.Impacting:
                    {
                        if(executionContext.attack1_isImpacted == false && attack1Timer.impactTimer > parameter.attack1_impactDelay)
                        {
                            var damage = Mathf.FloorToInt(Mathf.Min(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * parameter.attack1_DamageMultiplier));
                            var weaponDataSet = Attack1WeaponDataSet(damage, parameter.attack1_Range);

                            EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, transformImpl.position);
                            executionContext.attack1_isImpacted = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            patternImpl.executionContext.statusValue = executionContext;
        }

        private static void UpdateAttack2(RelicGolemEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, WeaponFactory weaponFactory, Vector2 playerPosition, uint EGID)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            var parameter = patternImpl.parameter.statusValue;
            var executionContext = patternImpl.executionContext.statusValue;

            var attack2Timer = attackTimerManageImpl.attackTimerStatuses[1].statusValue;

            switch (attack2Timer.attackTimerState)
            {
                case AttackTimerState.Start:
                    {
                        executionContext.attack2_shootingCounter = 0;
                    }
                    break;
                case AttackTimerState.Impacting:
                    {
                        if (attack2Timer.impactTimer > executionContext.attack2_shootingCounter * parameter.attack2_interval)
                        {
                            var playerAngle = Vector2.SignedAngle(playerPosition - (Vector2)transformImpl.position, Vector2.right);
                            var angleMin = playerAngle - parameter.attack2_angleRange;
                            var angleMax = playerAngle + parameter.attack2_angleRange;

                            var damage = Mathf.FloorToInt(Mathf.Min(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * parameter.attack2_DamageMultiplier));
                            var weaponDataSet = Attack2WeaponDataSet(damage, parameter.attack2_ProjectileSpeed, parameter.attack2_ProjectileLifeTime);

                            for (int i = 0; i < parameter.attack2_projectileNum; i++)
                            {
                                float angle = UnityEngine.Random.Range(angleMin, angleMax);
                                EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, transformImpl.position, Quaternion.Euler(0f, 0, -angle));
                            }

                            executionContext.attack2_shootingCounter++;
                        }
                    }
                    break;
                default:
                    break;
            }

            patternImpl.executionContext.statusValue = executionContext;
        }

        private static void UpdateAttack3(RelicGolemEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, WeaponFactory weaponFactory, Vector2 playerPosition, uint EGID)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            var parameter = patternImpl.parameter.statusValue;
            var executionContext = patternImpl.executionContext.statusValue;

            var attack3Timer = attackTimerManageImpl.attackTimerStatuses[2].statusValue;

            switch (attack3Timer.attackTimerState)
            {
                case AttackTimerState.Start:
                    {
                        executionContext.attack3_shootingCounter = 0;
                    }
                    break;
                case AttackTimerState.Impacting:
                    {
                        if (attack3Timer.impactTimer > executionContext.attack3_shootingCounter * parameter.attack3_interval)
                        {
                            var damage = Mathf.FloorToInt(Mathf.Min(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * parameter.attack3_DamageMultiplier));
                            var weaponDataSet = Attack3WeaponDataSet(damage, parameter.attack3_ProjectileSpeed, parameter.attack3_ProjectileLifeTime);

                            for (int i = 0; i < parameter.attack3_projectileNum; i++)
                            {
                                float angle = UnityEngine.Random.Range(0, 360);
                                EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, transformImpl.position, Quaternion.AngleAxis(angle, Vector3.forward));
                            }

                            executionContext.attack3_shootingCounter++;
                        }
                    }
                    break;
                default:
                    break;
            }

            patternImpl.executionContext.statusValue = executionContext;
        }

        private static void UpdateAttack4(RelicGolemEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, WeaponFactory weaponFactory, Vector2 playerPosition, uint EGID)
        {
            if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                return;

            var parameter = patternImpl.parameter.statusValue;
            var executionContext = patternImpl.executionContext.statusValue;

            var attack4Timer = attackTimerManageImpl.attackTimerStatuses[3].statusValue;

            switch (attack4Timer.attackTimerState)
            {
                case AttackTimerState.Start:
                    {
                        executionContext.attack4_shootingCounter = 0;
                        executionContext.attack4_angle = 0;
                    }
                    break;
                case AttackTimerState.Impacting:
                    {
                        executionContext.attack4_angle += parameter.attack4_angleSpeed * Time.deltaTime;
                        if (attack4Timer.impactTimer > executionContext.attack4_shootingCounter * parameter.attack4_interval)
                        {
                            var damage = Mathf.FloorToInt(Mathf.Min(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * parameter.attack4_DamageMultiplier));
                            var weaponDataSet = Attack4WeaponDataSet(damage, parameter.attack4_ProjectileSpeed, parameter.attack4_ProjectileLifeTime);

                            EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, transformImpl.position, Quaternion.AngleAxis(executionContext.attack4_angle, Vector3.forward));
                            EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, transformImpl.position, Quaternion.AngleAxis(executionContext.attack4_angle + 180, Vector3.forward));

                            executionContext.attack4_shootingCounter++;
                        }
                    }
                    break;
                default:
                    break;
            }

            patternImpl.executionContext.statusValue = executionContext;
        }

        private static WeaponDataSet Attack1WeaponDataSet(int damage, float area) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemySuicideBombingRelicGolem",
            createCount = 1,
            projectileSpeed = 0f,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = area,
            lifeDuration = 0.25f,
        };

        private static WeaponDataSet Attack2WeaponDataSet(int damage, float projectileSpeed, float lifeDuration) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLinearProjectileRelicGolem_Attack2",
            createCount = 1,
            projectileSpeed = projectileSpeed,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 0,
            lifeDuration = lifeDuration,
        };

        private static WeaponDataSet Attack3WeaponDataSet(int damage, float projectileSpeed, float lifeDuration) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLinearProjectileRelicGolem_Attack3",
            createCount = 1,
            projectileSpeed = projectileSpeed,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 0,
            lifeDuration = lifeDuration,
        };

        private static WeaponDataSet Attack4WeaponDataSet(int damage, float projectileSpeed, float lifeDuration) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLinearProjectileRelicGolem_Attack4",
            createCount = 1,
            projectileSpeed = projectileSpeed,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 0,
            lifeDuration = lifeDuration,
        };
    }
}
