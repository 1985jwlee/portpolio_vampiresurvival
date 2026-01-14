using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.ECS
{
    public class PauseView : UiView
    {
        public DeviceInventoryView deviceInventoryView;
        public Button resumeButton;
        public Button exitButton;
        public RectTransform iconArea;
        public Image playerIcon;
        public Image bossIcon;
        public Image mainBossIcon;
        public Image deviceBoxIcon;
        public Image teleportIcon;

        [Inject] private IEntityContainer entityContainer;
        [Inject] private MainGameSceneContextModel mainGameSceneContextModel;

        private List<Image> bossIconPool = new();
        private List<Image> deviceBoxIconPool = new();
        private List<Image> teleportIconPool = new();

        public override void Init()
        {
            playerIcon.rectTransform.anchoredPosition = new Vector2(0, 0);
            bossIcon.rectTransform.anchoredPosition = new Vector2(0, 0);
            mainBossIcon.rectTransform.anchoredPosition = new Vector2(0, 0);
            deviceBoxIcon.rectTransform.anchoredPosition = new Vector2(0, 0);
            teleportIcon.rectTransform.anchoredPosition = new Vector2(0, 0);

            gameObject.SetActive(false);
            deviceBoxIcon.gameObject.SetActive(false);
            bossIcon.gameObject.SetActive(false);
            teleportIcon.gameObject.SetActive(false);

            MessageBroker.Default.Receive<string>().Where(_ => gameObject.activeInHierarchy == false).Subscribe(message =>
            {
                switch (message)
                {
                    case "pause":
                        entityContainer.playerCharacterEntity.mobileJoystickImplement.Pause();
                        gameObject.SetActive(true);
                        deviceInventoryView.gameObject.SetActive(true);
                        Time.timeScale = 0f;
                        deviceInventoryView.InitSlots();
                        UpdateMap();
                        break;
                };
            }).AddTo(this);

            resumeButton.OnClickAsObservable().Subscribe(_ =>
            {
                OnClickResume();
            }).AddTo(this);

            exitButton.OnClickAsObservable().Subscribe(_ =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("GameStartScene");
            }).AddTo(this);
        }

        public void OnDisable()
        {
            deviceInventoryView.gameObject.SetActive(false);
        }

        private void UpdateMap()
        {
            Vector2 mapSize = mainGameSceneContextModel.mapBound.size;

            var playerPosition = entityContainer.playerCharacterEntity.transformImplement.position;
            playerIcon.rectTransform.anchorMin = playerIcon.rectTransform.anchorMax = CalcRate(playerPosition, mapSize);

            var mainBossPosition = mainGameSceneContextModel.mainBossIconPosition;
            mainBossIcon.rectTransform.anchorMin = mainBossIcon.rectTransform.anchorMax = CalcRate(mainBossPosition, mapSize);

            ProcBossIcon(mainGameSceneContextModel.bossIconPositions, mapSize);
            ProcDeviceBoxIcon(mapSize);
            ProcTeleportIcon(mainGameSceneContextModel.teleportIconPositions, mapSize);
        }

        private void ProcBossIcon(List<Vector2> bossPositions, Vector2 mapSize)
        {
            foreach (var icon in bossIconPool)
                icon.gameObject.SetActive(false);

            for (int i = 0; i < bossPositions.Count; i++)
            {
                if (bossIconPool.Count < i + 1)
                    bossIconPool.Add(Instantiate(bossIcon, iconArea));

                bossIconPool[i].gameObject.SetActive(true);
                bossIconPool[i].rectTransform.anchorMin = bossIconPool[i].rectTransform.anchorMax = CalcRate(bossPositions[i], mapSize);
            }
        }

        private void ProcTeleportIcon(List<Vector2> teleportPositions, Vector2 mapSize)
        {
            foreach (var icon in teleportIconPool)
                icon.gameObject.SetActive(false);

            for (int i = 0; i < teleportPositions.Count; i++)
            {
                if (teleportIconPool.Count < i + 1)
                    teleportIconPool.Add(Instantiate(teleportIcon, iconArea));

                teleportIconPool[i].gameObject.SetActive(true);
                teleportIconPool[i].rectTransform.anchorMin = teleportIconPool[i].rectTransform.anchorMax = CalcRate(teleportPositions[i], mapSize);
            }
        }

        private static Vector2 CalcRate(Vector3 position, Vector2 mapSize) => position / mapSize + Vector2.one * 0.5f;

        void ProcDeviceBoxIcon(Vector2 mapSize)
        {
            List<Vector2> deviceBoxPositions = new List<Vector2>();
            var tokenEntities = entityContainer.GetEntities<TokenEntity>();
            foreach (var entity in tokenEntities)
            {
                if (entity.applyBuffImplement.applyBuffList.Count > 0)
                {
                    if (entity.applyBuffImplement.applyBuffList[0].statusValue.buffType == BuffType.DeviceBox)
                    {
                        deviceBoxPositions.Add(entity.transformImplement.position);
                    }
                }
            }

            foreach (var icon in deviceBoxIconPool)
                icon.gameObject.SetActive(false);

            for (int i = 0; i < deviceBoxPositions.Count; i++)
            {
                if (deviceBoxIconPool.Count < i + 1)
                    deviceBoxIconPool.Add(Instantiate(deviceBoxIcon, iconArea));

                deviceBoxIconPool[i].gameObject.SetActive(true);
                deviceBoxIconPool[i].rectTransform.anchorMin = deviceBoxIconPool[i].rectTransform.anchorMax = CalcRate(deviceBoxPositions[i], mapSize);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickResume();
            }
        }

        private void OnClickResume()
        {
            entityContainer.playerCharacterEntity.mobileJoystickImplement.Resume();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
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
