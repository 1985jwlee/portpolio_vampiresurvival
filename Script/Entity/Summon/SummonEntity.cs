using UnityEngine;

namespace Game.ECS
{
    public class SummonEntity : Entity
    {
        public uint OwnerEGID { get; set; }
        
        public Transform transformImplement;
        public TranslateImplement translateImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            
            transformImplement = transform;
            if (TryGetComponent(out translateImplement))
            {
                Components.Add(translateImplement);
            }
        }
        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
        }
        
        public virtual void OnApplyEntityComponent()
        {
            
        }
        
        
        protected static void SyncTransform(Transform transformImpl, TranslateImplement translateImple)
        {
            translateImple.positionProperty.statusValue = transformImpl.localPosition;
            translateImple.rotationProperty.statusValue = transformImpl.localRotation;
            translateImple.scaleProperty.statusValue = transformImpl.localScale;
        }

    }
}
