using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IIsActivating : IStatusValue<bool> { }
    public interface IActivationDuration : IStatusValue<float> { }
    public interface IActivationDurationTimer : IStatusValue<float> { }
    public interface ICooldownDuration : IStatusValue<float> { }
    public interface ICooldownDurationTimer : IStatusValue<float> { }


    [System.Serializable]
    public struct IsActivating : IIsActivating
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct ActivationDuration : IActivationDuration
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct ActivationDurationTimer : IActivationDurationTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct CooldownDuration : ICooldownDuration
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct CooldownDurationTimer : ICooldownDurationTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    public class InteractableColliderImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private IsActivating isActivating;
        [SerializeField] private ActivationDuration activationDuration;
        private ActivationDurationTimer activationDurationTimer;
        [SerializeField] private CooldownDuration coolDownDuration;
        private CooldownDurationTimer coolDownDurationTimer;

        public ref IsActivating IsActivatingProperty => ref isActivating;
        public ref ActivationDuration ActivationDurationProperty => ref activationDuration;
        public ref ActivationDurationTimer ActivationDurationTimerProperty => ref activationDurationTimer;
        public ref CooldownDuration CoolDownDurationProperty => ref coolDownDuration;
        public ref CooldownDurationTimer CoolDownDurationTimerProperty => ref coolDownDurationTimer;

        public void InitializeComponent()
        {
            isActivating.statusValue = false;
            activationDurationTimer.statusValue = 0f;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out PlayerCharacterEntity player))
            {
                isActivating.statusValue = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out PlayerCharacterEntity player))
            {
                isActivating.statusValue = false;
            }
        }
    }
}
