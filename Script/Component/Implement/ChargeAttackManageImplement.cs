using System;
using UnityEngine;

namespace Game.ECS
{
    public enum ChargeState
    {
        Waiting,
        Start,
        Stamping,
        Charging,
        Finish,
    }

    public interface IChargeVelocity : IStatusValue<float> { };
    public interface IChargeDirection : IStatusValue<Vector2> { };
    public interface IHasChargeBlaze : IStatusValue<bool> { };
    public interface IChargeBlazeInterval : IStatusValue<float> { };
    public interface IChargeBlazeCounter : IStatusValue<int> { };

    [Serializable]
    public struct ChargeVelocity : IChargeVelocity
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ChargeDirection : IChargeDirection
    {
        [SerializeField] private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct HasChargeBlaze : IHasChargeBlaze
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ChargeBlazeInterval : IChargeBlazeInterval
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct ChargeBlazeCounter : IChargeBlazeCounter
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    public class ChargeAttackManageImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private ChargeVelocity chargeVelocity;
        [SerializeField] private HasChargeBlaze hasChargeBlaze;
        [SerializeField] private ChargeBlazeInterval chargeBlazeInterval;

        private ChargeDirection chargeDirection;
        private ChargeBlazeCounter chargeBlazeCounter;

        public ref ChargeVelocity ChargeVelocityProperty => ref chargeVelocity;
        public ref ChargeDirection ChargeDirectionProperty => ref chargeDirection;
        public ref HasChargeBlaze HasChargeBlazeProperty => ref hasChargeBlaze;
        public ref ChargeBlazeInterval ChargeBlazeIntervalProperty => ref chargeBlazeInterval;
        public ref ChargeBlazeCounter ChargeBlazeCounterProperty => ref chargeBlazeCounter;

        public void InitializeComponent()
        {
            chargeDirection.statusValue = Vector2.zero;
            chargeBlazeCounter.statusValue = 0;
        }
    }
}
