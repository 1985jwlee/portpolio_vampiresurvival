using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class CaliburnEntity : SwordEntity
    {
        private static Vector2 randomPosition { get; set; }

        protected override void SetRandomPosition()
        {
            var count = shootCounterImplement.shootCountProperty.statusValue;
            if (count > 1 == false)
            {
                var allEnemy = entityContainer.GetEntities<EnemyCharacterEntity>();
                var playerToEnemyDistance = (uint.MaxValue, float.MaxValue);
                foreach (var entity in allEnemy)
                {
                    var distance = Vector3.Distance(entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue, entity.translateImplement.positionProperty.statusValue);

                    if (playerToEnemyDistance.Item2 > distance)
                    {
                        playerToEnemyDistance = (entity.EGID, distance);
                    }
                }

                if (entityContainer.GetEntity(playerToEnemyDistance.Item1, out EnemyCharacterEntity o))
                {
                    randomPosition = o.translateImplement.positionProperty.statusValue;
                }
                else
                {
                    randomPosition = ExtensionFunction.RandomScreenPositionToWorld(mainCamera, 0f);
                }
            }
            CheckAttackRange();
        }

        protected override void CheckAttackRange()
        {
            CheckAttackRange(randomPosition);
        }
    }
}
