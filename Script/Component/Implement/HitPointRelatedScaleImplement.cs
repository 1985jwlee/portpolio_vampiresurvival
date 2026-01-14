using System;
using UnityEngine;

namespace Game.ECS
{
    public interface IScaleOnMaxHitPoint : IStatusValue<Vector2> { }
    public interface IScaleOnZeroHitPoint : IStatusValue<Vector2> { }

    [Serializable]
    public struct ScaleOnMaxHitPoint : IScaleOnMaxHitPoint
    {
        [SerializeField] public Vector2 value;
        public Vector2 statusValue { get => value; set => value = this.value; }
    }

    [Serializable]
    public struct ScaleOnZeroHitPoint : IScaleOnZeroHitPoint
    {
        [SerializeField] public Vector2 value;
        public Vector2 statusValue { get => value; set => value = this.value; }
    }

    public class HitPointRelatedScaleImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private ScaleOnMaxHitPoint scaleOnMaxHitPoint;
        [SerializeField] private ScaleOnZeroHitPoint scaleOnZeroHitPoint;

        public ref ScaleOnMaxHitPoint ScaleOnMaxHitPointProperty => ref scaleOnMaxHitPoint;
        public ref ScaleOnZeroHitPoint ScaleOnZeroHitPointProperty => ref scaleOnZeroHitPoint;

        public void InitializeComponent()
        {

        }
    }
}
