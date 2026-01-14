using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    [System.Serializable]
    public struct KnockBackReceiveDataSet
    {
        public bool isKnockBacking;
        public Vector2 direction;
        public float velocity;
        public float remainTime;
    }

    public interface IKnockBackReceiveData : IStatusValue<KnockBackReceiveDataSet> { }

    [Serializable]
    public struct KnockBackReceiveData : IKnockBackReceiveData
    {
        [SerializeField] private KnockBackReceiveDataSet value;
        public KnockBackReceiveDataSet statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct KnockBackResistData : IStatusValue<bool>
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }
}
