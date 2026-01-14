using UnityEngine;

namespace Game.ECS
{
    public class SpearEntity : AttackEntity
    {
        public override void OnApplyShootCountChanged()
        {
            RotateByStep(entityContainer, transformImplement, shootCounterImplement.shootCountProperty.statusValue);
        }

        private static void RotateByStep(IEntityContainer entityContainer, Transform transformImpl, int step)
        {
            step -= 1;
            var offsetStep = step / 4;
            var offset = Vector3.up * (offsetStep * 0.1f);
            var characterEntity = entityContainer.playerCharacterEntity;
            var srcPosition = characterEntity.translateImplement.positionProperty.statusValue;
            transformImpl.position = srcPosition;
            switch (step % 4)
            {
                case 0:
                    transformImpl.localRotation = Quaternion.identity;
                    break;
                case 1:
                    transformImpl.localRotation = Quaternion.Euler(0f, 0f, 180f);
                    break;
                case 2:
                    transformImpl.localRotation = Quaternion.Euler(0f, 0f, 270f);
                    break;
                case 3:
                    transformImpl.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
            }
            transformImpl.position -= transformImpl.right * 0.5f;
            var localpos = transformImpl.localPosition;
            localpos += offset;
            transformImpl.localPosition = localpos;
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

            transformImplement.localScale = sizeMultiply * 2.0f * Vector3.one;
        }
    }
}
