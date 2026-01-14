using System;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DecreaseSpeedImplement : MonoBehaviour, IComponent, IReactivePropertyComponent<Vector2>
    {
        public Vector2 initialVelocity;
        [SerializeField] private float zeroVelocityTimer;
        private float velocityDifferential;
        private float velocityScalar;
        [SerializeField] private float reversalWaitTime;
        private bool needReversalWait;
        private float reversalWaitTick;
        

        public IReactiveProperty<Vector2> reactiveProperty => currentVelocity;
        private ReactiveProperty<Vector2> currentVelocity;

        public void InitializeComponent()
        {
            if(currentVelocity != null){ currentVelocity.Dispose();}
            currentVelocity = new ReactiveProperty<Vector2>();
        }

        public void InitializeValue(Vector2 _initVelocity)
        {
            initialVelocity = _initVelocity;
            currentVelocity.Value = initialVelocity;
            velocityScalar = initialVelocity.magnitude;
            if (zeroVelocityTimer > 0f)
            {
                velocityDifferential = velocityScalar / zeroVelocityTimer;    
            }

            needReversalWait = reversalWaitTime > 0f;
            reversalWaitTick = 0f;
        }

        public void DecreaseSpeed()
        {
            if (velocityScalar < 0f &&  needReversalWait)
            {
                reversalWaitTick += Time.deltaTime;
                if (reversalWaitTick < reversalWaitTime)
                {
                    currentVelocity.Value = Vector2.zero;
                    return;
                }

                needReversalWait = false;
                velocityScalar = 0f;
            }
            velocityScalar -= velocityDifferential * Time.deltaTime;
            currentVelocity.Value = velocityScalar * initialVelocity.normalized;
        }
    }
}
