using System;
using UnityEngine;

namespace Game.ECS
{
    public interface IIsDead : IStatusValue<bool> { }
    public interface IIsFinalBoss : IStatusValue<bool> { }
    public interface IBossIconPosition : IStatusValue<Vector2> { }

    [Serializable]
    public struct IsDead : IIsDead
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct IsFinalBoss : IIsFinalBoss
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct BossIconPosition : IBossIconPosition
    {
        [SerializeField] private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }
}
