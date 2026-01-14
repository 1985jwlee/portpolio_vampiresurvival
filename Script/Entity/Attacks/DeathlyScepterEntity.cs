using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class DeathlyScepterEntity : AttackEntity
    {
        [Inject] protected ISeekingTargetSystem seekingTargetSystem;
        public Animator animatorImplement;
        public DelayAttackSecondImplement delayAttackImplement;
        public SeekingTargetImplement seekingTargetImplement;
        public Collider2D colliderImplement;
        
        private static readonly int Wait = Animator.StringToHash("Wait");
        private static float randomAngle;
        private float rotateAngle;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            
            TryGetComponent(out animatorImplement);
            TryGetComponent(out colliderImplement);
            colliderImplement.enabled = false;

            if (TryGetComponent(out delayAttackImplement))
            {
                Components.Add(delayAttackImplement);
            }

            if (TryGetComponent(out seekingTargetImplement))
            {
                Components.Add(seekingTargetImplement);
            }
            seekingTargetSystem.RegistComponent(seekingTargetImplement, this);
        }
        
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            
            animatorImplement.enabled = true;
            animatorImplement.SetTrigger(Wait);
            
            
            
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            seekingTargetImplement.seekingTargetProperty.seekTargetEGID = playerCharacterEntity.EGID;
            seekingTargetImplement.seekingTargetProperty.seekingTime = -1f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    CheckSeekTarget(entityContainer, seekingTargetImplement, transformImplement);
                });
            delayAttackImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    colliderImplement.enabled = true;
                });
            
        }

        public override void OnApplyShootCountChanged()
        {
            var count = shootCounterImplement.shootCountProperty.statusValue;
            if (count > 1)
            {
                rotateAngle = randomAngle + 180f;
            }
            else
            {
                randomAngle = Random.Range(0f, 360f);
                rotateAngle = randomAngle;
            }
            
            
            
        }


        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            animatorImplement.ResetTrigger(Wait);
            animatorImplement.enabled = false;
            colliderImplement.enabled = false;
            seekingTargetSystem.UnRegistComponent(this);
        }
        
        protected override void Update()
        {
            base.Update();
            transformImplement.rotation = Quaternion.Euler(0f, 0f, rotateAngle);
        }
        
        private static void CheckSeekTarget(IEntityContainer entityContainer, SeekingTargetImplement seekingTargetImpl, Transform transformImpl)
        {
            if (entityContainer.GetEntity<PlayerCharacterEntity>(seekingTargetImpl.seekingTargetProperty.seekTargetEGID, out var playerCharacter))
            {
                transformImpl.position = playerCharacter.translateImplement.positionProperty.statusValue;
            }
        }

        protected override void CheckAttackRange()
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var sizeMultiply = playerCharacterEntity.characterStatusImplement.buffedattackSizeRatioProperty.statusValue;
            foreach (var buff in applyBuffImplement.applyBuffList)
            {
                if (buff.statusValue.buffType == BuffType.AttackArea)
                {
                    sizeMultiply *= buff.statusValue.buffValue;
                }
            }

            transformImplement.localScale = new Vector3(1f, 1f * sizeMultiply, 1f);
        }
    }
    
    /*
     *
     * [Inject] private Camera viewCamera;
        public Animator animatorImplement;
        public DelayAttackSecondImplement delayAttackImplement;
        private float angle;
        public SpriteRenderer ready;
        public SpriteRenderer attack;
        private static readonly int Wait = Animator.StringToHash("Wait");

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            animatorImplement = GetComponent<Animator>();
            ready = transformImplement.Find("Ready/Ready").GetComponent<SpriteRenderer>();
            attack = transformImplement.Find("Attack").GetComponent<SpriteRenderer>();
            
            
            delayAttackImplement = GetComponent<DelayAttackSecondImplement>();
            Components.Add(delayAttackImplement);
        }
        
        public override void OnApplyShootCountChanged()
        {
            base.OnApplyShootCountChanged();
            angle = Random.Range(0f, 360f);

            animatorImplement.enabled = true;
            animatorImplement.SetTrigger(Wait);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            animatorImplement.ResetTrigger(Wait);
            animatorImplement.enabled = false;
        }

        protected override void Update()
        {
            base.Update();
            var characterPosition = entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue;
            if (ExtensionFunction.RandomViewportEdgeToWorldPos(viewCamera, characterPosition, angle, out var world) == false)
            {
                return;
            }

            var direction = (Vector2)(world - characterPosition);
            var distance = direction.magnitude;
            
            var randomAngle = Vector2.Angle(direction, Vector2.right);
            if (direction.y < 0f)
            {
                randomAngle *= -1f;
            }
            transformImplement.localRotation = Quaternion.Euler(0f, 0f, randomAngle);
            var readySize = ready.size;
            readySize.x = distance;
            ready.size = readySize;
            var attackSize = attack.size;
            attackSize.x = distance;
            attack.size = attackSize;
        }
     * */
}
