using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class SlimeKingEnemyCharacterEntity : BossEnemyCharacterEntity
    {
        [Inject] EnemyCharacterFactory enemyCharacterFactory;

        public SlimeKingEnemyPatternImplement patternImplement;
        public AttackTimerManageImplement attackTimerManageImplement;
        public GeneralTimerManageImplement generalTimerManageImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if(TryGetComponent(out patternImplement))
            {
                Components.Add(patternImplement);
            }

            if (TryGetComponent(out attackTimerManageImplement))
            {
                Components.Add(attackTimerManageImplement);
            }

            if (TryGetComponent(out generalTimerManageImplement))
            {
                Components.Add(generalTimerManageImplement);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
        }

        protected override void Update()
        {
            base.Update();
            SpecialEnemyFunctionalities.UpdateAttackTimer(characterDeathImplement, attackTimerManageImplement, animationImplement);
            SpecialEnemyFunctionalities.UpdateTimer(generalTimerManageImplement);
            UpdatePattern();
        }
        public void UpdatePattern()
        {
            var playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            UpdateAttack1(patternImplement, attackTimerManageImplement, characterDeathImplement, monsterDamageBaseValueImplement, transformImplement, weaponFactory, EGID);
            UpdateAttack2(patternImplement, attackTimerManageImplement, characterDeathImplement, transformImplement, animationImplement, enemyCharacterFactory);
            UpdateMoving(patternImplement, attackTimerManageImplement, generalTimerManageImplement, characterDeathImplement, transform, translateImplement, rigidBodyImplement, playerPosition);

            static void UpdateAttack1(SlimeKingEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, CharacterDeathImplement characterDeathImplement, MonsterDamageBaseValueImplement monsterDamageBaseValueImpl, Transform transformImpl, WeaponFactory weaponFactory, uint EGID)
            {
                if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                    return;

                var parameter = patternImpl.parameter.statusValue;
                var executionContext = patternImpl.executionContext.statusValue;
                Vector2 selfPosition = transformImpl.position;

                var attack1Timer = attackTimerManageImpl.attackTimerStatuses[0].statusValue;

                switch (attack1Timer.attackTimerState)
                {
                    case AttackTimerState.StartImpact:
                        {
                            executionContext.attack1_shootingCount = 0;
                        }
                        break;
                    case AttackTimerState.Impacting:
                        {
                            if(attack1Timer.impactTimer > parameter.attack1_shootingInterval * executionContext.attack1_shootingCount)
                            {
                                for (int i = 0; i < parameter.attack1_ProjectileNum; i++)
                                {
                                    float angle = 360f / parameter.attack1_ProjectileNum * i;
                                    if (executionContext.attack1_shootingCount % 2 == 0)
                                        angle += 22.5f;

                                    var damage = Mathf.FloorToInt(monsterDamageBaseValueImpl.DamageBaseValueProperty.statusValue * parameter.attack1_DamageMultiplier);
                                    var weaponDataSet = SlimeKingWeaponDataSet(damage, parameter.attack1_ProjectileSpeed, parameter.attack1_ProjectileLifeTime);
                                    EnemyWeaponCreator.CreateWeapon(weaponFactory, weaponDataSet, EGID, selfPosition, Quaternion.AngleAxis(angle, Vector3.forward));
                                }
                                executionContext.attack1_shootingCount++;
                            }
                        }
                        break;
                    default:
                        break;
                }

                patternImpl.executionContext.statusValue = executionContext;
            }

            static void UpdateAttack2(SlimeKingEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, AnimationImplement animationImpl, EnemyCharacterFactory enemyCharacterFactory)
            {
                if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                    return;

                var parameter = patternImpl.parameter.statusValue;
                Vector2 selfPosition = transformImpl.position;

                var attack2Timer = attackTimerManageImpl.attackTimerStatuses[1].statusValue;

                switch (attack2Timer.attackTimerState)
                {
                    case AttackTimerState.Foreswing:
                        {
                        }
                        break;
                    case AttackTimerState.StartImpact:
                        {
                            enemyCharacterFactory.CreateWave(new Timeline.WaveCreateMarkerData()
                            {
                                monsterId = "5",
                                type = Timeline.WaveCreateMarkerData.Type.Circle,
                                num = parameter.attack2_SpawnNum,
                                radius = 2,
                                colliderRadius = 1,
                            }, selfPosition);
                        }
                        break;
                    default:
                        break;
                }
            }

            static void UpdateMoving(SlimeKingEnemyPatternImplement patternImpl, AttackTimerManageImplement attackTimerManageImpl, GeneralTimerManageImplement generalTimerManageImpl, CharacterDeathImplement characterDeathImplement, Transform transformImpl, TranslateImplement translateImpl, Rigidbody2D rigidbodyImpl, Vector2 playerPosition)
            {
                if (characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                    return;

                var parameter = patternImpl.parameter.statusValue;
                var executionContext = patternImpl.executionContext.statusValue;
                Vector2 selfPosition = transformImpl.position;

                var attack1Timer = attackTimerManageImpl.attackTimerStatuses[0].statusValue;
                var attack2Timer = attackTimerManageImpl.attackTimerStatuses[1].statusValue;
                var movingTimer = generalTimerManageImpl.timerStatuses[0].statusValue;

                if (attack1Timer.attackTimerState != AttackTimerState.Waiting)
                    return;
                if (attack2Timer.attackTimerState != AttackTimerState.Waiting)
                    return;

                switch (movingTimer.timerState)
                {
                    case GeneralTimerState.Waiting:
                        {
                            SpecialEnemyFunctionalities.StartTimer(generalTimerManageImpl, 0, Random.Range(parameter.moving_DurationMin, parameter.moving_DurationMax));
                        }
                        break;
                    case GeneralTimerState.Counting:
                        {
                            var normalizeValue = (playerPosition - selfPosition).normalized;
                            translateImpl.moveDirectionProperty.statusValue = normalizeValue;
                            rigidbodyImpl.velocity = normalizeValue * translateImpl.velocityProperty.statusValue;
                        }
                        break;
                    case GeneralTimerState.Finish:
                        {
                            rigidbodyImpl.velocity = Vector2.zero;

                            if (executionContext.lastAttackKind == 1)
                            {
                                SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, 0);
                                executionContext.lastAttackKind = 0;
                            }
                            else
                            {
                                SpecialEnemyFunctionalities.StartTimer(attackTimerManageImpl, 1);
                                executionContext.lastAttackKind = 1;
                            }
                        }
                        break;
                }

                patternImpl.executionContext.statusValue = executionContext;
            }
        }

        private static WeaponDataSet SlimeKingWeaponDataSet(int damage, float projectileSpeed, float lifeDuration) => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = damage,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLinearProjectileSlimeKing",
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
