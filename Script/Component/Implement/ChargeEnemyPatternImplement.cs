using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class ChargeEnemyPatternImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private DetectionRadius detectionRadius;
        public ref DetectionRadius DetectionRadiusProperty => ref detectionRadius;
        public void InitializeComponent()
        {
        }
    }
}
