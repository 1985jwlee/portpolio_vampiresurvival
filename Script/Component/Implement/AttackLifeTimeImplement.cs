using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class AttackLifeTimeImplement : MonoBehaviour, IComponent, ITickComponent
    {
        public float lifeTime;
        public float fixedTickTime => lifeTime;
        public float currentTickTime { get; set; }
        
        public bool OnScanTime()
        {
            if (fixedTickTime < 0f)
            {
                return false;
            }
            
            currentTickTime += Time.deltaTime;
            if (currentTickTime > fixedTickTime)
            {
                return true;
            }
            return false;
        }
        
        public void InitializeComponent()
        {
            currentTickTime = 0f;
        }
    }
}
