using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IMoveDuration : IStatusValue<float> { }
    public interface IWaitDuration : IStatusValue<float> { }
    public interface IMoveTimer : IStatusValue<float> { }
    public interface IWaitTimer : IStatusValue<float> { }


    [System.Serializable]
    public struct MoveDuration : IMoveDuration
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct WaitDuration : IWaitDuration
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct MoveTimer : IMoveTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct WaitTimer : IWaitTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    public class RandomMovementImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private MoveDuration moveDuration;
        [SerializeField] private WaitDuration waitDuration;
        private MoveTimer moveTimer;
        private WaitTimer waitTimer;
        private MoveDirection moveDirection;

        public ref MoveDuration MoveDurationProperty => ref moveDuration;
        public ref WaitDuration WaitDurationProperty => ref waitDuration;
        public ref MoveTimer MoveTimerProperty => ref moveTimer;
        public ref WaitTimer WaitTimerProperty => ref waitTimer;
        public ref MoveDirection MoveDirectionProperty => ref moveDirection;

        public void InitializeComponent()
        {
            MoveDirectionProperty.statusValue = UnityEngine.Random.insideUnitCircle;
            MoveTimerProperty.statusValue = MoveDurationProperty.statusValue;
            WaitTimerProperty.statusValue = WaitDurationProperty.statusValue;
        }
    }
}
