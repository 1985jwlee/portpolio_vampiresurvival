using System;
using System.Linq;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class KnockbackToCenterImplement : MonoBehaviour, IComponent, ITickComponent
    {
        [Inject] private IEntityContainer entityContainer;
        public float radius;
        private TranslateImplement translateImplement;
        
        public float fixedTickTime => 0.5f;
        public float currentTickTime { get; set; }
        
        public void InitializeComponent()
        {
            currentTickTime = 0f;
            TryGetComponent(out translateImplement);
        }

        
        public bool OnScanTime()
        {
            currentTickTime += Time.deltaTime;

            if (currentTickTime > fixedTickTime)
            {
                
                var enemyInDistance = entityContainer.GetEntities<EnemyCharacterEntity>().Where(_x =>
                    Vector2.Distance(_x.translateImplement.positionProperty.statusValue, translateImplement.positionProperty.statusValue) < radius).ToList();

                if (enemyInDistance.Count > 0 == false)
                {
                    return false;
                }
                
                currentTickTime -= fixedTickTime;
                TryGetComponent(out AttackEntity myAttack);

                foreach (var enemy in enemyInDistance)
                {
                    var enemyPosition = enemy.translateImplement.positionProperty.statusValue;
                    enemy.knockBackReceiveImplement.knockBackReceiveProperty.statusValue = new KnockBackReceiveDataSet()
                    {
                        isKnockBacking = true, remainTime = GameSettings.DefaultKnockBackDuration, 
                        direction = ((Vector2)myAttack.translateImplement.positionProperty.statusValue - (Vector2)(enemyPosition)).normalized,
                        velocity = GameSettings.DefaultKnockBackVelocity * 0.2f
                    };
                }
                return true;
            }
            return false;
        }

        private void Update()
        {
            OnScanTime();
        }
    }
}
