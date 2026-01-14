using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class GuillotineEntity : AttackEntity
    {
        [Inject] protected Camera mainCam;

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            
            var viewportToWorld = mainCam.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.9f), 1.1f));
            transformImplement.position = new Vector3(viewportToWorld.x, viewportToWorld.y, 0f);
            rigidBodyImplement.gravityScale = 2f;
        }
    }
}
