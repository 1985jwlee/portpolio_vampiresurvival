using UnityEngine;

namespace Game.ECS
{
    public static class MaterialProperties
    {
        public static readonly int GrayScaleId = Shader.PropertyToID("_GrayScaleId");
        public static readonly Color GoldColor = new Color(1, 1, 0, 1f);
    }

    public class HitTintImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private HitTintTriggerData hitTintTriggerData;
        public ref HitTintTriggerData hitTintTriggerProperty => ref hitTintTriggerData;
        public void InitializeComponent()
        {
            
        }
    }
}
