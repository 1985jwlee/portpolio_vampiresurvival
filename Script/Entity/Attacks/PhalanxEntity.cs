using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class PhalanxEntity : AttackEntity
    {
        public override void OnApplyShootCountChanged()
        {
            transformImplement.position = entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue;
            var maxAttack = shootCounterImplement.maxShootCountProperty.statusValue;
            var count = shootCounterImplement.shootCountProperty.statusValue;
            transformImplement.rotation = Quaternion.Euler(0f, 0f, GetAngle(maxAttack, count));
            transformImplement.position -= transformImplement.right * 1.2f;
        }

        private static float GetAngle(int maxCount, int currentCount)
        {
            return 360f / maxCount * currentCount;
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
            transformImplement.localScale = sizeMultiply * 1.8f * Vector3.one;
        }
    }
}
