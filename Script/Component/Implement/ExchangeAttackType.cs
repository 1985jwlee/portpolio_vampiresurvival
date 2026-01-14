using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class ExchangeAttackType : MonoBehaviour, IComponent
    {
        [SerializeField] private AttackEntityType initAttackType;
        [SerializeField] private AttackEntityType exchangeAttackType;
        
        public ref AttackEntityType initAttackTypeProperty => ref initAttackType;
        public ref AttackEntityType exchangeAttackTypeProperty => ref exchangeAttackType;

        public void InitializeComponent()
        {
            
        }
    }
}
