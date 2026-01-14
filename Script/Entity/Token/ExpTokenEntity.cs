using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class ExpTokenEntity : TokenEntity
    {
        
        public SeekingTargetImplement seekingTargetImplement;
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            seekingTargetImplement = GetComponent<SeekingTargetImplement>();
            Components.Add(seekingTargetImplement);
            
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }
        
        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            seekingTargetSystem.UnRegistComponent(this);
        }
    }
}
