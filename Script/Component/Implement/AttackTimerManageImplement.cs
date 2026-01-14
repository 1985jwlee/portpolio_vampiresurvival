using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum AttackTimerState
    {
        Waiting,
        Start,
        Foreswing,
        StartImpact,
        Impacting,
        FinishImpact,
        Backswing,
        Finish,
    }

    [System.Serializable]
    public struct AttackTimerSettingData
    {
        public float foreswingDuration;
        public float impactDuration;
        public float backswingDuration;
    }

    public struct AttackTimerStatusData
    {
        public AttackTimerState attackTimerState;
        public float foreswingTimer;
        public float impactTimer;
        public float backswingTimer;
    }

    public interface IAttackTimerSetting : IStatusValue<AttackTimerSettingData> { };
    public interface IAttackTimerStatus : IStatusValue<AttackTimerStatusData> { };


    [System.Serializable]
    public struct AttackTimerSetting : IAttackTimerSetting
    {
        [SerializeField] private AttackTimerSettingData value;
        public AttackTimerSettingData statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct AttackTimerStatus : IAttackTimerStatus
    {
        [SerializeField] private AttackTimerStatusData value;
        public AttackTimerStatusData statusValue { get => value; set => this.value = value; }
    }

    public class AttackTimerManageImplement : MonoBehaviour, IComponent
    {
        public List<AttackTimerSetting> attackTimerSettings;
        public List<AttackTimerStatus> attackTimerStatuses;

        public void InitializeComponent()
        {
            if(attackTimerStatuses != null)
                attackTimerStatuses.Clear();
            else
                attackTimerStatuses = new List<AttackTimerStatus>();

            for (int i = 0; i < attackTimerSettings.Count; i++)
                attackTimerStatuses.Add(new AttackTimerStatus() { statusValue = new AttackTimerStatusData()});
        }
    }


}
