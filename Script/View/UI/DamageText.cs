using TMPro;
using UnityEngine;

namespace Game.ECS
{
    public class DamageText : MonoBehaviour
    {
        public Color defaultColor;
        public Color criticalColor;
        
        public RectTransform myTransform;
        public TextMeshProUGUI damageText;
        void Start()
        {
            Destroy(gameObject, 0.2f);
        }

        public void SetText(string damage, bool isCritical, AttackType attackType)
        {
            damageText.text = damage;
            damageText.color = isCritical ? criticalColor : defaultColor;
        }

        public void SetHealText(string heal)
        {
            damageText.text = heal;
            damageText.color = defaultColor;
        }

    }
}
