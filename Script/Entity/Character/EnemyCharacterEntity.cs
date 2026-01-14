using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class EnemyCharacterEntity : CharacterEntity
    {
        public MonsterTypeImplement monsterTypeImplement;
        public MonsterMaxHitpointImplement monsterMaxHitPointImplement;
        public MonsterDamageBaseValueImplement monsterDamageBaseValueImplement;
        public TableIndexDataImplement tableIndexDataImplement;
        public ApplyBuffImplement applyBuffImplement;
        public EnemyMeleeAttackImplement enemyMeleeAttackImplement;
        public WeaponInventoryImplement weaponInventoryImplement;
        public HitPointRelatedScaleImplement hitPointRelatedScaleImplement;

        public OnKilledEventImplement onKilledEventImplement;
        [Inject] protected IEnemyMeleeAttackSystem ememyMeleeAttackSystem;
        [Inject] protected ICandidateRemoveEntitySystem candidateRemoveEntitySystem;

        public WeaponFactory weaponFactory;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
            if (TryGetComponent(out applyBuffImplement))
            {
                Components.Add(applyBuffImplement);
            }
            if (TryGetComponent(out enemyMeleeAttackImplement))
            {
                Components.Add(enemyMeleeAttackImplement);
            }
            if (TryGetComponent(out weaponInventoryImplement))
            {
                Components.Add(weaponInventoryImplement);
            }
            if (TryGetComponent(out hitPointRelatedScaleImplement))
            {
                Components.Add(hitPointRelatedScaleImplement);
            }
            if (TryGetComponent(out onKilledEventImplement))
            {
                Components.Add(onKilledEventImplement);
            }

            if (TryGetComponent(out monsterTypeImplement))
            {
                Components.Add(monsterTypeImplement);
            }

            if (TryGetComponent(out monsterMaxHitPointImplement))
            {
                Components.Add(monsterMaxHitPointImplement);
            }

            if (TryGetComponent(out monsterDamageBaseValueImplement))
            {
                Components.Add(monsterDamageBaseValueImplement);
            }

            ememyMeleeAttackSystem.RegistComponent(enemyMeleeAttackImplement, this);
            candidateRemoveEntitySystem.RegistComponent(null, this);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            ememyMeleeAttackSystem.UnRegistComponent(this);
            candidateRemoveEntitySystem.UnRegistComponent(this);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            statusImplement.characterTypeProperty.statusValue = CharacterTypes.Enemy;
            statusImplement.hitPointProperty.statusValue = 100;
            characterDeathImplement.DeathStateProperty.statusValue = DeathState.Living;
            characterDeathImplement.DyingDurationTimerProperty.statusValue = 0f;
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimation(characterDeathImplement, animationImplement, translateImplement);
            UpdatUnitDirection(translateImplement, statusImplement);
            if (weaponInventoryImplement != null)
                WeaponCoolDown(weaponInventoryImplement, weaponFactory);
            if(hitPointRelatedScaleImplement != null)
                UpdateHitPointRelatedScale(monsterMaxHitPointImplement, hitPointRelatedScaleImplement, statusImplement, transformImplement);
        }

        private static void UpdateAnimation(CharacterDeathImplement characterDeathImpl, AnimationImplement animationImpl, TranslateImplement translateImpl)
        {
            if (characterDeathImpl.DeathStateProperty.statusValue != DeathState.Living)
                return;

            animationImpl.animationStateProperty.statusValue = translateImpl.moveDirectionProperty.statusValue.SqrMagnitude() > 0 ? AnimationHashs.Walk : AnimationHashs.Idle;
        }

        private static void UpdatUnitDirection(TranslateImplement translateImpl, CommonStatusImplement statusImpl){

            var normalizeValue = translateImpl.moveDirectionProperty.statusValue.normalized;
            var dot = normalizeValue.x;
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            var div = (int)(angle / 22.5f);
            if (normalizeValue.y < 0f)
            {
                switch (div)
                {
                    case 0:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CR;
                        break;
                    case 1:
                    case 2:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DR;
                        break;
                    case 3:
                    case 4:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DC;
                        break;
                    case 5:
                    case 6:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DL;
                        break;
                    case 7:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CL;
                        break;
                }
            }
            else
            {
                switch (div)
                {
                    case 0:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CR;
                        break;
                    case 1:
                    case 2:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UR;
                        break;
                    case 3:
                    case 4:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UC;
                        break;
                    case 5:
                    case 6:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UL;
                        break;
                    case 7:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CL;
                        break;
                }
            }
        }

        private void WeaponCoolDown(WeaponInventoryImplement weaponInventoryImpl, GameObjectFactory factory)
        {
            weaponInventoryImpl.CheckWeaponCooldown(1);
            weaponInventoryImpl.FireWeapon(out var list);
            foreach (var d in list)
            {
                if (string.IsNullOrEmpty(d.prefabPath))
                {
                    continue;
                }

                if (d.isSummonCreation == false)
                {
                    var createCount = d.createCount;
                    if (createCount > 1)
                    {
                        CreateWeapon(createCount, factory, d, CharacterTypes.Enemy).Forget();
                    }
                    else
                    {
                        CreateWeapon(factory, d, 1, 1, CharacterTypes.Enemy);
                    }
                }
                else
                {

                }
            }
        }

        private static void UpdateHitPointRelatedScale(MonsterMaxHitpointImplement maxHitpointImpl, HitPointRelatedScaleImplement hitPointRelatedScaleImplement, CommonStatusImplement statusImpl, Transform transformImplement)
        {
            var scaleOnZero = hitPointRelatedScaleImplement.ScaleOnZeroHitPointProperty.statusValue;
            var scaleOnMax = hitPointRelatedScaleImplement.ScaleOnMaxHitPointProperty.statusValue;
            var maxHitPoint = maxHitpointImpl.maxHitPointProperty.statusValue;
            var scale = Vector2.Lerp(scaleOnZero, scaleOnMax, (float)statusImpl.hitPointProperty.statusValue / maxHitPoint);
            transformImplement.localScale = scale;
    }
    }
}
