using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class TimelineManager : MonoBehaviour
    {
        public bool pause = true;
        private int executeTimeline;
        public float time;

        public string timeline = "Stage_1_1_1";

        private List<StageLevelTimelineEntity> queuedTracks = new List<StageLevelTimelineEntity>();

        [Inject] private TableDataHolder tableDataHolder;
        [Inject] private EnemyCharacterFactory enemyCharacterFactory;
        [Inject] private MainGameSceneContextModel sceneModel;
        [Inject] private IEntityContainer entityContainer;

        public void LoadTimelineTable()
        {
            queuedTracks.Clear();

            if (tableDataHolder.stageTimelineCollections.TryGetValue(timeline, out var collection))
            {
                foreach (var c in collection.GetIterator())
                {
                    queuedTracks.Add(c);
                }
            }
        }

        public void AddTrack(StageLevelTimelineEntity entity)
        {
            queuedTracks.Add(entity);
        }

        private void Update()
        {
            UpdateTimeline();
            if (Input.GetKeyDown(KeyCode.V))
            {
                entityContainer.playerCharacterEntity.TeleportPosition(sceneModel.bossTeleportPosition).Forget();
            }
        }

        private void UpdateTimeline()
        {
            if (pause)
            {
                return;
            }
            time += Time.deltaTime;
            var executedTrackCount = 0;
            for (int i = executeTimeline; i < queuedTracks.Count; ++i)
            {
                var track = queuedTracks[i];
                if (track.time < time)
                {
                    executedTrackCount++;
                    switch (track.timelineEventType)
                    {
                        case StageLevelTimelineEntity.TimelineEventType.Wave:
                            enemyCharacterFactory.CreateWavesAtPlayerPosition(track.waveCreateMarker);
                            break;
                        case StageLevelTimelineEntity.TimelineEventType.EnemyCreation:
                            enemyCharacterFactory.RefreshCreationData(track.monsterCreateMarker);
                            break;
                        case StageLevelTimelineEntity.TimelineEventType.BossBattle:
                            entityContainer.playerCharacterEntity.TeleportPosition(sceneModel.bossTeleportPosition).Forget();
                            break;
                    }
                }
            }

            executeTimeline += executedTrackCount;
        }
    }
}
