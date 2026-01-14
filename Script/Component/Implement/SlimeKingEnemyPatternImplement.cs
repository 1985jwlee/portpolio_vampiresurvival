using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class SlimeKingEnemyPatternImplement : MonoBehaviour, IComponent
    {
        public ParameterData parameter;
        public ExecutionContextData executionContext;

        public void InitializeComponent()
        {
            executionContext.statusValue = new ExecutionContext()
            {
                lastAttackKind = 1,
                attack1_shootingCount = 0,
            };
        }

        [Serializable]
        public struct Parameter
        {
            public float attack1_DamageMultiplier;
            public float attack1_ProjectileSpeed;
            public float attack1_ProjectileLifeTime;
            public int attack1_ProjectileNum;
            public float attack1_shootingInterval;

            public int attack2_SpawnNum;

            public float moving_DurationMin;
            public float moving_DurationMax;
        }

        [Serializable]
        public struct ExecutionContext
        {
            public int attack1_shootingCount;
            public int lastAttackKind;
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
