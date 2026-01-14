using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class CommonStatusImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private HitPoint hitpoint;
        [SerializeField] private CharacterType characterType;
        [SerializeField] private UnitScale unitScale;
        [SerializeField] private UnitDirection unitDirection;
        [SerializeField] private PlayerCharacterEXP characterExp;

        public ref HitPoint hitPointProperty => ref hitpoint;
        public ref CharacterType characterTypeProperty => ref characterType;
        public ref UnitScale unitScaleProperty => ref unitScale;
        public ref UnitDirection unitDirectionProperty => ref unitDirection;
        
        public ref PlayerCharacterEXP characterExpProperty => ref characterExp;

        public void InitializeComponent()
        {
            
        }
    }
}
