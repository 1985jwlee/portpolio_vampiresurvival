using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IMaxHitpoint : IStatusValue<int>{}
    public interface IHitpointRecoveryRate : IStatusValue<float> {}
    public interface ISpeedRatio : IStatusValue<float> {}
    public interface IProjectileSpeedRatio : IStatusValue<float> {}
    public interface IAttackSizeRatio : IStatusValue<float> {}
    public interface IFastCooldownRatio : IStatusValue<float> {}
    public interface IAdditionalProjectileCount : IStatusValue<int> {}
    public interface IAdditionalCriticalRatio : IStatusValue<float> {}
    public interface IAdditionalMagicDamageRatio : IStatusValue<float> {}
    public interface IDamageReductionRatio : IStatusValue<float> {}
    public interface ICounterAttackMultiple : IStatusValue<float> {}
    public interface IPickupAreaSizeMultiple : IStatusValue<float> {}
    public interface IExperienceMultiple : IStatusValue<float> {}
    public interface IAttackableLifeTimeMultiple : IStatusValue<float> {}
    public interface IGetGoldMultiple : IStatusValue<float> {}

    [System.Serializable]
    public struct MaxHitPoint : IMaxHitpoint
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct HitpointRecoveryRate : IHitpointRecoveryRate
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct SpeedRatio : ISpeedRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct ProjectileSpeedRatio : IProjectileSpeedRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct AttackSizeRatio : IAttackSizeRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct FastCooldownRatio : IFastCooldownRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct AdditionalProjectileCount : IAdditionalProjectileCount
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }
    
    
    
    [System.Serializable]
    public struct AdditionalCriticalRatio : IAdditionalCriticalRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct AdditionalMagicDamageRatio : IAdditionalMagicDamageRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct DamageReductionRatio : IDamageReductionRatio
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct CounterAttackMultiple : ICounterAttackMultiple
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct PickupAreaSizeMultiple : IPickupAreaSizeMultiple
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct ExperienceMultiple : IExperienceMultiple
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct AttackableLifeTimeMultiple : IAttackableLifeTimeMultiple
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct GetGoldMultiple : IGetGoldMultiple
    {
        [SerializeField] private float value;

        public float statusValue { get => value; set => this.value = value; }
    }

} 
