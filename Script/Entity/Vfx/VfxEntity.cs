using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class VfxEntity : Entity
    {
        [Inject] VfxFactory vfxFactory;
        [Inject] ISeekingTargetSystem seekingTargetSystem;

        public Transform transformImplement;
        public SeekingTargetImplement seekingTargetImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            transformImplement = transform;

            if(TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
                seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
            }
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();

            if(seekingTargetImplement != null)
            {
                seekingTargetImplement.reactiveCommand
                    .TakeUntilDisable(this)
                    .Subscribe(_ =>
                    {
                        if (entityContainer.GetEntity<CharacterEntity>(seekingTargetImplement.seekingTargetProperty.seekTargetEGID, out var characterEntity))
                        {
                            transformImplement.localPosition = characterEntity.translateImplement.positionProperty.statusValue;
                        }
                    });
            }
        }
    }
}
