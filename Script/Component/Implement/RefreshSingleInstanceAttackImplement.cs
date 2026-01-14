using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class RefreshSingleInstanceAttackImplement : MonoBehaviour, IComponent
    {
        
        [Inject] private IEntityContainer entityContainer;
        public string attackableId;
        public uint attackableEGID;


        public void RefreshAttackableDamage(string _attackableId, WeaponDataSet appliedBuff)
        {
            if (attackableId != _attackableId || !entityContainer.GetEntity(attackableEGID, out AttackEntity attackEntity)) {return;}

            {
                var buffData = new BuffData
                {
                    buffType = appliedBuff.attackType == AttackType.PHYSICS ? BuffType.Damage : BuffType.MagicDamage,
                    buffValue = appliedBuff.damage
                };
            
                var idx = attackEntity.applyBuffImplement.applyBuffList.FindIndex(_x => _x.statusValue.buffType == buffData.buffType);
                if (idx < 0 == false)
                {
                    var tmp = attackEntity.applyBuffImplement.applyBuffList[idx];
                    var value = tmp.statusValue;
                    value.buffValue = buffData.buffValue;
                    tmp.statusValue = value;
                    attackEntity.applyBuffImplement.applyBuffList[idx] = tmp;
                }
            }
            {
                
                
                var idx = attackEntity.applyBuffImplement.applyBuffList.FindIndex(_x => _x.statusValue.buffType == BuffType.AttackArea);
                if (idx < 0 == false)
                {
                    var tmp = attackEntity.applyBuffImplement.applyBuffList[idx];
                    var value = tmp.statusValue;
                    value.buffValue = appliedBuff.area;
                    tmp.statusValue = value;
                    attackEntity.applyBuffImplement.applyBuffList[idx] = tmp;
                }
            }
        }

        public void InitializeComponent()
        {
            
        }
    }
}
