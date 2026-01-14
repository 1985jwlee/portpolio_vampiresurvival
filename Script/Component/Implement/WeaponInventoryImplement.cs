using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public interface IWeaponCoolTimePause : IStatusValue<bool> { };

    public struct WeaponCoolTimePause : IWeaponCoolTimePause
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    [SerializeField]
    public class WeaponInventoryImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private WeaponCoolTimePause weaponCoolTimePause;
        [SerializeField] private List<WeaponData> weaponDatas = new List<WeaponData>();
        [Inject] private IEntityContainer entityContainer;
        [Inject] private WeaponFactory weaponFactory;

        public IReadOnlyList<WeaponData> WeaponDatas => weaponDatas;
        public ref WeaponCoolTimePause WeaponCoolTimePauseProperty => ref weaponCoolTimePause;

        public void AddOrUpdateWeapon(WeaponData newData)
        {
            var index = weaponDatas.FindIndex(elmt => elmt.statusValue.id == newData.statusValue.id);
            var value = newData.statusValue;
            value.coolTime += (value.createCount-1) * value.multiCreationTick;
            value.coolDown = value.isSingleCreation ? value.coolTime : 0f;
            
            newData.statusValue = value;
            if(index == -1)
            {
                weaponDatas.Add(newData);
            }
            else
            {
                if (value.isSingleCreation)
                {
                    var singleCreation = entityContainer.GetEntities<AttackEntity>()
                        .Where(_x => _x.refreshSingleAttackImplement != null)
                        .Select(_x => _x.refreshSingleAttackImplement);

                    foreach (var entity in singleCreation)
                    {
                        entity.RefreshAttackableDamage(newData.statusValue.id, newData.statusValue);
                    }
                }

                weaponDatas[index] = newData;
            }
        }

        public bool RemoveWeapon(string weaponId)
        {
            var index = weaponDatas.FindIndex(elmt => elmt.statusValue.id == weaponId);
            if (index != -1)
            {
                if (weaponDatas[index].statusValue.isSingleCreation)
                {
                    var singleCreation = entityContainer.GetEntities<AttackEntity>()
                        .Where(_x => _x.refreshSingleAttackImplement != null)
                        .Where(_x => _x.refreshSingleAttackImplement.attackableId == weaponDatas[index].statusValue.id);

                    foreach (var entity in singleCreation)
                    {
                        weaponFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                    }
                }

                weaponDatas.RemoveAt(index);

                return true;
            }

            return false;
        }

        public void ClearWeaponList()
        {
            weaponDatas.Clear();
        }
        
        public void CheckWeaponCooldown(float cooldownBuff)
        {
            for (int i = 0; i < weaponDatas.Count; ++i)
            {
                var tmp = weaponDatas[i];
                
                var tmpValue = tmp.statusValue;
                if (tmpValue.isCooldownCharging)
                {
                    continue;
                }
                tmpValue.coolDown += Time.deltaTime;
                if (tmpValue.coolDown > tmpValue.coolTime * cooldownBuff)
                {
                    tmpValue.isCooldownCharging = true;
                    tmpValue.coolDown = 0f;
                }

                tmp.statusValue = tmpValue;
                weaponDatas[i] = tmp;
            }
        }


        public void FireWeapon(out List<WeaponDataSet> output)
        {
            output = new List<WeaponDataSet>();
            for (int i = 0; i < weaponDatas.Count; ++i)
            {
                var tmp = weaponDatas[i];
                var tmpValue = weaponDatas[i].statusValue;
                if (tmpValue.markSingleCreation)
                {
                    continue;
                }
                
                if (tmpValue.isCooldownCharging)
                {
                    if (tmpValue.isSingleCreation == false)
                    {
                        tmpValue.isCooldownCharging = false;
                    }
                    else
                    {
                        tmpValue.markSingleCreation = true;
                    }
                    tmp.statusValue = tmpValue;
                    weaponDatas[i] = tmp;
                    output.Add(tmpValue);
                }
            }
        }

        public void InitializeComponent()
        {
            
        }
    }
}
