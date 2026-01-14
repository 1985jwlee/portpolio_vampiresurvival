using UnityEngine;

namespace Game.ECS
{

    public class BossImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private IsDead isDead;
        [SerializeField] private IsFinalBoss isFinalBoss;
        [SerializeField] private BossIconPosition bossIconPosition;

        public ref IsDead IsDeadProperty => ref isDead;
        public ref IsFinalBoss IsFinalBossProperty => ref isFinalBoss;
        public ref BossIconPosition BossIconPositionProperty => ref bossIconPosition;

        public void InitializeComponent()
        {
            isDead.statusValue = false;
            isFinalBoss.statusValue = false;
        }
    }
}
