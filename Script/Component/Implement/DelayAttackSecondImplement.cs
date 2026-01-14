using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DelayAttackSecondImplement : MonoBehaviour, IComponent, ITickComponent, IReactiveCommandComponent<Unit>
    {
        
        [SerializeField] private float delayTime;
        public bool enableDelay = true;
        public float fixedTickTime => delayTime;
        public float currentTickTime { get; set; }
        public IReactiveCommand<Unit> reactiveCommand => onDelayFinished;
        private ReactiveCommand<Unit> onDelayFinished = new ReactiveCommand();
        
        public void InitializeComponent()
        {
            currentTickTime = 0f;
            if (onDelayFinished != null)
            {
                onDelayFinished.Dispose();
            }

            onDelayFinished = new();
        }

        public bool OnScanTime()
        {
            currentTickTime += Time.deltaTime;
            if (currentTickTime > fixedTickTime)
            {
                currentTickTime -= fixedTickTime;
                onDelayFinished.Execute(Unit.Default);
                return true;
            }
            return false;
        }

        private void Update()
        {
            if (enableDelay == false)
            {
                return;
            }
            OnScanTime();
        }

        
    }
}
