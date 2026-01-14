using System;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class WallObjectCollisionImplement : MonoBehaviour
    {
        [Inject] private IEntityContainer entityContainer;
        private Bounds2D boxBounds;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            boxBounds = new Bounds2D(col.bounds.center, col.bounds.size);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<AttackEntity>(out var entity))
            {
                if (entityContainer.GetEntity<AttackEntity, WallBounceImplement>(entity.EGID, out _, out var wallBounce))
                {
                    wallBounce.reactiveCommand.Execute(boxBounds);
                }
            }
        }
    }
}
