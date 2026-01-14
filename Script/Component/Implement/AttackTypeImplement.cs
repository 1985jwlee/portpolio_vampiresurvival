using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum AttackType
    {
        PHYSICS, MAGIC, INSTANT_DEATH,
    }
    
    public interface IAttackType : IStatusValue<AttackType>{}

    [System.Serializable]
    public struct AttackEntityType : IAttackType
    {
        [SerializeField] private AttackType value;
        public AttackType statusValue { get => value; set => this.value = value; }
    }

    public class AttackTypeImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private AttackEntityType attackType;
        public ref AttackEntityType attackTypeProperty => ref attackType;
        public void InitializeComponent()
        {
            
        }
    }
}
