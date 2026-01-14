using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.ECS
{
    public class DeviceInventoryView : MonoBehaviour
    {
        [Inject] MainGameSceneContextModel sceneModel;
        [Inject] TableDataHolder tableDataHolder;
        [Inject] DeviceInventoryManager inventoryManager;

        public RectTransform slotParent;
        public DeviceSlotView deviceSlotView;
        public TextMeshProUGUI deviceNum;
        public Animator animator;

        private List<DeviceSlotView> deviceSlotViews = new ();

        public void InitSlots()
        {
            deviceNum.text = $"아케인 디바이스 ({inventoryManager.GetDeviceNum()}/{inventoryManager.GetSlotNum()})";

            var slotNum = inventoryManager.GetSlotNum();
            for (int i = deviceSlotViews.Count; i < slotNum; i++)
                deviceSlotViews.Add(Instantiate(deviceSlotView, slotParent));
            deviceSlotView.gameObject.SetActive(false);

            for (int i = 0; i < deviceSlotViews.Count; i++)
            {
                var slotType = inventoryManager.GetSlotType(i);
                deviceSlotViews[i].background.sprite = LoadSlotBackgroundImage(slotType);

                ILeveledArcaneDeviceInfo device = inventoryManager.GetDeviceAtSlotIndex(i);
                if (device != null)
                {
                    deviceSlotViews[i].deviceIcon.sprite = inventoryManager.LoadDeviceIcon(device);
                    deviceSlotViews[i].deviceIcon.gameObject.SetActive(true);
                    var maxLevel = tableDataHolder.arcaneDeviceDict[device.GetArcaneDeviceId].MaxLevel;
                    InitSlotLevelIndicator(deviceSlotViews[i], device.DeviceLevel, maxLevel);
                }
                else
                {
                    deviceSlotViews[i].deviceIcon.gameObject.SetActive(false);
                    InitSlotLevelIndicator(deviceSlotViews[i], 0, 0);
                }
            }

            void InitSlotLevelIndicator(DeviceSlotView slotView, int level, int maxLevel)
            {
                for (int i = 0; i < slotView.levelIndicators.Length; i++)
                {
                    slotView.levelIndicators[i].gameObject.SetActive(i < level);
                    slotView.levelIndicatorSlots[i].gameObject.SetActive(i < maxLevel);
                }
            }
        }

        public Sprite LoadSlotBackgroundImage(DeviceSlot type)
        {
            string name = type switch
            {
                DeviceSlot.WeaponSlot => "Blue",
                DeviceSlot.MagicSlot => "Red",
                DeviceSlot.LegacySlot => "Yellow",
                _ => "Weapon"
            };

            return Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Etc").GetSprite($"Slot_{name}");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            HideTask().Forget();
        }

        public async UniTask HideTask()
        {
            animator.SetTrigger("Close");

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            gameObject.SetActive(false);
        }
    }
}
