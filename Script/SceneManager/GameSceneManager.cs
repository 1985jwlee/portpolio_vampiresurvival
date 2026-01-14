using Cysharp.Threading.Tasks;
using Reflex;
using Reflex.Scripts.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.ECS
{
    public class GameSceneManager : MonoBehaviour
    {
        [Inject] private TimelineManager timelineManager;
        [Inject]private PlayerCharacterFactory playerCharacterFactory;
        [Inject]private EnemyCharacterFactory enemyCharacterFactory;

        [Inject]private MapFactory mapFactory;
        
        [Inject]private MainGameSceneContextModel sceneModel;

        [Inject] private IEntityContainer entityContainer;
        [Inject] private TickGameSystemManager tickGameSystemManager;
        [Inject] private IEnemyMeleeAttackSystem enemyMeleeAttackSystem;
        [Inject] private ISeekingTargetSystem seekingTargetSystem;
        [Inject] private ISpearAttackSystem spearAttackSystem;
        [Inject] private ISyncEntityTransformSystem syncEntityTransformSystem;
        [Inject] private IRemoveLifeOverAttackSystem removeLifeOverAttackSystem;
        [Inject] private ICandidateRemoveEntitySystem candidateRemoveEntitySystem;
        [Inject] private ICameraFollowSystem cameraFollowSystem;
        [Inject] private DeviceInventoryManager deviceInventoryManager;
        [Inject] private ItemBoxFactory itemBoxFactory;
        
        [Inject] private IMonsterKillAchivementSystem monsterKillAchivementSystem;
        [Inject] private IDealingDamageAchivementSystem dealingDamageAchivementSystem;
        [Inject] private IDealingDamageSpecificDeviceAchievementSystem dealingDamageSpecificDeviceAchievementSystem;
        [Inject] private IGetGoldAchievementSystem getGoldAchievementSystem;

        [SerializeField] public PlayableDirector startTimeline;

        private void Awake()
        {
            enemyMeleeAttackSystem.InitGameSystem();
            seekingTargetSystem.InitGameSystem();
            spearAttackSystem.InitGameSystem();
            syncEntityTransformSystem.InitGameSystem();
            removeLifeOverAttackSystem.InitGameSystem();
            candidateRemoveEntitySystem.InitGameSystem();
            cameraFollowSystem.InitGameSystem();
            monsterKillAchivementSystem.InitGameSystem();
            dealingDamageAchivementSystem.InitGameSystem();
            dealingDamageSpecificDeviceAchievementSystem.InitGameSystem();
            getGoldAchievementSystem.InitGameSystem();
        }

        private void Start()
        {
            mapFactory.CreateMap();
            var playerCharacter = playerCharacterFactory.CreateGameObject($"Prefabs/Char/PC/{sceneModel.characterDataEntity.prefabName}");
            playerCharacter.transform.position = sceneModel.startPosition;
            timelineManager.LoadTimelineTable();

            PlayStartTimelineThenPlayTimeline(playerCharacter.GetComponent<PlayerCharacterEntity>()).Forget();
        }

        private async UniTask PlayStartTimelineThenPlayTimeline(PlayerCharacterEntity playerCharacterEntity)
        {
            playerCharacterEntity.weaponInventoryImplement.WeaponCoolTimePauseProperty.statusValue = true;
            startTimeline.Play();
            await UniTask.Delay(System.TimeSpan.FromSeconds(startTimeline.duration));

            timelineManager.pause = false;
            playerCharacterEntity.weaponInventoryImplement.WeaponCoolTimePauseProperty.statusValue = false;
        }

        void Update()
        {
            tickGameSystemManager.UpdateSystem();
            enemyCharacterFactory.Tick();
            itemBoxFactory.Tick();
        }
    }
}
