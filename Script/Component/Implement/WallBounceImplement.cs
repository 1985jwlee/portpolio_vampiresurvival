using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class WallBounceImplement : MonoBehaviour, IComponent, IReactiveCommandComponent<Bounds2D>
    {
        public IReactiveCommand<Bounds2D> reactiveCommand => bounds;

        private ReactiveCommand<Bounds2D> bounds = new ReactiveCommand<Bounds2D>();


        public void InitializeComponent()
        {
            if (bounds != null)
            {
                bounds.Dispose();    
            }
            bounds = new ReactiveCommand<Bounds2D>();
        }

        
    }
}
