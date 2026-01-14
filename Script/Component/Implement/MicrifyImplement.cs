using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class MicrifyImplement : MonoBehaviour, IComponent
    {
        public MicrifyTimeData MicrifyTimeProperty;

        public void InitializeComponent()
        {
        }
    }

    [Serializable]
    public struct MicrifyTimeData : IStatusValue<float>
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
}
