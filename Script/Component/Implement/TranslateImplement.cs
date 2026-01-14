using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class TranslateImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private Velocity velocity;
        [SerializeField] private MoveDirection moveDirection;
        
        [SerializeField] private Position position;
        [SerializeField] private Rotation rotation;
        [SerializeField] private Scale scale;
        
        
        public ref MoveDirection moveDirectionProperty => ref moveDirection;
        public ref Velocity velocityProperty => ref velocity;
        
        public ref Position positionProperty => ref position;
        public ref Rotation rotationProperty => ref rotation;
        public ref Scale scaleProperty => ref scale;
        public void InitializeComponent()
        {
            
        }
    }
}
