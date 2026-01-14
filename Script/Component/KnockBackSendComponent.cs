using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum KnockBackDirection
    {
        RigidBodyVelocity,
        OppositeOfPlayer,
        OppositeOfAttack
    }

    [System.Serializable]
    public struct KnockBackSendDataSet
    {
        public KnockBackDirection direction;
        public float distance;

        public KnockBackReceiveDataSet CalcKnockBackReceiveDataSet(Vector2 playerPos, Vector2 attackPos, Vector2 attackVelocity, Vector2 enemyPos)
        {
            Vector2 directionVector = direction switch
            {
                KnockBackDirection.RigidBodyVelocity => attackVelocity,
                KnockBackDirection.OppositeOfPlayer => enemyPos - playerPos,
                KnockBackDirection.OppositeOfAttack => enemyPos - attackPos,
                _ => attackVelocity
            };
            directionVector.Normalize();

            float velocity = GameSettings.DefaultKnockBackVelocity;
            float duration = (velocity != 0)? distance / velocity : GameSettings.DefaultKnockBackDuration;
            return new KnockBackReceiveDataSet() { isKnockBacking = true, direction = directionVector, remainTime = duration, velocity = GameSettings.DefaultKnockBackVelocity };
        }
    }

    public interface IKnockBackSendData : IStatusValue<KnockBackSendDataSet> { }

    [Serializable]
    public struct KnockBackSendData : IKnockBackSendData
    {
        [SerializeField] private KnockBackSendDataSet value;
        public KnockBackSendDataSet statusValue { get => value; set => this.value = value; }
    }
}
