using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum AmbushState
    {
        None,
        Waiting,
        Chasing,
    }

    public interface IAmbushStateValue : IStatusValue<AmbushState> { };

    [Serializable]
    public struct AmbushStateValue : IAmbushStateValue
    {
        [SerializeField] private AmbushState value;
        public AmbushState statusValue { get => value; set => this.value = value; }
    }

    public class AmbushEnemyPatternImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private DetectionRadius detectionRadius;
        private AmbushStateValue state;

        public ref DetectionRadius DetectionRadiusProperty => ref detectionRadius;
        public ref AmbushStateValue StateProperty => ref state;

        public void InitializeComponent()
        {
            state.statusValue = AmbushState.Waiting;
        }
    }
}
