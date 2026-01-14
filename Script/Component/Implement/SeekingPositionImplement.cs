using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class SeekingPositionImplement : BaseSeekTargetImplement
    {
        [SerializeField] private SeekingPositionComponent seekingTarget;
        private ReactiveCommand<Unit> onScanTime;
        
        public override float fixedTickTime => seekingTarget.seekingTime;
        
        public ref SeekingPositionComponent seekingTargetProperty => ref seekingTarget;
        public override IReactiveCommand<Unit> reactiveCommand => onScanTime;
        
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            if(onScanTime != null){ onScanTime.Dispose();}
            onScanTime = new ReactiveCommand<Unit>();
        }

        public override bool OnScanTime()
        {
            if (seekingTargetProperty.startSeek == false)
            {
                currentTickTime = 0f;
                return false;
            }
            currentTickTime += Time.deltaTime;
            if (currentTickTime > seekingTargetProperty.seekingTime)
            {
                currentTickTime -= seekingTargetProperty.seekingTime;
                onScanTime.Execute(new Unit());
                return true;
            }
            return false;
        }
    }
}
