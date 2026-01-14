using UnityEngine;

namespace Game.ECS
{
    public class HighElvenSwordEntity : AttackEntity
    {
        public GameObject rotateChild;
        
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.position = playerCharacterEntity.translateImplement.positionProperty.statusValue;
        }
        
        protected override void Update()
        {
            base.Update();
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
        }
        
        public override void OnApplyShootCountChanged()
        {
            var randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            var randomDirectionAngle = Vector2.Angle(randomDirection, Vector2.right);
            if (randomDirection.y < 0f)
            {
                randomDirectionAngle *= -1f;
            }
            
            var angleRadian = randomDirectionAngle * Mathf.Deg2Rad;
            translateImplement.moveDirectionProperty.statusValue = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
            rotateChild.transform.localRotation = Quaternion.Euler(0f, 0f, randomDirectionAngle - 50f);
        }
        
        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }
    }
}
