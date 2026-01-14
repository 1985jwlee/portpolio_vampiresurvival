using UnityEngine;

namespace Game.ECS
{
    public class EnemyStraightToPlayerProjectileEntity : AttackEntity
    {
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out spearAttackImplement))
            {
                Components.Add(spearAttackImplement);
            }
        }

        public override void InitWithRootCharacterEGID(uint EGID)
        {
            base.InitWithRootCharacterEGID(EGID);
            entityContainer.GetEntity<CharacterEntity>(EGID, out var characterEntity);
            translateImplement.moveDirectionProperty.statusValue = (entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue - characterEntity.translateImplement.positionProperty.statusValue).normalized;

            transformImplement.localPosition = characterEntity.transformImplement.localPosition;
        }

        protected override void Update()
        {
            base.Update();
            MoveOneDirection(rigidBodyImplement, translateImplement);
            SyncRotateByMoveDirection(translateImplement, transformImplement);
        }

        private static void MoveOneDirection(Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            var direction = translateImpl.moveDirectionProperty.statusValue;
            rigidbodyImpl.velocity = direction * (translateImpl.velocityProperty.statusValue);
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
