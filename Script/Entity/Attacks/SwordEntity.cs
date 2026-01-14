using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class SwordEntity : AttackEntity
    {
        [Inject] protected Camera mainCamera;
        public List<GameObject> randomChilds;
        private static Vector2 randomPosition { get; set; }
        

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            for (int i = 0; i < transformImplement.childCount; ++i)
            {
                var go = transformImplement.GetChild(i).gameObject;
                randomChilds.Add(go);
                go.SetActive(false);
            }
        }

        public override void OnApplyShootCountChanged()
        {
            SetRandomPosition();
        }

        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
        }

        protected virtual void SetRandomPosition()
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

        protected void CheckAttackRange(Vector2 position)
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

            var pos = ExtensionFunction.RandomCirclePosition(position, sizeMultiply * 0.1f);
            transformImplement.position = pos;
            var go =  randomChilds[Random.Range(0, 2)];
            go.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            transformImplement.localScale = Vector3.one * sizeMultiply;
            
            go.SetActive(true);
        }

        protected override void CheckAttackRange()
        {
            CheckAttackRange(randomPosition);
        }
    }
}
