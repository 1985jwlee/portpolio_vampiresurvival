using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game.ECS
{
    public class GetArcaneDeviceView : UiView
    {
        [Inject] MainGameSceneContextModel sceneModel;
        [Inject] TableDataHolder tableDataHolder;
        [Inject] DeviceInventoryManager inventoryManager;
        [Inject] private IEntityContainer entityContainer;

        public DeviceInventoryView deviceInventoryView;

        public RectTransform choosableDeviceUiParent;
        public RectTransform alternativeChoosableDeviceUiParent;

        public DeviceChoosableView[] deviceChoosableViews;

        public DeviceChoosableView moneyChoosableView;
        public DeviceChoosableView cakeChoosableView;

        private List<ILeveledArcaneDeviceInfo> choosableDevices = new();

        public override void Init()
        {
            gameObject.SetActive(false);

            MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch (message)
                {
                    case "selectDeviceByLevelUp":
                        entityContainer.playerCharacterEntity.mobileJoystickImplement.Pause();
                        gameObject.SetActive(true);
                        Time.timeScale = 0f;
                        InitChoosables();
                        deviceInventoryView.InitSlots();
                        break;
                };
            }).AddTo(this);

            for (int i = 0; i < deviceChoosableViews.Length; i++)
            {
                int index = i;
                deviceChoosableViews[i].button.OnPointerClickAsObservable().Select(_ => index).Subscribe(index =>
                {
                    var chosenDevice = choosableDevices[index];

                    inventoryManager.AddOrUpdateArcaneDevice(chosenDevice);

                    OnClickChoosableButton();
                }).AddTo(this);
            }

            moneyChoosableView.InitWithExtraReward(DeviceChoosableView.ExtraReward.Gold, true);
            moneyChoosableView.button.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new KeyValuePair<string, uint>("addGold", GameSettings.SubstituteGolds));
                OnClickChoosableButton();
            }).AddTo(this);

            cakeChoosableView.InitWithExtraReward(DeviceChoosableView.ExtraReward.Cake, true);
            cakeChoosableView.button.OnClickAsObservable().Subscribe(_ =>
            {
                var playerCharacterEntity = entityContainer.playerCharacterEntity;
                playerCharacterEntity.buffImplement.appliedBuff.Add(new Buff { statusValue = new BuffData { buffType = BuffType.Heal, buffValue = GameSettings.SubstituteHeal, remainTime = -1, rootCharacter = new CharacterType { statusValue = CharacterTypes.Token } } });
                OnClickChoosableButton();
            }).AddTo(this);
        }

        public void OnDisable()
        {
            deviceInventoryView.gameObject.SetActive(false);
        }

        void OnClickChoosableButton()
        {
            entityContainer.playerCharacterEntity.mobileJoystickImplement.Resume();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            MessageBroker.Default.Publish("timeStopEventDone");
        }

        private void Update()
        {
            int index = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                index = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                index = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                index = 2;

            if (index != -1)
            {
                var chosenDevice = choosableDevices[index];
                inventoryManager.AddOrUpdateArcaneDevice(chosenDevice);
                OnClickChoosableButton();
            }
        }

        private void InitChoosables()
        {
            choosableDevices.Clear();

            List<ILeveledArcaneDeviceInfo> selectedDevices = inventoryManager.GetChoosableDeviceList(3, true, true);

            if (selectedDevices.Count == 0)
            {
                choosableDeviceUiParent.gameObject.SetActive(false);
                alternativeChoosableDeviceUiParent.gameObject.SetActive(true);
            }
            else
            {
                var length = Mathf.Min(deviceChoosableViews.Length, selectedDevices.Count);
                for (int i = 0; i < length; i++)
                {
                    choosableDevices.Add(selectedDevices[i]);
                    deviceChoosableViews[i].InitWithDevice(selectedDevices[i]);
                }
                for (int i = length; i < deviceChoosableViews.Length; i++)
                    deviceChoosableViews[i].gameObject.SetActive(false);
            }
        }

        public void ShowInventory()
        {
            deviceInventoryView.Show();
        }

        public void HideInventory()
        {
            deviceInventoryView.Hide();
        }
    }
}