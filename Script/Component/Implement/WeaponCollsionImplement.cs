using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class WeaponCollsionImplement : MonoBehaviour, IComponent, IReactiveCommandComponent<Vector2>
    {
        public IReactiveCommand<Vector2> reactiveCommand => colliderPosition;
        private ReactiveCommand<Vector2> colliderPosition = new ReactiveCommand<Vector2>();
        
        public void InitializeComponent()
        {
            if (colliderPosition != null)
            {
                colliderPosition.Dispose();
            }

            colliderPosition = new ReactiveCommand<Vector2>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out EnemyCharacterEntity character))
            {
                colliderPosition.Execute(character.translateImplement.positionProperty.statusValue);
            }
        }
    }
}
