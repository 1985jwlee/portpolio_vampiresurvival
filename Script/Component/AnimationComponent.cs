using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public static class AnimationHashs
    {
        public static int Idle = Animator.StringToHash(nameof(Idle));
        public static int Walk = Animator.StringToHash(nameof(Walk));
        public static int Die = Animator.StringToHash(nameof(Die));
        public static int Damaged = Animator.StringToHash(nameof(Damaged));

        public static int[] AllHashs = new int[] { Idle, Walk, Die, Damaged };
    }

    public static class SlimeKingAnimationHashs
    {
        public static int Attack_0_Before = Animator.StringToHash(nameof(Attack_0_Before));
        public static int Attack_0 = Animator.StringToHash(nameof(Attack_0));
        public static int Attack_0_After = Animator.StringToHash(nameof(Attack_0_After));

        public static int Attack_1_Before = Animator.StringToHash(nameof(Attack_1_Before));
        public static int Attack_1 = Animator.StringToHash(nameof(Attack_1));
        public static int Attack_1_After = Animator.StringToHash(nameof(Attack_1_After));

        public static int Attack_2_Before = Animator.StringToHash(nameof(Attack_2_Before));
        public static int Attack_2 = Animator.StringToHash(nameof(Attack_2));
        public static int Attack_2_After = Animator.StringToHash(nameof(Attack_2_After));

        public static int Attack_3_Before = Animator.StringToHash(nameof(Attack_3_Before));
        public static int Attack_3 = Animator.StringToHash(nameof(Attack_3));
        public static int Attack_3_After = Animator.StringToHash(nameof(Attack_3_After));

        public static (int Before, int Attack, int After)[] AllHashs = new (int Before, int Attack, int After)[] { 
            (Attack_0_Before, Attack_0, Attack_0_After),
            (Attack_1_Before, Attack_1, Attack_1_After),
            (Attack_2_Before, Attack_2, Attack_2_After),
            (Attack_3_Before, Attack_3, Attack_3_After),
        };
    }

    public interface IAnimationHashData : IStatusValue<int> { }

    [System.Serializable]
    public struct AnimationHashData : IAnimationHashData
    {
        [SerializeField] private int _oldState;
        [SerializeField] private int value;
        public int oldValue { get => _oldState; set => _oldState = value; }
        public int statusValue { get => value; set => this.value = value; }
    }
}
