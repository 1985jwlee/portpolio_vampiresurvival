using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class KnockBackReceiveImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private KnockBackReceiveData knockBackData;
        [SerializeField] private KnockBackResistData knockBackResistData;

        public ref KnockBackReceiveData knockBackReceiveProperty => ref knockBackData;
        public ref KnockBackResistData knockBackResistProperty => ref knockBackResistData;

        public void InitializeComponent()
        {
            
        }
    }
}
