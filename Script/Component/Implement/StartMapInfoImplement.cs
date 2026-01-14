using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IStartPosition : IStatusValue<Vector2> { };

    [System.Serializable]
    public struct StartPosition : IStartPosition
    {
        [SerializeField] private Vector2 value;
        public Vector2 statusValue { get => value; set => this.value = value; }
    }

    public class StartMapInfoImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private StartPosition startPosition;

        public ref StartPosition StartPositionProperty => ref startPosition;

        public void InitializeComponent()
        {
            return;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(startPosition.statusValue, "StartPosition.png", true);
        }
#endif
    }
}
