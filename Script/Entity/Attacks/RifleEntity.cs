using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class RifleEntity : AttackEntity
    {
        protected override void Update()
        {
            base.Update();
            
            MoveOneDirection(entityContainer, rigidBodyImplement, translateImplement);
            SyncRotateByMoveDirection(translateImplement, transformImplement);
        }

        public override void OnApplyShootCountChanged()
        {
                
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.position = playerCharacterEntity.transformImplement.localPosition;
            
            var count = shootCounterImplement.shootCountProperty.statusValue;
            var angle = GetAngle(count) + 45f;
            var angleRadian = angle * Mathf.Deg2Rad;
            
            translateImplement.moveDirectionProperty.statusValue = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        }
        
        private static float GetAngle(int count) => (count%4) switch
        {
            0 => 0,
            1 => 90,
            2 => 180,
            3 => 270,
            _ => 360
        };
        
        private static void MoveOneDirection(IEntityContainer entityContainer, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var addProjSpeedRatio = entityContainer.playerCharacterEntity.characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue * addProjSpeedRatio);
        }
        
        private static void SyncRotateByMoveDirection(TranslateImplement translateImpl, Transform transformImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            var angle = Vector2.Angle(direction, Vector2.right);
            if (direction.y < 0f)
            {
                angle *= -1f;
            }
            transformImpl.localRotation = Quaternion.Euler(0f, 0, angle);
        }
    }
}
