using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum BuffType
    {
        Damage, Heal, Stun, ExpUp, Magnet, DeviceBox, GoldUp, Bomb, GoldFever, GoldFeverTimeUp,
        Gigantic, Micrify,
        CriticalDamage, MagicDamage, DeathPenalty,
        AttackArea,
        DamageReduction, CounterAttack, MaxHealthIncrement, 
        CooldownReduction, ProjectileIncrement, PickupAreaIncrement, 
        MagicDamageIncrement, CriticalChanceIncrement, ExperienceIncrement, 
        MoveSpeedIncrement, HealthRegenIncrement, ProjectileSpeedIncrement, AttackableLifeTime, GetMoreGold,
    }
    
    [System.Serializable]
    public struct BuffData
    {
        public string buffDeviceId;
        public uint rootEntityId;
        public CharacterType rootCharacter;
        public BuffType buffType;
        public float remainTime;
        public float buffValue;
    }
    
    public interface IBuff : IStatusValue<BuffData>
    {
        bool markExecuted { get; set; }
    }

    [System.Serializable]
    public struct Buff : IBuff
    {
        [SerializeField] private BuffData value;
        public BuffData statusValue { get => value; set => this.value = value; }
        public bool markExecuted { get; set; }
    }
}
