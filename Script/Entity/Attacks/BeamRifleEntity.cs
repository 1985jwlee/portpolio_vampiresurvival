using UnityEngine;

namespace Game.ECS
{
    public class BeamRifleEntity : AttackEntity
    {
        public override void OnApplyShootCountChanged()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.position = playerCharacterEntity.translateImplement.positionProperty.statusValue;
            transformImplement.rotation = Quaternion.Euler(0f, 0f, GetAngle(shootCounterImplement.shootCountProperty.statusValue) + 45f);
        }
        
        private static float GetAngle(int count) => (count%4) switch
        {
            0 => 0,
            1 => 90,
            2 => 180,
            3 => 270,
            _ => 360
        };
    }
}
