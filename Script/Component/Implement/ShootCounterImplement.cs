using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface  IShootCounter : IStatusValue<int> {}

    [System.Serializable]
    public struct ShootCounter : IShootCounter
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    public class ShootCounterImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private ShootCounter maxShootCount;
        [SerializeField] private ShootCounter shootCount;
        public ref ShootCounter shootCountProperty => ref shootCount;
        public ref ShootCounter maxShootCountProperty => ref maxShootCount;
        public void InitializeComponent()
        {
            
        }
    }
}
