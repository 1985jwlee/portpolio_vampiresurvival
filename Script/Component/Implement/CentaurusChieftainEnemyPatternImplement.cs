using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IAttackProbability : IStatusValue<float> { };
    public interface IDistance : IStatusValue<float> { };

    [Serializable]
    public struct AttackProbability : IAttackProbability
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct Distance : IDistance
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    public class CentaurusChieftainEnemyPatternImplement : MonoBehaviour, IComponent
    {
        public ParameterData parameter;
        public ExecutionContextData executionContext;

        [SerializeField] private AttackProbability fanwiseShootingProbability;
        [SerializeField] private AttackProbability chargeProbability;
        [SerializeField] private AttackCoolDown coolDownMin;
        [SerializeField] private AttackCoolDown coolDownMax;
        [SerializeField] private Distance distanceMinToMaintain;
        [SerializeField] private Distance distanceMaxToMaintain;

        private AttackCoolDownTimer coolDownTimer;

        public ref AttackProbability FanwiseShootingProbabilityProperty => ref fanwiseShootingProbability;
        public ref AttackProbability ChargeProbabilityProperty => ref chargeProbability;
        public ref AttackCoolDown CoolDownMinProperty => ref coolDownMin;
        public ref AttackCoolDown CoolDownMaxProperty => ref coolDownMax;
        public ref Distance DistanceMinToMaintainProperty => ref distanceMinToMaintain;
        public ref Distance DistanceMaxToMaintainProperty => ref distanceMaxToMaintain;
        public ref AttackCoolDownTimer CoolDownTimerProperty => ref coolDownTimer;

        public void InitializeComponent()
        {
        }

        [Serializable]
        public struct Parameter
        {
            public float attack1_DamageMultiplier;
            public float attack1_ProjectileSpeed;
            public float attack1_ProjectileLifeTime;

            public float attack2_DamageMultiplier;
            public float attack2_ProjectileLifeTime;
        }

        [Serializable]
        public struct ExecutionContext
        {
        }

        [Serializable]
        public struct ParameterData : IStatusValue<Parameter>
        {
            [SerializeField] private Parameter value;
            public Parameter statusValue { get => value; set => this.value = value; }
        }

        [Serializable]
        public struct ExecutionContextData : IStatusValue<ExecutionContext>
        {
            [SerializeField] private ExecutionContext value;
            public ExecutionContext statusValue { get => value; set => this.value = value; }
        }
    }
}
