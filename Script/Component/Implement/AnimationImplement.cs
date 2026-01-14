using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class AnimationImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private AnimationHashData animationState;
        public ref AnimationHashData animationStateProperty => ref animationState;


        public void InitializeComponent()
        {
        }
    }
}
