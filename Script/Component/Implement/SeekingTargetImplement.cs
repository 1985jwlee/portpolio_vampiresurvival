using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class SeekingTargetImplement : BaseSeekTargetImplement
    {
        [SerializeField] private SeekingTargetComponent seekingTarget;
        private ReactiveCommand<Unit> onScanTime;
        public override IReactiveCommand<Unit> reactiveCommand => onScanTime;
        public ref SeekingTargetComponent seekingTargetProperty => ref seekingTarget;

        public override float fixedTickTime => seekingTargetProperty.seekingTime;
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
        
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            if(onScanTime != null){ onScanTime.Dispose();}
            onScanTime = new ReactiveCommand<Unit>();
        }
        
        public void SetTimerFullCharge()
        {
            currentTickTime = seekingTargetProperty.seekingTime;
        }
    }
}
