using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ITeleportId : IStatusValue<uint> { }
    public interface ITeleportPosition : IStatusValue<Vector2> { }
    public interface ITeleportFxPosition : IStatusValue<Vector2> { }
    public interface IFxEGID : IStatusValue<uint> { }
    public interface IIsInitialized : IStatusValue<bool> { }

    [System.Serializable]
    public struct TeleportId : ITeleportId
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct TeleportPosition : ITeleportPosition
    {
        [SerializeField] private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct TeleportFxPosition : ITeleportFxPosition
    {
        [SerializeField] private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct FxEGID : IFxEGID
    {
        [SerializeField] private uint value;
        public uint statusValue { get => value; set => this.value = value; }
    }

    [System.Serializable]
    public struct IsInitialized : IIsInitialized
    {
        [SerializeField] private bool value;
        public bool statusValue { get => value; set => this.value = value; }
    }

    public class InteractableTeleportImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private TeleportId teleportId;
        [SerializeField] private TeleportPosition teleportPosition;
        private IsInitialized isInitialized;

        public List<TeleportFxPosition> teleportFxPositions;
        public List<FxEGID> fxEgids;

        public ref TeleportId TeleportIdProperty => ref teleportId;
        public ref TeleportPosition TeleportPositionProperty => ref teleportPosition;
        public ref IsInitialized IsInitializedProperty => ref isInitialized;

        public void InitializeComponent()
        {
        }
    }
}
