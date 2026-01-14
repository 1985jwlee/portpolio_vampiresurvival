namespace Game.ECS
{
    public class RestorationCircleEntity : HolyStaffEntity
    {
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            var characterEntity = entityContainer.playerCharacterEntity;
            entityContainer.playerCharacterEntity.characterStatusImplement.hitpointRecoveryRateProperty.statusValue += 0.05f;
            characterEntity.OnApplyPassiveArcaneDevice();
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            var characterEntity = entityContainer.playerCharacterEntity;
            entityContainer.playerCharacterEntity.characterStatusImplement.hitpointRecoveryRateProperty.statusValue -= 0.05f;
            characterEntity.OnApplyPassiveArcaneDevice();
        }
    }
}
