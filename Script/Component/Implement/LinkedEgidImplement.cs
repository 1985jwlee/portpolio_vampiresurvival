using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class LinkedEgidImplement : MonoBehaviour, IComponent
    {
        public LinkedEgidData linkedEgidProperty;

        public void InitializeComponent()
        {
            linkedEgidProperty.statusValue = 0;
        }

        [Serializable]
        public struct LinkedEgidData : IStatusValue<uint>
        {
            [SerializeField] private uint value;
            public uint statusValue { get => value; set => this.value = value; }
        }

    }
}
