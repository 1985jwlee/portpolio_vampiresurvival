using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class AmbushEnemyCharacterEntity : SeekTargetEnemyCharacterEntity
    {
        public AmbushEnemyPatternImplement patternImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            patternImplement = GetComponent<AmbushEnemyPatternImplement>();
            Components.Add(patternImplement);
        }
        protected override void Update()
        {
            base.Update();
            UpdatePattern();
        }

        public void UpdatePattern()
        {
            UpdatePattern(patternImplement, transformImplement, seekingTargetImplement, knockBackReceiveImplement, rigidBodyImplement);

            static void UpdatePattern(AmbushEnemyPatternImplement patternImpl, Transform transformImpl, SeekingTargetImplement seekingTargetImpl, KnockBackReceiveImplement knockBackReceiveImplement, Rigidbody2D rigidbodyImpl)
            {
                Vector2 selfPosition = transformImpl.position;

                switch (patternImpl.StateProperty.statusValue)
                {
                    case AmbushState.Waiting:
                        {
                            var colliders = Physics2D.OverlapCircleAll(selfPosition, patternImpl.DetectionRadiusProperty.statusValue, GameSettings.playerLayerMask);
                            foreach(var collider in colliders)
                            {
                                if (collider.TryGetComponent<PlayerCharacterEntity>(out var characterEntity))
                                {
                                    patternImpl.StateProperty.statusValue = AmbushState.Chasing;
                                    break;
                                }
                            }
                            
                            seekingTargetImpl.seekingTargetProperty.startSeek = false;
                            if (knockBackReceiveImplement.knockBackReceiveProperty.statusValue.isKnockBacking == false)
                                rigidbodyImpl.velocity = Vector2.zero;
                        }
                        break;
                    case AmbushState.Chasing:
                        {
                            seekingTargetImpl.seekingTargetProperty.startSeek = true;
                        }
                        break;
                    default:
                        {
                            seekingTargetImpl.seekingTargetProperty.startSeek = false;
                        }
                        break;
                }
            }
        }

    }
}
