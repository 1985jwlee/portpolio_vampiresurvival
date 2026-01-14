using Reflex.Scripts.Attributes;

namespace Game.ECS
{
    public class ShielderEnemyCharacterEntity : SeekTargetEnemyCharacterEntity
    {
        [Inject] private SummonObjectFactory summonObjectFactory;
        public LinkedEgidImplement linkedEgidImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            linkedEgidImplement = GetComponent<LinkedEgidImplement>();

            Components.Add(linkedEgidImplement);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = entityContainer.playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            seekingTargetImplement.seekingTargetProperty.seekingTime = 4f;

            var summonObject = summonObjectFactory.CreateGameObject($"Prefabs/Summon/EnemyShield");
            var shieldEntity = summonObject.GetComponent<ShieldEntity>();
            shieldEntity.OwnerEGID = EGID;
            shieldEntity.seekingTargetImplement.seekingTargetProperty.seekTargetEGID = EGID;
            shieldEntity.seekingTargetImplement.seekingTargetProperty.seekingTime = -1f;

            linkedEgidImplement.linkedEgidProperty.statusValue = shieldEntity.EGID;
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();

            if (entityContainer.GetEntity(linkedEgidImplement.linkedEgidProperty.statusValue, out ShieldEntity indicatorEntity))
            {
                summonObjectFactory.EnqueRecycle(indicatorEntity, indicatorEntity.SrcPathHashCode);
            }
        }
    }
}
