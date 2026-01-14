using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IPosition : IStatusValue<Vector3> {}
    public interface IRotation : IStatusValue<Quaternion> {}
    public interface IScale : IStatusValue<Vector3> {}
    public interface IMoveDirection : IStatusValue<Vector2> {}
    public interface IVelocity : IStatusValue<float> {}
}
