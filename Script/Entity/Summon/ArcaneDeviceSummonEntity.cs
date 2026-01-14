using System.Collections.Generic;

namespace Game.ECS
{
    public class ArcaneDeviceSummonEntity : SummonEntity
    {
        public WeaponInventoryImplement weaponInventoyImplement;
        public TableIndexDataImplement tableIndexDataImplement;
        
        protected readonly List<WeaponDataSet> weaponSets = new List<WeaponDataSet>();

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            if (TryGetComponent(out weaponInventoyImplement))
            {
                Components.Add(weaponInventoyImplement);
            }

            if (TryGetComponent(out tableIndexDataImplement))
            {
                Components.Add(tableIndexDataImplement);
            }
        }
    }
}
