using Reflex.Scripts.Attributes;
using UniRx;

namespace Game.ECS
{
    public class BossEnemyCharacterEntity : EnemyCharacterEntity
    {
        [Inject] protected TimelineManager timelineManager;
        [Inject] protected MainGameSceneContextModel mainGameSceneContextModel;

        public  BossImplement bossImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            bossImplement = GetComponent<BossImplement>();
            Components.Add(bossImplement);
        }

        #region System

        protected override void Update()
        {
            base.Update();
            HandleBossDeath(bossImplement, statusImplement, timelineManager, mainGameSceneContextModel);
        }

        private static void HandleBossDeath(BossImplement bossImpl, CommonStatusImplement statusImpl, TimelineManager timelineManager, MainGameSceneContextModel mainGameSceneContextModel)
        {
            if (statusImpl.hitPointProperty.statusValue <= 0 && bossImpl.IsDeadProperty.statusValue == false)
            {
                bossImpl.IsDeadProperty.statusValue = true;
                timelineManager.pause = false;
                if (bossImpl.IsFinalBossProperty.statusValue)
                {
                    MessageBroker.Default.Publish("gameClear");
                }
                else
                {
                    mainGameSceneContextModel.bossIconPositions.Remove(bossImpl.BossIconPositionProperty.statusValue);
                }
            }
        }
        #endregion
    }
}
