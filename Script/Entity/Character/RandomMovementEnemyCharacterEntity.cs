using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class RandomMovementEnemyCharacterEntity : EnemyCharacterEntity
    {
        public RandomMovementImplement randomMovementImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            randomMovementImplement = GetComponent<RandomMovementImplement>();

            Components.Add(randomMovementImplement);
        }

        protected override void Update()
        {
            base.Update();
            UpdateRandomMovement(randomMovementImplement, knockBackReceiveImplement, rigidBodyImplement, translateImplement);
        }

        private static void UpdateRandomMovement(RandomMovementImplement randomMovementImpl, KnockBackReceiveImplement knockBackReceiveImpl, Rigidbody2D rigidbodyImpl, TranslateImplement translateImpl)
        {
            if(knockBackReceiveImpl.knockBackReceiveProperty.statusValue.isKnockBacking == false)
            {
                if (randomMovementImpl.MoveTimerProperty.statusValue > 0)
                {
                    rigidbodyImpl.velocity = randomMovementImpl.MoveDirectionProperty.statusValue * translateImpl.velocityProperty.statusValue;
                    randomMovementImpl.MoveTimerProperty.statusValue -= Time.deltaTime;
                }
                else if (randomMovementImpl.WaitTimerProperty.statusValue > 0)
                {
                    randomMovementImpl.WaitTimerProperty.statusValue -= Time.deltaTime;
                    rigidbodyImpl.velocity = Vector2.zero;
                }
                else
                {
                    randomMovementImpl.MoveDirectionProperty.statusValue = Random.insideUnitCircle;
                    randomMovementImpl.MoveTimerProperty.statusValue = randomMovementImpl.MoveDurationProperty.statusValue;
                    randomMovementImpl.WaitTimerProperty.statusValue = randomMovementImpl.WaitDurationProperty.statusValue;
                }
            }
        }
    }
}
