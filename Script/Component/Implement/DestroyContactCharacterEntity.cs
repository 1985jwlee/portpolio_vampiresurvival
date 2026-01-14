using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DestroyContactCharacterEntity : MonoBehaviour, IComponent, IReactiveCommandComponent<Unit>
    {
        public IReactiveCommand<Unit> reactiveCommand => onContanctCharacterEntity;
        private ReactiveCommand<Unit> onContanctCharacterEntity = new ReactiveCommand<Unit>();
        
        public void InitializeComponent()
        {
            if (onContanctCharacterEntity != null)
            {
                onContanctCharacterEntity.Dispose();
            }
            onContanctCharacterEntity = new ReactiveCommand<Unit>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var e = other.GetComponent<CharacterEntity>();
            if (e != null)
            {
                onContanctCharacterEntity.Execute(default);
            }
        }
    }
}
