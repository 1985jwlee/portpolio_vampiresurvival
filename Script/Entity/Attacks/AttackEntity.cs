using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class AttackEntity : Entity
    {

        [Inject] protected IRemoveLifeOverAttackSystem removeLifeOverAttackSystem;
        
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            transformImplement = transform;
            TryGetComponent(out rigidBodyImplement);

            this.TryGetOrAddComponentInChild(out spriterendererImplement);

            if (TryGetComponent(out translateImplement))
            {
                Components.Add(translateImplement);
            }
            if (TryGetComponent(out applyBuffImplement))
            {
                Components.Add(applyBuffImplement);
            }
            if (TryGetComponent(out attackLifetimeImplement))
            {
                Components.Add(attackLifetimeImplement);
            }
            if (TryGetComponent(out shootCounterImplement))
            {
                Components.Add(shootCounterImplement);
            }
            if (TryGetComponent(out knockBackSendImplement))
            {
                Components.Add(knockBackSendImplement);
            }
            if (TryGetComponent(out attackTypeImplement))
            {
                Components.Add(attackTypeImplement);
            }
            if (TryGetComponent(out weaponDataSetImplement))
            {
                Components.Add(weaponDataSetImplement);
            }

            if (TryGetComponent(out spearAttackImplement))
            {
                Components.Add(spearAttackImplement);
            }

            removeLifeOverAttackSystem.RegistComponent(attackLifetimeImplement, this);
        }

        public override void ApplyComponentData()
        {
            for (int i = 0, cnt = applyBuffImplement.applyBuffList.Count; i < cnt; i++)
            {
                var data = applyBuffImplement.applyBuffList[i];
                var tmp = data.statusValue;
                tmp.rootEntityId = EGID;
                data.statusValue = tmp;
                applyBuffImplement.applyBuffList[i] = data;
            }
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            applyBuffImplement.applyBuffList.Clear();
            removeLifeOverAttackSystem.UnRegistComponent(this);
        }

        public Transform transformImplement;
        public Rigidbody2D rigidBodyImplement;
        public SpriteRenderer spriterendererImplement;
        public TranslateImplement translateImplement;
        public ApplyBuffImplement applyBuffImplement;
        public AttackLifeTimeImplement attackLifetimeImplement;
        public SpearAttackImplement spearAttackImplement;
        public ShootCounterImplement shootCounterImplement;
        public KnockBackSendImplement knockBackSendImplement;
        public AttackTypeImplement attackTypeImplement;
        public RefreshSingleInstanceAttackImplement refreshSingleAttackImplement;
        public AttackWeaponStatusImplement weaponDataSetImplement;

        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
            CheckAttackRange();
        }

        public virtual void OnApplyShootCountChanged()
        {
            
        }

        public virtual void InitWithRootCharacterEGID(uint EGID)
        {

        }

        protected static void SyncTransform(Transform transformImpl, TranslateImplement translateImple)
        {
            translateImple.positionProperty.statusValue = transformImpl.localPosition;
            translateImple.rotationProperty.statusValue = transformImpl.localRotation;
            translateImple.scaleProperty.statusValue = transformImpl.localScale;
        }

        protected static void RotateObject(Transform transformImpl, float speed)
        {
            var localRot = transformImpl.localRotation.eulerAngles;
            localRot.z += Time.deltaTime * speed;
            transformImpl.localRotation = Quaternion.Euler(localRot);
        }

        protected virtual void CheckAttackRange()
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
            transformImplement.localScale = sizeMultiply * Vector3.one;
        }
    }
}
