using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class PoisonFlaskEntity : AttackEntity
    {
        [Inject] protected Camera mainCamera;

        public CircleCollider2D circleColliderImplement;
        public KnockbackToCenterImplement knockbackToCenterImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            TryGetComponent(out circleColliderImplement);
            if (TryGetComponent(out knockbackToCenterImplement))
            {
                Components.Add(knockbackToCenterImplement);
            }
        }

        public override void OnApplyShootCountChanged()
        {
            transformImplement.position = ExtensionFunction.RandomScreenPositionToWorld(mainCamera, 0f);
        }

        protected override void CheckAttackRange()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var sizeMultiply = playerCharacterEntity.characterStatusImplement.buffedattackSizeRatioProperty.statusValue;
            foreach (var buff in applyBuffImplement.applyBuffList)
            {
                if (buff.statusValue.buffType == BuffType.AttackArea)
                {
                    sizeMultiply *= buff.statusValue.buffValue;
                }
            }
            transformImplement.localScale = sizeMultiply * Vector3.one;
            knockbackToCenterImplement.radius = sizeMultiply * circleColliderImplement.radius;
        }
    }
}
