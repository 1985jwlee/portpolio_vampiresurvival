using System;
using UnityEngine;

namespace Game.ECS
{
    public enum SuicideBombingState
    {
        None,
        Idle,
        Charging,
        Explode,
    }

    public interface IDetectionRadius : IStatusValue<float> { };
    public interface IExplosionRadius : IStatusValue<float> { };
    public interface IExplosionDelay : IStatusValue<float> { };
    public interface IExplosionDelayFill : IStatusValue<float> { };
    public interface ISuicideBombingStateValue : IStatusValue<SuicideBombingState> { };

    [Serializable]
    public struct DetectionRadius : IDetectionRadius
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ExplosionRadius : IExplosionRadius
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ExplosionDelay : IExplosionDelay
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ExplosionDelayFill : IExplosionDelayFill
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct SuicideBombingStateValue : ISuicideBombingStateValue
    {
        [SerializeField] private SuicideBombingState value;
        public SuicideBombingState statusValue { get => value; set => this.value = value; }
    }

    public class SuicideBombingEnemyPatternImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private DetectionRadius detectionRadius;
        [SerializeField] private ExplosionRadius explosionRadius;
        [SerializeField] private ExplosionDelay explosionDelay;
        private ExplosionDelayFill explosionDelayFill;
        private SuicideBombingStateValue state;

        public ref DetectionRadius DetectionRadiusProperty => ref detectionRadius;
        public ref ExplosionRadius ExplosionRadiusProperty => ref explosionRadius;
        public ref ExplosionDelay ExplosionDelayProperty => ref explosionDelay;
        public ref ExplosionDelayFill ExplosionDelayFillProperty => ref explosionDelayFill;
        public ref SuicideBombingStateValue StateProperty => ref state;

        public void InitializeComponent()
        {
            state.statusValue = SuicideBombingState.Idle;
            explosionDelayFill.statusValue = 0;
        }
    }
}
