using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IAttackSpearCount : IStatusValue<int>{}

    /// <summary>
    /// StatusValue 가 음수일 경우 무한관통
    /// </summary>
    [System.Serializable]
    public struct AttackSpearCount : IAttackSpearCount
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    public class SpearAttackImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private AttackSpearCount attackSpearCount;
        public ref AttackSpearCount attackSpearCountProperty => ref attackSpearCount;
        public void InitializeComponent()
        {
            
        }
    }
}
