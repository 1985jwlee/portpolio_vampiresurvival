using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class ArcaneVeilEntity : ArcaneCycleEntity
    {
        public override void OnApplyShootCountChanged()
        {
            var index = shootCounterImplement.shootCountProperty.statusValue;
            var count = shootCounterImplement.maxShootCountProperty.statusValue;
            srcArcaneChild.transform.localPosition = GetChildLocalPosition(count, index - 1, 3.0f);
        }
    }
}
