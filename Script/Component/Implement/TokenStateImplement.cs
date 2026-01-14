using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class TokenStateImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private TokenState tokenState;
        public ref TokenState tokenStateProperty => ref tokenState;
        public void InitializeComponent()
        {
            
        }
    }
}
