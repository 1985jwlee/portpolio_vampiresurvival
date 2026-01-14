using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class RidiculousStaffEntity : GuillotineEntity
    {
        public WeaponCollsionImplement weaponCollisionImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            if (TryGetComponent(out weaponCollisionImplement))
            {
                Components.Add(weaponCollisionImplement);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();

            weaponCollisionImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(Bounce);
        }

        private void Bounce(Vector2 forceCenter)
        {
            var forceDirection = (Vector2)translateImplement.positionProperty.statusValue - forceCenter;
            forceDirection *= new Vector2(300f, 1500f);
            rigidBodyImplement.AddForce(forceDirection);
            rigidBodyImplement.AddTorque(Random.Range(-600f, 600f));
        }
    }
}
