using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Game.ECS
{
    public enum CharacterTypes
    {
        PlayerCharacter, Enemy, Neutral, Token, Sanctum
    }
    
    public enum UnitScaleType
    {
        Small, Midium, Large
    }
    
    public enum UnitViewDirection
    {
        UL, UC, UR, CL, CR, DL, DC, DR
    }


    public interface IStatusValue<T> where T : struct
    {
        T statusValue { get; set; }
    }

    public interface IHitPoint : IStatusValue<int> {}
    public interface ICharacterType : IStatusValue<CharacterTypes>{}
    public interface IUnitScale : IStatusValue<UnitScaleType>{}
    public interface IUnitViewDirection : IStatusValue<UnitViewDirection> {}
    
    public interface IPlayerCharacterEXP : IStatusValue<uint> {}

    [System.Serializable]
    public struct HitPoint : IHitPoint
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct MoveDirection : IMoveDirection
    {
        [SerializeField]private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct Velocity : IVelocity
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct Position : IPosition
    {
        [SerializeField]private Vector3 value;
        public Vector3 statusValue { get => value; set => this.value = value; }
    }
    
    [System.Serializable]
    public struct Rotation : IRotation
    {
        [SerializeField]private Quaternion value;
        public Quaternion statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct Scale : IScale
    {
        [SerializeField]private Vector3 value;
        public Vector3 statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct CharacterType : ICharacterType
    {
        [SerializeField] private CharacterTypes value;
        public CharacterTypes statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct UnitScale : IUnitScale
    {
        [SerializeField] private UnitScaleType value;
        public UnitScaleType statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct UnitDirection : IUnitViewDirection
    {
        [SerializeField] private UnitViewDirection value;
        public UnitViewDirection statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct PlayerCharacterEXP : IPlayerCharacterEXP
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    
}
