using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DoubleEdgeSwordEntity : AttackEntity
    {
        public DecreaseSpeedImplement decreaseSpeedImplement;
        public Transform childTransformImplment;
        private float rotateMultiplier;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out decreaseSpeedImplement))
            {
                Components.Add(decreaseSpeedImplement);    
            }
        }
        
        

        protected override void Update()
        {
            base.Update();

            decreaseSpeedImplement.DecreaseSpeed();
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
            RotateObject(childTransformImplment, 600f * rotateMultiplier);
        }
        
        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var playerToEnemyDistance = (uint.MaxValue, float.MaxValue);
            var allEnemy = entityContainer.GetEntities<EnemyCharacterEntity>();
            var characterPosition = playerCharacterEntity.translateImplement.positionProperty.statusValue;
            
            
            foreach (var entity in allEnemy)
            {
                var distance = Vector3.Distance(characterPosition, entity.translateImplement.positionProperty.statusValue);

                if (playerToEnemyDistance.Item2 > distance)
                {
                    playerToEnemyDistance = (entity.EGID, distance);
                }
            }
            
            transformImplement.position = characterPosition;
            
            if (entityContainer.GetEntity(playerToEnemyDistance.Item1, out EnemyCharacterEntity o))
            {
                translateImplement.moveDirectionProperty.statusValue = (o.translateImplement.positionProperty.statusValue - characterPosition).normalized;
            }
            else
            {
                translateImplement.moveDirectionProperty.statusValue = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }

            
            
            childTransformImplment = spriterendererImplement.transform;
            childTransformImplment.transform.localRotation = Quaternion.identity;
        }

        public override void OnApplyShootCountChanged()
        {
            decreaseSpeedImplement.InitializeValue(translateImplement.velocityProperty.statusValue * translateImplement.moveDirectionProperty.statusValue);
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rotateMultiplier = (translateImplement.velocityProperty.statusValue * addProjSpeedRatio);
            
            decreaseSpeedImplement.reactiveProperty
                .TakeUntilDisable(this)
                .Subscribe(vec =>
                {
                    translateImplement.moveDirectionProperty.statusValue = vec.normalized;
                    translateImplement.velocityProperty.statusValue = vec.magnitude;
                });
        }
    }
}
