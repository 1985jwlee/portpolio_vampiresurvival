using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IAttackCoolDown : IStatusValue<float> { }
    public interface IAttackCoolDownTimer : IStatusValue<float> { }
    public interface IIndicationDuration : IStatusValue<float> { }

    [System.Serializable]
    public struct AttackCoolDown : IAttackCoolDown
    {
        [SerializeField] public float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct AttackCoolDownTimer : IAttackCoolDown
    {
        [SerializeField] public float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct IndicationDuration : IIndicationDuration
    {
        [SerializeField] public float value;
        public float statusValue { get => value; set => this.value = value; }
    }


    public class IndicatedAttackEnemyPatternImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private DetectionRadius detectionRadius;
        [SerializeField] private AttackCoolDown attackCoolDown;
        [SerializeField] private IndicationDuration indicationDuration;

        private AttackCoolDownTimer attackCoolDownTimer;

        public ref DetectionRadius DetectionRadiusProperty => ref detectionRadius;
        public ref AttackCoolDown AttackCoolDownProperty => ref attackCoolDown;
        public ref AttackCoolDownTimer AttackCoolDownTimerProperty => ref attackCoolDownTimer;
        public ref IndicationDuration IndicationDurationProperty => ref indicationDuration;

        public void InitializeComponent()
        {
        }
    }
}
