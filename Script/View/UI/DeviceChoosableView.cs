using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Game.ECS
{
    public class DeviceChoosableView : MonoBehaviour
    {
        [Inject] private TableDataHolder tableDataHolder;
        [Inject] private DeviceInventoryManager inventoryManager;

        public TextMeshProUGUI deviceName;
        public Image deviceIcon;
        public TextMeshProUGUI description;

        public Graphic[] levelIndicatorSlots;
        public Image[] levelIndicators;
        public Image background;

        public Button button;
        public RectTransform toolTip;

        public Image newIcon;

        public ParticleSystem composeFx;

        public TextMeshProUGUI composeInfoText;
        public Image[] composeInfoSlots;
        public Image[] composeInfoContents;

        private Tweener fliker;

        public void Start()
        {
            if (toolTip != null)
            {
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    toolTip.gameObject.SetActive(toolTip.gameObject.activeInHierarchy == false);
                });
            }
        }

        public void InitWithDevice(ILeveledArcaneDeviceInfo leveledDeviceInfo)
        {
            deviceName.text = leveledDeviceInfo.DeviceName;
            deviceIcon.sprite = inventoryManager.LoadDeviceIcon(leveledDeviceInfo);
            description.text = leveledDeviceInfo.Description;

            if (newIcon != null)
            {
                bool isNew = inventoryManager.Containes(leveledDeviceInfo) == false && leveledDeviceInfo.DeviceType != ArcaneDeviceType.ADVANCED_ACTIVE;
                newIcon.gameObject.SetActive(isNew);
            }

            if (background != null)
                background.sprite = LoadDeviceChoosableBackgroundImage(leveledDeviceInfo.DeviceSlot);

            if (levelIndicatorSlots.Length != 0 && levelIndicators.Length != 0)
            {
                if (fliker != null)
                    fliker.Kill();

                var maxLevel = tableDataHolder.arcaneDeviceDict[leveledDeviceInfo.GetArcaneDeviceId].MaxLevel;
                InitChoosableLevelIndicator(leveledDeviceInfo.DeviceLevel, maxLevel);
            }

            if (toolTip != null)
                toolTip.gameObject.SetActive(false);

            if(composeFx != null)
                composeFx.gameObject.SetActive(leveledDeviceInfo.DeviceType == ArcaneDeviceType.ADVANCED_ACTIVE);

            SetUpComposeInfo(leveledDeviceInfo);
        }
        public void InitWithExtraReward(ExtraReward extra, bool interactable)
        {
            deviceName.text = extra switch
            {
                ExtraReward.Gold => "골드",
                ExtraReward.Cake => "사과",
                _ => ""
            };
            deviceIcon.sprite = LoadExtraRewardIcon(extra);
            description.text = extra switch
            {
                ExtraReward.Gold => $"골드를 {GameSettings.SubstituteGolds} 획득합니다.",
                ExtraReward.Cake => $"체력을 {GameSettings.SubstituteGolds} 회복합니다.",
                _ => ""
            };

            if (newIcon != null)
            {
                newIcon.gameObject.SetActive(false);
            }

            if (background != null)
            {
                background.sprite = LoadExtraRewardBackgroundImage();
            }

            if (levelIndicatorSlots.Length != 0 && levelIndicators.Length != 0)
            {
                for (int i = 0; i < levelIndicatorSlots.Length; i++)
                    levelIndicatorSlots[i].gameObject.SetActive(false);
            }

            if (toolTip != null)
                toolTip.gameObject.SetActive(false);

            if (composeFx != null)
                composeFx.gameObject.SetActive(false);

            if (composeInfoText != null)
                composeInfoText.gameObject.SetActive(false);

            foreach (var slot in composeInfoSlots)
                slot.gameObject.SetActive(false);
        }

        private void SetUpComposeInfo(ILeveledArcaneDeviceInfo leveledDeviceInfo)
        {
            if (composeInfoText != null && composeInfoSlots.Count() != 0 && composeInfoContents.Count() != 0)
            {
                foreach (var slot in composeInfoSlots)
                    slot.gameObject.SetActive(false);

                composeInfoText.gameObject.SetActive(false);

                if (leveledDeviceInfo.DeviceType != ArcaneDeviceType.ADVANCED_ACTIVE)
                {
                    int advDeviceNum = 0;
                    var advDevices = tableDataHolder.AdvActiveDeviceCollection.GetIterator();
                    foreach (var advDevice in advDevices)
                    {
                        var advanceInfo = advDevice.AdvanceInfo;
                        if (advanceInfo.HasValue)
                        {
                            if (IsInAdvanceInfo(advanceInfo.Value, leveledDeviceInfo))
                            {
                                var composeOppsiteInfo = GetComposeOppositeDevice(advanceInfo.Value, leveledDeviceInfo);
                                composeInfoSlots[advDeviceNum].gameObject.SetActive(true);
                                composeInfoContents[advDeviceNum].sprite = inventoryManager.LoadDeviceIcon(composeOppsiteInfo);
                                advDeviceNum++;
                            }
                        }
                    }

                    if(advDeviceNum > 0)
                        composeInfoText.gameObject.SetActive(true);
                }
            }
        }

        public bool IsInAdvanceInfo(AdvanceInfo advanceInfo, ILeveledArcaneDeviceInfo leveledArcaneDeviceInfo)
        {
            bool isUnadvancedActiveDevice = (leveledArcaneDeviceInfo.DeviceType == ArcaneDeviceType.ACTIVE || leveledArcaneDeviceInfo.DeviceType == ArcaneDeviceType.CHARACTER_OWNED_ACTIVE);
            bool isPassive = leveledArcaneDeviceInfo.DeviceType == ArcaneDeviceType.PASSIVE;
            bool isMainActive = isUnadvancedActiveDevice && advanceInfo.MainActiveId == leveledArcaneDeviceInfo.DeviceId; 
            bool isSubPassive = isPassive && advanceInfo.SubPassiveId == leveledArcaneDeviceInfo.DeviceId;
            bool isSubActive = isUnadvancedActiveDevice && advanceInfo.SubActiveId == leveledArcaneDeviceInfo.DeviceId;

            if(isMainActive || isSubPassive || isSubActive)
                return true;
            else
                return false;
        }

        public IArcaneDeviceInfo GetComposeOppositeDevice(AdvanceInfo advanceInfo, ILeveledArcaneDeviceInfo leveledArcaneDeviceInfo)
        {
            bool isUnadvancedActiveDevice = (leveledArcaneDeviceInfo.DeviceType == ArcaneDeviceType.ACTIVE || leveledArcaneDeviceInfo.DeviceType == ArcaneDeviceType.CHARACTER_OWNED_ACTIVE);
            bool isMainActive = isUnadvancedActiveDevice && advanceInfo.MainActiveId == leveledArcaneDeviceInfo.DeviceId;

            if(isMainActive)
            {
                if(string.IsNullOrEmpty(advanceInfo.SubPassiveId) == false)
                    return inventoryManager.GetArcaneDeviceInfo(ArcaneDeviceType.PASSIVE, advanceInfo.SubPassiveId);
                else
                    return inventoryManager.GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advanceInfo.SubActiveId);
            }
            else
            {
                return inventoryManager.GetArcaneDeviceInfo(ArcaneDeviceType.ACTIVE, advanceInfo.MainActiveId);
            }
        }

        private void InitChoosableLevelIndicator(int level, int maxLevel)
        {
            for (int i = 0; i < levelIndicators.Length; i++)
            {
                if (i < level)
                {
                    levelIndicators[i].gameObject.SetActive(true);
                    levelIndicators[i].color = Color.white;
                    if (i + 1 == level)
                        fliker = levelIndicators[i].DOFade(0f, 1f).SetLoops(-1).SetUpdate(true);
                }
                else
                {
                    levelIndicators[i].gameObject.SetActive(false);
                }

                levelIndicatorSlots[i].gameObject.SetActive(i < maxLevel);
            }
        }

        public Sprite LoadDeviceChoosableBackgroundImage(DeviceSlot type)
        {
            string name = type switch
            {
                DeviceSlot.WeaponSlot => "Blue",
                DeviceSlot.MagicSlot => "Red",
                DeviceSlot.LegacySlot => "Yellow",
                _ => "Etc"
            };

            return Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Etc").GetSprite($"Panel_{name}");
        }

        public Sprite LoadExtraRewardBackgroundImage()
        {
            return Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Etc").GetSprite($"Panel_Wht");
        }

        public Sprite LoadExtraRewardIcon(ExtraReward extraReward)
        {
            string name = extraReward switch
            {
                ExtraReward.Gold => "UI_Com_Coin_Icon",
                ExtraReward.Cake => "UI_Com_Cake_Icon",
                _ => "Gold"
            };

            return Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Com_Device_165x165").GetSprite(name);
        } 

        public enum ExtraReward
        {
            Gold,
            Cake
        }
    }
}
