using System;
using UnityEngine;

namespace Game.ECS
{
    public interface IHitTintColorData : IStatusValue<Color> { }

    [Serializable]
    public struct HitTintColorData : IHitTintColorData
    {
        [SerializeField] private Color value;
        public Color statusValue { get => value; set => this.value = value; }
    }

    public interface IHitTintTimeData : IStatusValue<float> { }

    [Serializable]
    public struct HitTintTimeData : IHitTintTimeData
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    public interface IHitTintTriggerData : IStatusValue<bool> { }

    [Serializable]
    public struct HitTintTriggerData : IHitTintTriggerData
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }
}
