using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Scripts.Attributes;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UniRx;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Game.ECS
{
    public class DeviceBoxView : UiView
    {
        public Button okButton;
        public DeviceChoosableView[] devices;
        public PlayableDirector introPlayableDirector;
        public PlayableDirector devicePlayableDirector;
        public DeviceInventoryView deviceInventoryView;

        [Inject] DeviceInventoryManager inventoryManager;
        [Inject] IEntityContainer entityContainer;
        [Inject] TableDataHolder tableDataHolder;

        private CancellationTokenSource okButtonCancellationSource;

        public override void Init()
        {
            gameObject.SetActive(false);

            MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch (message)
                {
                    case "deviceBoxEvent":
                        entityContainer.playerCharacterEntity.mobileJoystickImplement.Pause();
                        gameObject.SetActive(true);
                        Time.timeScale = 0f;
                        StartEvent();
                        break;
                };
            }).AddTo(this);

            okButton.OnClickAsObservable().Subscribe(_ =>
            {
                AfterClickOkButton();
            }).AddTo(this);
        }

        private void OnDisable()
        {
            deviceInventoryView.gameObject.SetActive(false);
        }

        void AfterClickOkButton()
        {
            okButtonCancellationSource.Cancel();
            devicePlayableDirector.Stop();
            entityContainer.playerCharacterEntity.mobileJoystickImplement.Resume();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            MessageBroker.Default.Publish("timeStopEventDone");
        }

        private void Update()
        {
            if(okButton.interactable)
                if (Input.GetKeyDown(KeyCode.Space))
                    AfterClickOkButton();
        }

        private void StartEvent()
        {
            const float fiveDeviceThreshold = GameSettings.FiveDeviceProbability;
            const float threeDeviceThreshold = GameSettings.FiveDeviceProbability + GameSettings.ThreeDeviceProbability;

            int deviceNum = Random.value switch
            {
                < fiveDeviceThreshold => 5,
                < threeDeviceThreshold => 3,
                _ => 1,
            };

            var moneyAndPresentationTime = CalcRewardMoneyAndDuration();

            InitDevices(deviceNum);

            deviceInventoryView.InitSlots();

            StartAnimation(moneyAndPresentationTime.money, moneyAndPresentationTime.duration).Forget();
        }

        private void InitDevices(int deviceNum)
        {
            for (int i = 0; i < devices.Length; i++)
                devices[i].gameObject.SetActive(false);

            int length = Mathf.Min(devices.Length, deviceNum);
            for(int i = 0; i < length; i++)
            {
                devices[i].gameObject.SetActive(true);

                var list = inventoryManager.GetChoosableDeviceList(1, false, true);
                if (list.Count != 0)
                {
                    inventoryManager.AddOrUpdateArcaneDevice(list[0]);
                    devices[i].InitWithDevice(list[0]);
                }
                else
                {
                    MessageBroker.Default.Publish(new KeyValuePair<string, uint>("addGold", GameSettings.SubstituteGolds));
                    devices[i].InitWithExtraReward(DeviceChoosableView.ExtraReward.Gold, false);
                }
            }
            
        }

        private async UniTask StartAnimation(int rewardMoney, float moneyCountDuration)
        {
            okButtonCancellationSource = new CancellationTokenSource();
            okButton.interactable = false;

            introPlayableDirector.Play();
            var introDuration = introPlayableDirector.duration;
            var introDurationTask = UniTask.Delay(System.TimeSpan.FromSeconds(introDuration), true);

            await introDurationTask;

            okButton.interactable = true;

            bool isInLoop = false;
            float loopStartTime = 10f;
            while(okButtonCancellationSource.IsCancellationRequested == false)
            {
                var duration = devicePlayableDirector.duration;
                if(isInLoop)
                {
                    devicePlayableDirector.initialTime = loopStartTime;
                    duration -= loopStartTime;
                }
                else
                {
                    isInLoop = true;
                }
                devicePlayableDirector.Play();
                await UniTask.Delay(System.TimeSpan.FromSeconds(duration), true, cancellationToken: okButtonCancellationSource.Token);
                devicePlayableDirector.Stop();
            }
        }

        private (int money, float duration) CalcRewardMoneyAndDuration()
        {
            int money = 0;
            float duration = 0f;

            money += Random.Range(100, 200);
            duration += 1f;

            var deviceNum = inventoryManager.GetDeviceNum();
            if(deviceNum > 2)
            {
                float alpha = (deviceNum - 3) / 2f;
                int minMoney = Mathf.FloorToInt(Mathf.Lerp(100, 200, alpha));
                int maxMoney = Mathf.FloorToInt(Mathf.Lerp(200, 400, alpha));

                money += Random.Range(minMoney, maxMoney);
                duration += 0.3f;
            }

            bool shouldAddMaxLevelDuration = false;
            for(int i = 0; i < deviceNum; i++)
            {
                var leveledDeviceInfo = inventoryManager.GetDeviceAtTrimIndex(i);

                tableDataHolder.arcaneDeviceDict.TryGetValue(leveledDeviceInfo.GetArcaneDeviceId, out var deviceInfo);
                if(leveledDeviceInfo.DeviceLevel == deviceInfo.MaxLevel)
                {
                    money += Random.Range(leveledDeviceInfo.DeviceLevel * 10, leveledDeviceInfo.DeviceLevel * 20);
                    shouldAddMaxLevelDuration = true;
                }
            }
            if (shouldAddMaxLevelDuration)
                duration += 0.3f;

            // 진화스킬 수에 따른 처리 추가.

            return (money, duration);
        }
    }
}