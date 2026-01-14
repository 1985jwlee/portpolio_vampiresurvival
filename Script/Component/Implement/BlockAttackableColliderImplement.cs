using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class BlockAttackableColliderImplement : MonoBehaviour, IComponent
    {
        [Inject] private IEntityContainer entityContainer;
        [Inject] private ISpearAttackSystem spearAttackSystem;

        public float blockAngle;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<AttackEntity>(out var attackEntity))
            {
                // 블록의 회전 기준은 Vector3.right 인데,
                // 공격과 블록이 서로 반대 방향일 때 막는 것이므로, 공격 방향의 역을 구하기위해 Vector3.left 에 대한 회전량을 구함.
                var attackQuaternion = Quaternion.FromToRotation(Vector3.left, attackEntity.rigidBodyImplement.velocity);
                var blockQuaternion = transform.rotation;

                var angle = Quaternion.Angle(attackQuaternion, blockQuaternion);

                if (angle < blockAngle)
                {
                    var applyBuffImpl = attackEntity.applyBuffImplement;

                    for (int i = 0; i < applyBuffImpl.applyBuffList.Count; ++i)
                    {
                        bool isAttackOfPlayer = applyBuffImpl.applyBuffList[i].statusValue.rootCharacter.statusValue == CharacterTypes.PlayerCharacter;
                        bool isDamageBuff = applyBuffImpl.applyBuffList[i].statusValue.buffType == BuffType.Damage;
                        if (isAttackOfPlayer && isDamageBuff)
                        {
                            var buffCopy = applyBuffImpl.applyBuffList[i];

                            var statusValueCopy = buffCopy.statusValue;
                            statusValueCopy.buffValue = statusValueCopy.buffValue * 0.5f;
                            buffCopy.statusValue = statusValueCopy;

                            applyBuffImpl.applyBuffList[i] = buffCopy;
                        }
                    }
                }
            }
        }

        public void InitializeComponent()
        {

        }
    }
}
