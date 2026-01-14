using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class RelicGolemEnemyPatternImplement : MonoBehaviour, IComponent
    {
        public ParameterData parameter;
        public ExecutionContextData executionContext;

        public void InitializeComponent()
        {

        }

        [Serializable]
        public struct Parameter
        {
            public float coolTime;

            public float nearPlayerDistance;
            [Header("Probability")]
            public float attack1Probability;
            public float attack2Probability;
            public float attack3Probability;

            public float nearPlayerAttack1Probability;
            public float nearPlayerAttack2Probability;
            public float nearPlayerAttack3Probability;

            [Header("Attack1")]
            public float attack1_Range;
            public float attack1_impactDelay;
            public float attack1_DamageMultiplier;

            [Header("Attack2")]
            public float attack2_interval;
            public int attack2_projectileNum;
            public float attack2_angleRange;
            public float attack2_DamageMultiplier;
            public float attack2_ProjectileSpeed;
            public float attack2_ProjectileLifeTime;

            [Header("Attack3")]
            public float attack3_interval;
            public int attack3_projectileNum;
            public float attack3_DamageMultiplier;
            public float attack3_ProjectileSpeed;
            public float attack3_ProjectileLifeTime;

            [Header("Attack4")]
            public float attack4_interval;
            public float attack4_angleSpeed;
            public float attack4_DamageMultiplier;
            public float attack4_ProjectileSpeed;
            public float attack4_ProjectileLifeTime;
        }

        [Serializable]
        public struct ExecutionContext
        {
            public bool attack1_isImpacted;

            public int attack2_shootingCounter;

            public int attack3_shootingCounter;

            public int attack4_shootingCounter;
            public float attack4_angle;
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
