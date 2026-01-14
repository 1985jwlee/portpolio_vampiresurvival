using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ICharacterSpeechTypeValue : IStatusValue<CharacterSpeechType> { }

    [System.Serializable]
    public struct CharacterSpeechTypeValue : ICharacterSpeechTypeValue
    {
        [SerializeField] private CharacterSpeechType value;
        public CharacterSpeechType statusValue { get => value; set => this.value = value; }
    }

    public class TokenSpeechImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private CharacterSpeechTypeValue characterSpeechType;

        public ref CharacterSpeechTypeValue CharacterSpeechTypeProperty => ref characterSpeechType;

        public void InitializeComponent()
        {

        }
    }
}
