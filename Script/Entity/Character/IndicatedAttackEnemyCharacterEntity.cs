using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class IndicatedAttackEnemyCharacterEntity : EnemyCharacterEntity
    {
        [Inject] protected IndicatorFactory indicatorFactory;

        IndicatedAttackManageImplement indicatedAttackManageImplement;
        IndicatedAttackEnemyPatternImplement patternImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            indicatedAttackManageImplement = GetComponent<IndicatedAttackManageImplement>();
            patternImplement = GetComponent<IndicatedAttackEnemyPatternImplement>();

            Components.Add(indicatedAttackManageImplement);
            Components.Add(patternImplement);
        }

        protected override void Update()
        {
            base.Update();
            var playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            SpecialEnemyFunctionalities.InstantiateIndicatedAttacks(characterDeathImplement, indicatedAttackManageImplement, indicatorFactory, playerPosition);
            SpecialEnemyFunctionalities.ManageIndicatedAttacks(indicatedAttackManageImplement, transformImplement, entityContainer, indicatorFactory, weaponFactory, EGID);
            UpdatePattern();
        }

        public void UpdatePattern()
        {
            UpdatePattern(patternImplement, indicatedAttackManageImplement, transformImplement, rigidBodyImplement, entityContainer);
            
            static void UpdatePattern(IndicatedAttackEnemyPatternImplement patternImpl, IndicatedAttackManageImplement indicatedAttackManageImpl, Transform transformImpl, Rigidbody2D rigidbodyImpl, IEntityContainer entityContainer)
            {
                rigidbodyImpl.velocity = Vector2.zero;

                Vector2 playerPosition = entityContainer.playerCharacterEntity.transform.position;
                Vector2 selfPosition = transformImpl.position;
                if(patternImpl.AttackCoolDownTimerProperty.statusValue <= 0)
                {
                    var playerClosestPoint = entityContainer.playerCharacterEntity.rigidBodyImplement.ClosestPoint(selfPosition);
                    if (Vector2.Distance(selfPosition, playerClosestPoint) < patternImpl.DetectionRadiusProperty.statusValue)
                    {
                        indicatedAttackManageImpl.indicatedAttackDatas.Add(new IndicatedAttackData()
                        {
                            statusValue = new IndicatedAttackDataSet()
                            {
                                indicatorType = IndicatorType.Circle,
                                position = playerPosition,
                                scale = Vector2.one,
                                rotation = 0,
                                chargeDuration = patternImpl.IndicationDurationProperty.statusValue,
                                weaponDataSet = LightningWeaponDataSet,
                                weaponCount = 1,
                            },
                        });
                        patternImpl.AttackCoolDownTimerProperty.statusValue = patternImpl.AttackCoolDownProperty.statusValue;
                    }
                }
                else
                {
                    patternImpl.AttackCoolDownTimerProperty.statusValue -= Time.deltaTime;
                }
            }
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();

            foreach(var data in indicatedAttackManageImplement.instantiatedIndicatedAttackDatas)
            {
                if (entityContainer.GetEntity<EnemyAttackIndicatorEntity>(data.statusValue.indicatorEgid, out var enemyCircleAttackIndicatorEntity))
                {
                    indicatorFactory.EnqueRecycle(enemyCircleAttackIndicatorEntity, enemyCircleAttackIndicatorEntity.SrcPathHashCode);
                }
            }
            indicatedAttackManageImplement.instantiatedIndicatedAttackDatas.Clear();
        }

        private static WeaponDataSet LightningWeaponDataSet => new WeaponDataSet()
        {
            id = string.Empty,
            level = 1,
            damage = 1,
            coolTime = 0,
            isSingleCreation = false,
            isSummonCreation = false,
            prefabPath = $"Prefabs/Projectile/EnemyLightningWand",
            createCount = 1,
            projectileSpeed = 2.5f,
            multiCreationTick = 0,
            criticalRatio = 0,
            criticalProbability = 0,
            spearCount = 0,
            knockBack = 0,
            attackType = AttackType.PHYSICS,
            area = 1,
        };
    }
}
