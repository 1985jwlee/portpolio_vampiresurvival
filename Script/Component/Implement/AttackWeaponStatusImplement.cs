using UnityEngine;

namespace Game.ECS
{
    public class AttackWeaponStatusImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private WeaponData weaponDataSet;
        public ref WeaponData weaponDataSetProperty => ref weaponDataSet;
        public void InitializeComponent()
        {
            
        }
    }
}
