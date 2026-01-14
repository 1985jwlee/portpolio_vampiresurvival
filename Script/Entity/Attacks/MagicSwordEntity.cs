using UnityEngine;

namespace Game.ECS
{
    public class MagicSwordEntity : AttackEntity
    {
        public static Vector2 randomDirection;
        public GameObject rotateChild;

        protected override void Update()
        {
            base.Update();
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.position = playerCharacterEntity.translateImplement.positionProperty.statusValue;
        }

        public override void OnApplyShootCountChanged()
        {
            var count = shootCounterImplement.shootCountProperty.statusValue;
            if (count > 1 == false)
            {
                randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
            
            var randomDirectionAngle = Vector2.Angle(randomDirection, Vector2.right);
            if (randomDirection.y < 0f)
            {
                randomDirectionAngle *= -1f;
            }
            int shootNum = shootCounterImplement.maxShootCountProperty.statusValue;
            var angle = randomDirectionAngle;
            
            if (shootNum > 1)
            {
                float angleRange = GetAngle(shootNum);
                var startAngle = randomDirectionAngle - (angleRange * 0.5f);
                var angleOffset = angleRange / (shootNum - 1) * (shootCounterImplement.shootCountProperty.statusValue - 1);
                angle = startAngle + angleOffset;
            }
            var angleRadian = angle * Mathf.Deg2Rad;
            rotateChild.transform.localRotation = Quaternion.Euler(0f, 0f, angle - 50f);
            translateImplement.moveDirectionProperty.statusValue = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        }
        
        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }
        
        private static float GetAngle(int count) => count switch
        {
            1 => 0,
            2 => 20,
            3 => 30,
            4 => 40,
            _ => 40
        };
        
        
    }
}
