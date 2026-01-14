using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class MonsterMaxHitpointImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private MaxHitPoint maxHitPoint;
        public ref MaxHitPoint maxHitPointProperty => ref maxHitPoint;
        
        public void InitializeComponent()
        {
            
        }
    }
}
