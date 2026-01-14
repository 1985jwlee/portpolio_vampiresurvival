using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class ChakramEntity : AttackEntity
    {
        public ScreenBounceImplement screenBounceImplement;
        public WallBounceImplement wallBounceImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out screenBounceImplement))
            {
                Components.Add(screenBounceImplement);
            }
            if (TryGetComponent(out wallBounceImplement))
            {
                Components.Add(wallBounceImplement);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            
            wallBounceImplement.reactiveCommand.TakeUntilDisable(this)
                .Subscribe(bound =>
                {
                    var position = (Vector2)transformImplement.position;
                    var moveDir = translateImplement.moveDirectionProperty.statusValue;

                    if (position.x < bound.leftExtents || position.x > bound.rightExtents)
                    {
                        moveDir.x *= -1f;
                    }

                    if (position.y < bound.downExtents || position.y > bound.upExtents)
                    {
                        moveDir.y *= -1f;
                    }
                    
                    translateImplement.moveDirectionProperty.statusValue = moveDir;
                });
        }

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
            transformImplement.localPosition = playerCharacterEntity.transformImplement.localPosition;
            var angleRadian = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
            translateImplement.moveDirectionProperty.statusValue = direction;
        }
    }
}
