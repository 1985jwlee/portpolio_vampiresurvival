using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.ECS
{
    public class EnemyAttackIndicatorEntity : Entity
    {
        public SpriteRenderer background;
        public SpriteRenderer fillSprite;
        public Transform transformImplement;
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            transformImplement = GetComponent<Transform>();
            background = GetComponent<SpriteRenderer>();
            fillSprite = transform.Find("fillSprite").GetComponent<SpriteRenderer>();
        }
    }
}
