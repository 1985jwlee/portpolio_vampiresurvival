using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface IMonsterType : IStatusValue<MonsterType> {}
    
    [System.Serializable]
    public struct MonsterTypeData : IMonsterType
    {
        [SerializeField] private MonsterType value;
        public MonsterType statusValue { get => value; set => this.value = value; }
    }
    
    public class MonsterTypeImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private MonsterTypeData monsterTypeData;
        public ref MonsterTypeData monsterTypeDataProperty => ref monsterTypeData;
        
        public void InitializeComponent()
        {
            
        }
    }
}
