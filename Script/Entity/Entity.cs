using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class Entity : MonoBehaviour, IEntity
    {
        [Inject] protected IEntityContainer entityContainer;
        
#region Entity
        public int SrcPathHashCode { get; set; }
        public uint EGID { get; set; }
        
        public ICollection<IComponent> Components { get; private set; }
        
        public virtual void OnCreateEntity() => entityContainer.AddEntityPull(this);

        public virtual void OnRemoveEntity() => entityContainer.RemoveEntityPull(this);

        public void InitializeComponents()
        {
            foreach (var comp in Components)
            {
                comp.InitializeComponent();
            }
        }
        
        public virtual void ApplyComponentData(){}

#endregion

        protected void Awake()
        {
            Components  = new List<IComponent>();
        }

        protected virtual void Update()
        {
            
        }
    }
}
