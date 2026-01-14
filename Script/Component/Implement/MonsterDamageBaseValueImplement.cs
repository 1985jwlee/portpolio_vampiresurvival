using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IDamageBaseValue : IStatusValue<float> { };

    [System.Serializable]
    public struct DamageBaseValue : IDamageBaseValue
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }


    public class MonsterDamageBaseValueImplement : MonoBehaviour, IComponent
    {
        private DamageBaseValue damageBaseValue;

        public ref DamageBaseValue DamageBaseValueProperty => ref damageBaseValue;

        public void InitializeComponent()
        {
        }
    }
}
