using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public enum TokenStateType
    {
        Idle, Catched, Applied
    }
    
    public interface ITokenState : IStatusValue<TokenStateType> {}

    [System.Serializable]
    public struct TokenState : ITokenState
    {
        [SerializeField] private TokenStateType value;
        public TokenStateType statusValue { get => value; set => this.value = value; }
    }
}
