using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum IndicatorType
    {
        Circle,
        Fan,
        Rect,
    }

    public struct IndicatedAttackDataSet
    {
        public Vector2 position;
        public Vector2 scale;
        public IndicatorType indicatorType;
        public float rotation;
        public float chargeDuration;
        public WeaponDataSet weaponDataSet;
        public int weaponCount;
    }

    public struct InstantiatedIndicatedAttackDataSet
    {
        public uint indicatorEgid;
        public float indicatorChargeTimer;
        public float indicatorChargeDuration;
        public WeaponDataSet weaponDataSet;
        public int weaponCount;
        public float angleBetweenWeapon;
        public Vector2 fillInitialScale;
        public Vector2 fillInitialOffset;
        public Vector2 fillScalePerAlpha;
        public Vector2 fillOffsetPerAlpha;
    }

    public interface IIndicatedAttackData : IStatusValue<IndicatedAttackDataSet> { }
    public interface IIndicatedAttackDurationTimer : IStatusValue<float> { }

    public interface IInstantiatedIndicatedAttackData : IStatusValue<InstantiatedIndicatedAttackDataSet>
    {
        bool shouldDelete { get; set; }
    }

    [System.Serializable]
    public struct IndicatedAttackData : IIndicatedAttackData
    {
        [SerializeField] private IndicatedAttackDataSet value;
        public IndicatedAttackDataSet statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct InstantiatedIndicatedAttackData : IInstantiatedIndicatedAttackData
    {
        [SerializeField] private InstantiatedIndicatedAttackDataSet value;
        public InstantiatedIndicatedAttackDataSet statusValue { get => value; set => this.value = value; }
        public bool shouldDelete { get; set; }
    }

    [System.Serializable]
    public struct IndicatedAttackDurationTimer : IIndicatedAttackDurationTimer
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    public class IndicatedAttackManageImplement : MonoBehaviour, IComponent
    {
        public List<IndicatedAttackData> indicatedAttackDatas;
        public List<InstantiatedIndicatedAttackData> instantiatedIndicatedAttackDatas;

        private IndicatedAttackDurationTimer indicatedAttackDurationTimer;

        public ref IndicatedAttackDurationTimer IndicatedAttackDurationTimerProperty => ref indicatedAttackDurationTimer;

        public void InitializeComponent()
        {
        }
    }
}
