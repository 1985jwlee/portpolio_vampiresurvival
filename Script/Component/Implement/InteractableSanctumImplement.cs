using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum SanctumType
    {
        Experience,
        Speed,
        Heal,
    }

    public interface ISanctumTypeValue : IStatusValue<SanctumType> { };

    [System.Serializable]
    public struct SanctumTypeValue : ISanctumTypeValue
    {
        [SerializeField] private SanctumType value;
        public SanctumType statusValue { get => value; set => this.value = value; }
    }

    public class InteractableSanctumImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private SanctumTypeValue sanctumType;

        [SerializeField] public ref SanctumTypeValue SanctumTypeProperty => ref sanctumType;

        public void InitializeComponent()
        {
        }
    }
}
