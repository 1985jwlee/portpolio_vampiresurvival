using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class BowEntity : AttackEntity
    {
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out spearAttackImplement))
            {
                Components.Add(spearAttackImplement);
            }
        }
        
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var playerToEnemyDistance = (uint.MaxValue, float.MaxValue);
            var allEnemy = entityContainer.GetEntities<EnemyCharacterEntity>();
            transformImplement.position = playerCharacterEntity.transformImplement.position;
            
            if (allEnemy.Count > 0)
            {
                foreach (var entity in allEnemy)
                {
                    var distance = Vector3.Distance(playerCharacterEntity.translateImplement.positionProperty.statusValue, entity.translateImplement.positionProperty.statusValue);

                    if (playerToEnemyDistance.Item2 > distance)
                    {
                        playerToEnemyDistance = (entity.EGID, distance);
                    }
                }
                if (entityContainer.GetEntity(playerToEnemyDistance.Item1, out EnemyCharacterEntity o))
                {
                    translateImplement.moveDirectionProperty.statusValue = (o.translateImplement.positionProperty.statusValue - playerCharacterEntity.translateImplement.positionProperty.statusValue).normalized;
                }
            }
            else
            {
                translateImplement.moveDirectionProperty.statusValue = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
        }

        protected override void Update()
        {
            base.Update();
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
            SyncRotateByMoveDirection(translateImplement, transformImplement);
        }
        
        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity =  direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }


        private static void SyncRotateByMoveDirection(TranslateImplement translateImpl, Transform transformImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var angle = Vector2.Angle(direction, Vector2.right);
            if (direction.y < 0f)
            {
                angle *= -1f;
            }
            transformImpl.rotation = Quaternion.Euler(0f, 0, angle);
        }
    }
}
