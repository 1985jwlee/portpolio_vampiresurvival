using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum DeathState
    {
        Living,
        Dying,
        Dead
    }

    public interface ICharacterDeathState : IStatusValue<DeathState> { }
    public interface IDyingDuration : IStatusValue<float> { }
    public interface IDyingDurationTimer : IStatusValue<float> { }


    [System.Serializable]
    public struct CharacterDeathState : ICharacterDeathState
    {
        [SerializeField] private DeathState value;
        public DeathState statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct DyingDuration : IDyingDuration
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct DyingDurationTimer : IDyingDurationTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }


    public class CharacterDeathImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private DyingDuration dyingDuration;

        private CharacterDeathState deathState;
        private DyingDurationTimer dyingDurationTimer;

        public ref DyingDuration DyingDurationProperty => ref dyingDuration;

        public ref CharacterDeathState DeathStateProperty => ref deathState;
        public ref DyingDurationTimer DyingDurationTimerProperty => ref dyingDurationTimer;

        public void InitializeComponent()
        {
        }
    }
}
