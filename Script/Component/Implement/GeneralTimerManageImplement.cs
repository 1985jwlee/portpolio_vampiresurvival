using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum GeneralTimerState
    {
        Waiting,
        Counting,
        Finish,
    }

    [System.Serializable]
    public struct GeneralTimerSettingData
    {
        public float duration;
    }

    public struct GeneralTimerStatusData
    {
        public GeneralTimerState timerState;
        public float timer;
    }

    public interface IGeneralTimerSetting : IStatusValue<GeneralTimerSettingData> { };
    public interface IGeneralTimerStatus : IStatusValue<GeneralTimerStatusData> { };

    [System.Serializable]
    public struct GeneralTimerStatus : IGeneralTimerStatus
    {
        [SerializeField] private GeneralTimerStatusData value;
        public GeneralTimerStatusData statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct GeneralTimerSetting : IGeneralTimerSetting
    {
        [SerializeField] private GeneralTimerSettingData value;
        public GeneralTimerSettingData statusValue { get => value; set => this.value = value; }
    }

    public class GeneralTimerManageImplement : MonoBehaviour, IComponent
    {
        public List<GeneralTimerSetting> timerSettings;
        public List<GeneralTimerStatus> timerStatuses;

        public void InitializeComponent()
        {
            if (timerStatuses != null)
                timerStatuses.Clear();
            else
                timerStatuses = new List<GeneralTimerStatus>();

            for (int i = 0; i < timerSettings.Count; i++)
                timerStatuses.Add(new GeneralTimerStatus() { statusValue = new GeneralTimerStatusData() });
        }
    }
}
