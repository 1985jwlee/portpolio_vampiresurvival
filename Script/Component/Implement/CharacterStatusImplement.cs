using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class CharacterStatusImplement : MonoBehaviour, IComponent
    {
        #region SrcStatus
        [SerializeField] private MaxHitPoint maxHitPoint;
        [SerializeField] private HitpointRecoveryRate hitpointRecoveryRate;
        [SerializeField] private SpeedRatio speedRatio;
        [SerializeField] private ProjectileSpeedRatio projectileSpeedRatio;
        [SerializeField] private AttackSizeRatio attackSizeRatio;
        [SerializeField] private FastCooldownRatio cooldownRatio;
        [SerializeField] private AdditionalProjectileCount addProjCount;
        [SerializeField] private AdditionalCriticalRatio addCriticalRatio;
        [SerializeField] private AdditionalMagicDamageRatio addMagicRatio;
        [SerializeField] private DamageReductionRatio damageReductionRatio;
        [SerializeField] private CounterAttackMultiple counterAttackMultiple;
        [SerializeField] private PickupAreaSizeMultiple pickupAreaSizeMultiple;
        [SerializeField] private ExperienceMultiple expMultiple;
        [SerializeField] private AttackableLifeTimeMultiple attackableLifeTimeMultiple;
        [SerializeField] private GetGoldMultiple getGoldMultiple;
        
        public ref MaxHitPoint maxHitPointProperty => ref maxHitPoint;
        public ref HitpointRecoveryRate hitpointRecoveryRateProperty => ref hitpointRecoveryRate;
        public ref SpeedRatio speedRatioProperty => ref speedRatio;
        public ref ProjectileSpeedRatio projectileSpeedRatioProperty => ref projectileSpeedRatio;
        public ref AttackSizeRatio attackSizeRatioProperty => ref attackSizeRatio;
        public ref FastCooldownRatio cooldownRatioProperty => ref cooldownRatio;
        public ref AdditionalProjectileCount addProjCountProperty => ref addProjCount;
        public ref AdditionalCriticalRatio addCriticalRatioProperty => ref addCriticalRatio;
        public ref AdditionalMagicDamageRatio addMagicRatioProperty => ref addMagicRatio;
        public ref DamageReductionRatio damageReductionRatioProperty => ref damageReductionRatio;
        public ref CounterAttackMultiple counterAttackMultipleProperty => ref counterAttackMultiple;
        public ref PickupAreaSizeMultiple pickAreaSizeMultipleProperty => ref pickupAreaSizeMultiple;
        public ref ExperienceMultiple expMultipleProperty => ref expMultiple;

        public ref AttackableLifeTimeMultiple attackableLifeTimeMultipleProperty => ref attackableLifeTimeMultiple;
        public ref GetGoldMultiple getGoldMultipleProperty => ref getGoldMultiple;

#endregion

#region DstBuffedStatus
        [SerializeField] private MaxHitPoint buffedmaxHitPoint;
        [SerializeField] private HitpointRecoveryRate buffedhitpointRecoveryRate;
        [SerializeField] private SpeedRatio buffedspeedRatio;
        [SerializeField] private ProjectileSpeedRatio buffedprojectileSpeedRatio;
        [SerializeField] private AttackSizeRatio buffedattackSizeRatio;
        [SerializeField] private FastCooldownRatio buffedcooldownRatio;
        [SerializeField] private AdditionalProjectileCount buffedaddProjCount;
        [SerializeField] private AdditionalCriticalRatio buffedaddCriticalRatio;
        [SerializeField] private AdditionalMagicDamageRatio buffedaddMagicRatio;
        [SerializeField] private DamageReductionRatio buffeddamageReductionRatio;
        [SerializeField] private CounterAttackMultiple buffedcounterAttackMultiple;
        [SerializeField] private PickupAreaSizeMultiple buffedpickupAreaSizeMultiple;
        [SerializeField] private ExperienceMultiple buffedexpMultiple;
        [SerializeField] private AttackableLifeTimeMultiple buffedAttackableLifeTimeMultiple;
        [SerializeField] private GetGoldMultiple buffedGetGoldMultiple;
        public ref MaxHitPoint buffedmaxHitPointProperty => ref buffedmaxHitPoint;
        public ref HitpointRecoveryRate buffedhitpointRecoveryRateProperty => ref buffedhitpointRecoveryRate;
        public ref SpeedRatio buffedspeedRatioProperty => ref buffedspeedRatio;
        public ref ProjectileSpeedRatio buffedprojectileSpeedRatioProperty => ref buffedprojectileSpeedRatio;
        public ref AttackSizeRatio buffedattackSizeRatioProperty => ref buffedattackSizeRatio;
        public ref FastCooldownRatio buffedcooldownRatioProperty => ref buffedcooldownRatio;
        public ref AdditionalProjectileCount buffedaddProjCountProperty => ref buffedaddProjCount;
        public ref AdditionalCriticalRatio buffedaddCriticalRatioProperty => ref buffedaddCriticalRatio;
        public ref AdditionalMagicDamageRatio buffedaddMagicRatioProperty => ref buffedaddMagicRatio;
        public ref DamageReductionRatio buffeddamageReductionRatioProperty => ref buffeddamageReductionRatio;
        public ref CounterAttackMultiple buffedcounterAttackMultipleProperty => ref buffedcounterAttackMultiple;
        public ref PickupAreaSizeMultiple buffedpickAreaSizeMultipleProperty => ref buffedpickupAreaSizeMultiple;
        public ref ExperienceMultiple buffedexpMultipleProperty => ref buffedexpMultiple;
        
        public ref AttackableLifeTimeMultiple buffedAttackableLifeTimeMultipleProperty => ref attackableLifeTimeMultiple;
        public ref GetGoldMultiple buffedGetGoldMultipleProperty => ref buffedGetGoldMultiple;
#endregion

        public void InitializeComponent()
        {
            
        }
    }
}
