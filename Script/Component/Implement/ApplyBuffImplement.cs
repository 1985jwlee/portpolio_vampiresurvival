using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class ApplyBuffImplement : MonoBehaviour, IComponent
    {
        public List<Buff> applyBuffList = new List<Buff>();
        public void InitializeComponent()
        {
            
        }
    }
}
