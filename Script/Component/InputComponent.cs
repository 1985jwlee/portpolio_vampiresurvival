using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public struct PlayerInput : IStatusValue<Vector2>
    {
        private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }
}
