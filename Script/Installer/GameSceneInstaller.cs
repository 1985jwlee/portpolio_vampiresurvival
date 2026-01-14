using Game.ECS;
using Reflex;
using Reflex.Scripts;
using System.Linq;
using UnityEngine;

public class GameSceneInstaller : Installer
{
    [SerializeField] private TimelineManager timeLineManager;
    [SerializeField] private CustomRenderTexture crt;
    [SerializeField] private Camera minimapCamera;

    public override void InstallBindings(Container container)
    {
        container.BindInstance(timeLineManager);
        container.BindInstance(GetComponent<Camera>());
        container.BindSingleton<PlayerCharacterFactory, PlayerCharacterFactory>();
        container.BindSingleton<EnemyCharacterFactory, EnemyCharacterFactory>();
        container.BindSingleton<WeaponFactory, WeaponFactory>();
        container.BindSingleton<IndicatorFactory, IndicatorFactory>();
        container.BindSingleton<TokenFactory, TokenFactory>();
        container.BindSingleton<SummonObjectFactory,SummonObjectFactory>();
        container.BindSingleton<MapFactory, MapFactory>();
        container.BindSingleton<EventTriggerFactory, EventTriggerFactory>();
        container.BindSingleton<ItemBoxFactory, ItemBoxFactory>();
        container.BindSingleton<DummyFactory, DummyFactory>();
        container.BindSingleton<InteractableFactory, InteractableFactory>();
        container.BindSingleton<VfxFactory, VfxFactory>();
        container.BindSingleton<MainGameSceneContextModel, MainGameSceneContextModel>();
        container.BindSingleton<DeviceInventoryManager, DeviceInventoryManager>();
        container.BindSingleton<IEntityContainer, EntityContainer>();
        container.BindSingleton<TickGameSystemManager, TickGameSystemManager>();
        container.BindInstanceAs(Camera.main);
        BindGameSystems(container);

        var mapScope = container.Scope("Map");
        mapScope.BindInstance(crt);
        mapScope.BindInstance(minimapCamera);

        OnBinded(container);
    }

    private static void BindGameSystems(Container container)
    {
        container.BindSingleton<IEnemyMeleeAttackSystem, EnemyMeleeAttackSystem>();
        container.BindSingleton<ISeekingTargetSystem, SeekTargetSystem>();
        container.BindSingleton<ISpearAttackSystem, SpearAttackSystem>();
        container.BindSingleton<ISyncEntityTransformSystem, SyncEntityTransformSystem>();
        container.BindSingleton<IRemoveLifeOverAttackSystem, RemoveLifeOverAttackSystem>();
        container.BindSingleton<ICandidateRemoveEntitySystem, CandidateRemoveEntitySystem>();
        container.BindSingleton<ICameraFollowSystem, CameraFollowSystem>();
        container.BindSingleton<IGoldFeverTimeSystem, GoldFeverTimeSystem>();
        container.BindSingleton<IMonsterKillAchivementSystem, MonsterKillAchivementSystem>();
        container.BindSingleton<IDealingDamageAchivementSystem, DealingDamageAchivementSystem>();
        container.BindSingleton<IDealingDamageSpecificDeviceAchievementSystem, DealingDamageSpecificDeviceAchievementSystem>();
        container.BindSingleton<IGetGoldAchievementSystem, GetGoldAchievementSystem>();
    }

    private static void OnBinded(Container container)
    {
        var projectContextModel = container.Resolve<ProjectContextModel>();
        var sceneContextModel = container.Resolve<MainGameSceneContextModel>();

        if(projectContextModel.characterSelectionById)
        {
            var tableDataHolder = container.Resolve<TableDataHolder>();
            if (tableDataHolder.CharacterCollection.TryGetEntity(projectContextModel.selectedCharacterId, out var characterDataEntity))
                sceneContextModel.characterDataEntity = characterDataEntity;
            else
                sceneContextModel.characterDataEntity = tableDataHolder.CharacterCollection.GetIterator().ElementAt(3);

            if (tableDataHolder.CharacterCollection.TryGetEntity(projectContextModel.selectedCharacterId, out var support0CharacterDataEntity))
                sceneContextModel.supporterCharacter0DataEntity = support0CharacterDataEntity;
            else
                sceneContextModel.supporterCharacter0DataEntity = tableDataHolder.CharacterCollection.GetIterator().ElementAt(4);

            if (tableDataHolder.CharacterCollection.TryGetEntity(projectContextModel.selectedCharacterId, out var support1CharacterDataEntity))
                sceneContextModel.supporterCharacter1DataEntity = support1CharacterDataEntity;
            else
                sceneContextModel.supporterCharacter1DataEntity = tableDataHolder.CharacterCollection.GetIterator().ElementAt(5);
        }
        else
        {
            sceneContextModel.characterDataEntity = projectContextModel.selectedCharacterEntity;
            sceneContextModel.supporterCharacter0DataEntity = projectContextModel.selectedCharacterEntity;
            sceneContextModel.supporterCharacter1DataEntity = projectContextModel.selectedCharacterEntity;
        }
    }
}
 