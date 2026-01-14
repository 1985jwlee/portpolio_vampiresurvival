using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class OneDirectionEnemyCharacterEntity : EnemyCharacterEntity
    {
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            translateImplement.moveDirectionProperty.statusValue = (playerCharacterEntity.translateImplement.positionProperty.statusValue - translateImplement.positionProperty.statusValue).normalized;
        }

        protected override void Update()
        {
            base.Update();
            MoveOneDirection(rigidBodyImplement, translateImplement);
        }

        private static void MoveOneDirection(Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            rigidbodyImpl.velocity = translateImpl.velocityProperty.statusValue * translateImpl.moveDirectionProperty.statusValue;
        }
    }
}
