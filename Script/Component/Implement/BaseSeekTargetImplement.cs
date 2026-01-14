using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public abstract class BaseSeekTargetImplement : MonoBehaviour, IComponent, ITickComponent, IReactiveCommandComponent<Unit>
    {
        public abstract IReactiveCommand<Unit> reactiveCommand { get; }
        public abstract float fixedTickTime { get; }
        public float currentTickTime { get; set; }
        
        public virtual void InitializeComponent()
        {
            currentTickTime = 0f;
        }

        public abstract bool OnScanTime();
    }
}
