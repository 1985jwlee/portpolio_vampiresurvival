using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IVfxEgid : IStatusValue<uint> { };

    public interface ILifeTime : IStatusValue<float> { };

    [System.Serializable]
    public struct VfxEgid : IVfxEgid
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct LifeTime : ILifeTime
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    public class SanctumVfxManageImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private VfxEgid vfxEgid; 
        [SerializeField] private LifeTime lifeTime;

        public ref VfxEgid VfxEgidProperty => ref vfxEgid;
        public ref LifeTime LifeTimeProperty => ref lifeTime;

        public void InitializeComponent() { }
    }
}
