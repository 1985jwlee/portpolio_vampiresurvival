using UnityEngine;

namespace Game.ECS
{
    public class TableIndexDataImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private TableDataIndexNo tableIndexNo;
        public ref TableDataIndexNo tableDataIndexNoProperty => ref tableIndexNo;
        public void InitializeComponent()
        {
            
        }
    }
}
