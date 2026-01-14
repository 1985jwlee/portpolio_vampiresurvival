using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class KnockBackSendImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private KnockBackSendData knockBackData;
        public ref KnockBackSendData knockBackSendProperty => ref knockBackData;
        public void InitializeComponent()
        {
            
        }
    }
}
