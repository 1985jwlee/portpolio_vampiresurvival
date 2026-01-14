using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class LightningWandEntity : AttackEntity
    {
        [Inject] protected Camera mainCamera;
        
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            SetAttackPoint();
        }

        protected virtual EnemyCharacterEntity SetAttackPoint()
        {
            var enemyEntities = entityContainer.GetEntities<EnemyCharacterEntity>();
            var list = new List<EnemyCharacterEntity>();
            foreach (var entity in enemyEntities)
            {
                var viewportPosition = mainCamera.WorldToViewportPoint(entity.translateImplement.positionProperty.statusValue);
                if (viewportPosition.x.IsWithin(0f, 1f) && viewportPosition.y.IsWithin(0f, 1f))
                {
                    list.Add(entity);
                }
            }

            if (list.Count > 0)
            {
                var enemy = list.Random();
                transformImplement.position = enemy.translateImplement.positionProperty.statusValue;
                return enemy;
            }
            transformImplement.position = ExtensionFunction.RandomScreenPositionToWorld(mainCamera, 0f);
            return null;
        }
    }
}
