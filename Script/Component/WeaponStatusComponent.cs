using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    [Serializable]
    public struct WeaponDataSet
    {
        public string id;
        public string prefabPath;
        public int level;
        public int damage;
        public float coolDown;
        public float coolTime;
        public bool isCooldownCharging;
        /// <summary>
        /// 하나만 생성해야하는 웨폰인지
        /// </summary>
        public bool isSingleCreation;

        /// <summary>
        /// 소환체를 생성해야하는 웨폰인지
        /// </summary>
        public bool isSummonCreation;

        /// <summary>
        /// 소환체에 의해 생성될 무기Id
        /// </summary>
        public string summonAttackDeviceId;
        
        /// <summary>
        /// 하나만 생성해야하는 웨폰이 생성되었는지
        /// </summary>
        public bool markSingleCreation;

        public int createCount;
        public float multiCreationTick;
        public float projectileSpeed;
        public float countMultiCreationCooltime;

        /// <summary>
        /// 크리티컬 추가치
        /// </summary>
        public float criticalRatio;
        /// <summary>
        /// 크리티컬 확률
        /// </summary>
        public float criticalProbability;
        public int spearCount;

        public float knockBack;
        public AttackType attackType;

        public bool mustHaveTarget;
        public float area;
        public bool isGroupAttack;
        public int groupCount;
        public float lifeDuration;
    }

    public interface IWeaponDataSet : IStatusValue<WeaponDataSet>
    {
    }

    [Serializable]
    public struct WeaponData : IWeaponDataSet
    {
        [SerializeField] private WeaponDataSet value;
        public WeaponDataSet statusValue { get => value; set => this.value = value; }
    }


}
