using Cysharp.Threading.Tasks;
using Reflex;
using Reflex.Scripts.Attributes;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.ECS
{
    public class DeviceTestSceneManager : MonoBehaviour
    {
        public string timelineTable;
        public CharacterDataEntity characterDataEntity = CharacterDataEntity.GetTeeste;
        public StartDevice[] startDevices;
        public BuffData[] startBuffs;
        public ItemKind[] itemSpawns;
        public MonsterSpawnInfo[] monsterSpawns;

        [Inject] private Container container;
        [Inject] private ProjectContextModel projectContextModel;
        [Inject] private TableDataHolder tableDataHolder;

        // Start is called before the first frame update
        void Start()
        {
            LoadAndSettingScene().Forget();
        }

        public void OnClickReload()
        {
            ReloadScene().Forget();
        }
        
        public void OnClickResetCharacter()
        {
            characterDataEntity = CharacterDataEntity.GetTeeste;
        }

        public void OnClickSetCharacter(TestCharacterSelection selection)
        {
            var copy = Instantiate(tableDataHolder);
            copy.LoadCSV();

            if(copy.CharacterCollection.TryGetEntity(((int)selection).ToString(), out var _characterDataEntity))
                characterDataEntity = _characterDataEntity;
            DestroyImmediate(copy);
        }

        private async UniTask ReloadScene()
        {
            if(Application.isPlaying)
            {
                await SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1), UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                await LoadAndSettingScene();
            }
        }

        private async UniTask LoadAndSettingScene()
        {
            projectContextModel.characterSelectionById = false;
            projectContextModel.selectedCharacterEntity = characterDataEntity;
            await SceneManager.LoadSceneAsync("TilemapTest", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

            var gameScene = SceneManager.GetSceneAt(1);
            var rootObjects = gameScene.GetRootGameObjects();

            var director = rootObjects.First(elmt => elmt.GetComponent<TimelineManager>() != null).GetComponent<TimelineManager>();
            director.timeline = timelineTable;

            await UniTask.DelayFrame(1);

            AddStartDevices();

            AddBuffs();

            int _index = 0;

            if(string.IsNullOrEmpty(timelineTable))
            {
                foreach (var monsterSpawn in monsterSpawns)
                {
                    director.AddTrack(new StageLevelTimelineEntity(new StageLevelTimelineData()
                    {
                        index = (_index).ToString(),
                        time = monsterSpawn.time.ToString(),
                        type = "Wave",
                        waveType = "Circle",
                        wave_monsterId = monsterSpawn.monsterId,
                        wave_num = "1",
                        circle_radius = "3",
                        circle_angleOffset = (_index % 2 == 0) ? "0" : "180",
                    }));
                    _index++;
                }
            }

            var tokenFactory = container.Parent.Children.Last().Resolve<TokenFactory>();
            var entityContainer = container.Parent.Children.Last().Resolve<IEntityContainer>();
            for (int i = 0; i < itemSpawns.Count(); i++)
            {
                var item = itemSpawns[i];
                var tokenPath = item switch
                {
                    ItemKind.보석 => GameSettings.GetExpTokenPrefabPath(1),
                    ItemKind.골드 => GameSettings.GetGoldTokenPrefabPath(1 / 100f),
                    ItemKind.케이크 => GameSettings.CakeTokenPath,
                    ItemKind.자석 => GameSettings.MagnetTokenPath,
                    ItemKind.박스 => GameSettings.DeviceBoxTokenPath,
                    ItemKind.폭탄 => GameSettings.BombTokenPath,
                    ItemKind.피버 => GameSettings.GoldFeverTokenPath,
                    ItemKind.거대 => GameSettings.GiganticTokenPath,
                    ItemKind.축소 => GameSettings.MicrifyTokenPath,
                    _=> GameSettings.GetExpTokenPrefabPath(1),
                };

                var tokenGo = tokenFactory.CreateGameObject(tokenPath);
                var tokenEntity = tokenGo.GetComponent<TokenEntity>();

                var position = (Vector2)entityContainer.playerCharacterEntity.transformImplement.position + Vector2.up * 2 + (Vector2.right * 2 * (i - (itemSpawns.Count() / 2f)));
                tokenEntity.transformImplement.position = position;
            }
        }

        public void AddStartDevices()
        {
            foreach (var device in startDevices)
            {
                string key = $"{ExtensionFunction.GetPrefixStringByArcaneDeviceType(device.arcaneDeviceType)}_{device.deviceId}";
                if (device.deviceSelection != TestDeviceSelection.아이디로_고르기)
                    key = ToKey(device.deviceSelection);

                tableDataHolder.arcaneDeviceDict.TryGetValue(key, out var deviceInfo);

                container.Parent.Children.Last().Resolve<DeviceInventoryManager>().AddOrUpdateArcaneDevice(deviceInfo, device.level);
            }
        }

        private void AddBuffs()
        {
            var playerCharacter = container.Parent.Children.Last().Resolve<IEntityContainer>().playerCharacterEntity;
            foreach(var buffData in startBuffs)
            {
                Buff buff = new Buff() { statusValue = buffData };
                playerCharacter.buffImplement.appliedBuff.Add(buff);
                playerCharacter.OnApplyPassiveArcaneDevice();
            }
        }

        public enum TestDeviceSelection
        {
            아이디로_고르기,
            장창,
            활,
            산탄총,
            도끼,
            신성한_지팡이,
            번개_완드,
            아케인_회전체,
            
            가시방패,
            갑옷,
            왜곡의_시계,
            쌍둥이_반지,
            염력_오브,
            마력_증폭기,
            전투_자극제,
            경험의_고서,
            신속의_장화,
            치유의_잔,
            중력장_망토
        }

        public static string ToKey(TestDeviceSelection selection) => selection switch
        {
            TestDeviceSelection.장창          => "Active_1",
            TestDeviceSelection.활            => "Active_2",
            TestDeviceSelection.산탄총        => "Active_3",
            TestDeviceSelection.도끼          => "Active_4",
            TestDeviceSelection.신성한_지팡이 => "Active_5",
            TestDeviceSelection.번개_완드     => "Active_6",
            TestDeviceSelection.아케인_회전체 => "Active_7",
            TestDeviceSelection.가시방패      => "Passive_1",
            TestDeviceSelection.갑옷          => "Passive_2",
            TestDeviceSelection.왜곡의_시계   => "Passive_3",
            TestDeviceSelection.쌍둥이_반지   => "Passive_4",
            TestDeviceSelection.염력_오브     => "Passive_5",
            TestDeviceSelection.마력_증폭기   => "Passive_6",
            TestDeviceSelection.전투_자극제   => "Passive_7",
            TestDeviceSelection.경험의_고서   => "Passive_8",
            TestDeviceSelection.신속의_장화   => "Passive_9",
            TestDeviceSelection.치유의_잔     => "Passive_10",
            TestDeviceSelection.중력장_망토   => "Passive_11",
            _ => throw new System.NotImplementedException(),
        };

        [System.Serializable]
        public struct StartDevice
        {
            public TestDeviceSelection deviceSelection;
            public ArcaneDeviceType arcaneDeviceType;
            public string deviceId;
            [Range(1, 6)]
            public int level;
        }

        public enum TestCharacterSelection
        {
            킬론 = 1,
            에버릿 = 2,
            연채영 = 3
        }

        [System.Serializable]
        public struct MonsterSpawnInfo
        {
            public float time;
            public string monsterId;
        }

        public enum ItemKind
        {
            보석,
            골드,
            케이크,
            자석,
            박스,
            폭탄,
            피버,
            거대,
            축소
        }
    }
}
