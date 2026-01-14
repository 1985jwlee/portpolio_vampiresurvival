using UnityEngine;

namespace Game.ECS
{
    public class SuicideBombingEntity : AttackEntity
    {
        protected override void CheckAttackRange()
        {
            var sizeMultiply = 1f;
            foreach (var buff in applyBuffImplement.applyBuffList)
            {
                if (buff.statusValue.buffType == BuffType.AttackArea)
                {
                    sizeMultiply *= buff.statusValue.buffValue;
                }
            }

            transformImplement.localScale = Vector3.one * sizeMultiply;
        }
    }
}
