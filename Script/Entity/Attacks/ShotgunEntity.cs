using UnityEngine;

namespace Game.ECS
{
    public class ShotgunEntity : AttackEntity
    {
        protected override void Update()
        {
            base.Update();

            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
        }

        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }

        public override void OnApplyShootCountChanged()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.localPosition = playerCharacterEntity.translateImplement.positionProperty.statusValue;
            var characterDirection = playerCharacterEntity.translateImplement.moveDirectionProperty.statusValue.normalized;
            var characterDirectionAngle = Vector2.Angle(characterDirection, Vector2.right);
            if (characterDirection.y < 0f)
            {
                characterDirectionAngle *= -1f;
            }

            int shootNum = shootCounterImplement.maxShootCountProperty.statusValue;
            var angle = characterDirectionAngle;
            if (shootNum != 1)
            {
                float angleRange = GetAngle(weaponDataSetImplement.weaponDataSetProperty.statusValue.level);
                var startAngle = characterDirectionAngle - (angleRange * 0.5f);
                var angleOffset = angleRange / (shootNum - 1) * (shootCounterImplement.shootCountProperty.statusValue - 1);
                angle = startAngle + angleOffset;
            }
            var angleRadian = angle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
            translateImplement.moveDirectionProperty.statusValue = direction;
        }

        private static float GetAngle(int level) => level switch
        {
            1 => 40,
            2 => 50,
            3 => 60,
            4 => 70,
            5 => 80,
            6 => 90,
            _ => 90
        };
    }
}
