using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Game.ECS
{

    public struct DamageType
    {
        public AttackType attackType;
        public bool isCritical;
        public int damage;
    }
    
    public class CharacterEntity : Entity
    {
        #region Entity
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            TryGetComponent(out rigidBodyImplement);
            transformImplement = transform;
            spriterendererImplement = GetComponentInChildren<SpriteRenderer>();
            animatorImplement = GetComponentInChildren<Animator>();
            characterScalerImplement = transform.Find("CharacterScaler");
            
            if (TryGetComponent(out translateImplement))
            {
                Components.Add(translateImplement);
            }
            if (TryGetComponent(out statusImplement))
            {
                Components.Add(statusImplement);
            }
            if (TryGetComponent(out buffImplement))
            {
                Components.Add(buffImplement);
            }
            if (TryGetComponent(out animationImplement))
            {
                Components.Add(animationImplement);
            }
            if (TryGetComponent(out knockBackReceiveImplement))
            {
                Components.Add(knockBackReceiveImplement);
            }
            if (TryGetComponent(out waitbuffTickImplement))
            {
                Components.Add(waitbuffTickImplement);
            }
            if (TryGetComponent(out hitTintImplement))
            {
                Components.Add(hitTintImplement);
            }
            if (TryGetComponent(out grayscaleImplement))
            {
                Components.Add(grayscaleImplement);
            }
            if (TryGetComponent(out translateImplement))
            {
                Components.Add(translateImplement);
            }
            if (TryGetComponent(out characterDeathImplement))
            {
                Components.Add(characterDeathImplement);
            }
        }
        
        public override void ApplyComponentData()
        {
            animationImplement.animationStateProperty.statusValue = AnimationHashs.Idle;
        }
        
        #endregion

        #region Component
        public CommonStatusImplement statusImplement;
        public CharacterDeathImplement characterDeathImplement;
        public BuffImplement buffImplement;
        public Transform transformImplement;
        public Rigidbody2D rigidBodyImplement;
        public SpriteRenderer spriterendererImplement;
        public TranslateImplement translateImplement;
        public AnimationImplement animationImplement;
        public Animator animatorImplement;
        public KnockBackReceiveImplement knockBackReceiveImplement;
        public WaitBuffTickImplement waitbuffTickImplement;
        public HitTintImplement hitTintImplement;
        public GrayScaleImplement grayscaleImplement;
        public Transform characterScalerImplement;
        #endregion

        #region System

        protected override void Update()
        {
            SyncTransform(transformImplement, translateImplement);
            UpdateAnimator(animatorImplement, animationImplement);
            UpdateDirection(characterScalerImplement, statusImplement);
            ApplyBuffValue(EGID, entityContainer, buffImplement, statusImplement);
            ApplyUnitDie(statusImplement, animationImplement, characterDeathImplement);
            ApplyKnockBack(knockBackReceiveImplement, rigidBodyImplement);
            UpdateSpriteColor(animatorImplement, hitTintImplement);
            UpdateSpriteGrayScale(spriterendererImplement, grayscaleImplement);
            UpdateDeathState(characterDeathImplement);
        }


        private static void SyncTransform(Transform transformImpl, TranslateImplement translateImple)
        {
            translateImple.positionProperty.statusValue = transformImpl.localPosition;
            translateImple.rotationProperty.statusValue = transformImpl.localRotation;
            translateImple.scaleProperty.statusValue = transformImpl.localScale;
        }

        private static void UpdateAnimator(Animator animatorImpl, AnimationImplement animationImpl)
        {
            if (animationImpl.animationStateProperty.oldValue == animationImpl.animationStateProperty.statusValue)
                return;

            int newStateHash = animationImpl.animationStateProperty.statusValue;

            foreach (var parameters in animatorImpl.parameters)
                if(parameters.type == AnimatorControllerParameterType.Trigger)
                    animatorImpl.ResetTrigger(parameters.name);

            animatorImpl.SetTrigger(newStateHash);
            animationImpl.animationStateProperty.oldValue = animationImpl.animationStateProperty.statusValue;
        }

        private static void UpdateDirection(Transform characterScalerImpl, CommonStatusImplement statusImpl)
        {
            characterScalerImpl.transform.rotation = statusImpl.unitDirectionProperty.statusValue switch
            {
                UnitViewDirection.UL => GameSettings.NotReversed,
                UnitViewDirection.UC => characterScalerImpl.transform.rotation,
                UnitViewDirection.UR => GameSettings.Reversed,
                UnitViewDirection.CL => GameSettings.NotReversed,
                UnitViewDirection.CR => GameSettings.Reversed,
                UnitViewDirection.DL => GameSettings.NotReversed,
                UnitViewDirection.DC => characterScalerImpl.transform.rotation,
                UnitViewDirection.DR => GameSettings.Reversed,
                _ => characterScalerImpl.transform.rotation,
            };
        }
        
        private static void ApplyBuffValue(uint egid, IEntityContainer entityContainer,  BuffImplement buffImpl, CommonStatusImplement statusImpl)
        {
            buffImpl.CheckBuffCooldown();
            buffImpl.BuffImpl(out var buffDatas);
           
            for (int i = 0; i < buffDatas.Count; ++i)
            {
                switch (buffDatas[i].buffType)
                {
                    case BuffType.CriticalDamage:
                    case BuffType.Damage:
                        {
                            {
                                var reduction = 1f;
                                if (egid == entityContainer.playerCharacterEntity.EGID)
                                {
                                    reduction -= entityContainer.playerCharacterEntity.characterStatusImplement.buffeddamageReductionRatioProperty.statusValue;
                                }
                                var damageValue = new DamageType();
                                damageValue.attackType = AttackType.PHYSICS;
                                damageValue.damage = (int)(buffDatas[i].buffValue * reduction);
                                damageValue.isCritical = buffDatas[i].buffType == BuffType.CriticalDamage;
                                statusImpl.hitPointProperty.statusValue -= damageValue.damage;
                                MessageBroker.Default.Publish(new KeyValuePair<string,(uint, DamageType, CharacterTypes)>("applyDamage", (egid, damageValue, statusImpl.characterTypeProperty.statusValue)));
                            }
                        }
                        break;
                    case BuffType.MagicDamage:
                        {
                            {
                                var reduction = 1f;
                                if (egid == entityContainer.playerCharacterEntity.EGID)
                                {
                                    reduction -= entityContainer.playerCharacterEntity.characterStatusImplement.buffeddamageReductionRatioProperty.statusValue;
                                }
                                var damageValue = new DamageType();
                                damageValue.attackType = AttackType.MAGIC;
                                damageValue.damage = (int)(buffDatas[i].buffValue * reduction);
                                damageValue.isCritical = false;
                                statusImpl.hitPointProperty.statusValue -= damageValue.damage;
                                MessageBroker.Default.Publish(new KeyValuePair<string,(uint, DamageType, CharacterTypes)>("applyDamage", (egid, damageValue, statusImpl.characterTypeProperty.statusValue)));
                            }
                        }
                        break;
                    case BuffType.DeathPenalty:
                        {
                            if (entityContainer.GetEntity<EnemyCharacterEntity, MonsterTypeImplement>(egid, out var enemyEntity, out var typeImpl))
                            {
                                switch (typeImpl.monsterTypeDataProperty.statusValue)
                                {
                                    case MonsterType.Common:
                                        {
                                            statusImpl.hitPointProperty.statusValue = 0;
                                            MessageBroker.Default.Publish(new KeyValuePair<string, uint>("applyDeath", egid));
                                        }
                                        break;
                                    case MonsterType.Boss:
                                        {
                                            var decrease = Mathf.FloorToInt(enemyEntity.monsterMaxHitPointImplement.maxHitPointProperty.statusValue * 0.1f);
                                            var damageValue = new DamageType();
                                            damageValue.attackType = AttackType.PHYSICS;
                                            damageValue.damage = decrease;
                                            damageValue.isCritical = false;
                                            enemyEntity.statusImplement.hitPointProperty.statusValue -= decrease;
                                            MessageBroker.Default.Publish(new KeyValuePair<string,(uint, DamageType, CharacterTypes)>("applyDamage", (egid, damageValue, enemyEntity.statusImplement.characterTypeProperty.statusValue)));
                                        }
                                        break;
                                    case MonsterType.Elite:
                                        {
                                            var decrease = Mathf.FloorToInt(enemyEntity.monsterMaxHitPointImplement.maxHitPointProperty.statusValue * 0.25f);
                                            var damageValue = new DamageType();
                                            damageValue.attackType = AttackType.PHYSICS;
                                            damageValue.damage = decrease;
                                            damageValue.isCritical = false;
                                            enemyEntity.statusImplement.hitPointProperty.statusValue -= decrease;
                                            MessageBroker.Default.Publish(new KeyValuePair<string,(uint, DamageType, CharacterTypes)>("applyDamage", (egid, damageValue, enemyEntity.statusImplement.characterTypeProperty.statusValue)));
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                statusImpl.hitPointProperty.statusValue = 0;
                                MessageBroker.Default.Publish(new KeyValuePair<string, uint>("applyDeath", egid));
                            }
                                
                        }
                        break;
                    case BuffType.ExpUp:
                        {
                            var addExp = (uint)buffDatas[i].buffValue;
                            statusImpl.characterExpProperty.statusValue += addExp;
                            MessageBroker.Default.Publish(new KeyValuePair<string,uint>("addExp", addExp));
                        }
                        break;
                    case BuffType.Heal:
                        {
                            int healValue = (int)buffDatas[i].buffValue;
                            statusImpl.hitPointProperty.statusValue += healValue;
                            MessageBroker.Default.Publish(new KeyValuePair<string, (uint, int, CharacterTypes)>("applyHeal", (egid, healValue, statusImpl.characterTypeProperty.statusValue)));
                        }
                        break;
                    case BuffType.DeviceBox:
                        {
                            MessageBroker.Default.Publish("requestDeviceBoxEvent");
                        }
                        break;
                    case BuffType.GoldUp:
                        {
                            var addGold = (uint)(buffDatas[i].buffValue * entityContainer.playerCharacterEntity.characterStatusImplement.buffedGetGoldMultipleProperty.statusValue);
                            MessageBroker.Default.Publish(new KeyValuePair<string, uint>("addGold", addGold));
                        }
                        break;
                    case BuffType.Bomb:
                        {
                            var playerCharacterEntity = entityContainer.playerCharacterEntity;
                            var colliders = Physics2D.OverlapCircleAll(playerCharacterEntity.transformImplement.position, GameSettings.BombDistance);
                            foreach (var collider in colliders)
                            {
                                if(collider.TryGetComponent<EnemyCharacterEntity>(out var enemyEntity))
                                {
                                    if (enemyEntity.characterDeathImplement.DeathStateProperty.statusValue != DeathState.Living)
                                        continue;

                                    enemyEntity.buffImplement.appliedBuff.Add(new Buff()
                                    {
                                        statusValue = new BuffData()
                                        {
                                            buffDeviceId = string.Empty,
                                            buffType = BuffType.DeathPenalty,
                                            buffValue = 0,
                                            remainTime = -1,
                                            rootCharacter = new CharacterType() { statusValue = CharacterTypes.Token },
                                            rootEntityId = 0
                                        }
                                    });
                                }
                            }

                            MessageBroker.Default.Publish("screenFlash");
                        }
                        break;
                    case BuffType.GoldFever:
                        {
                            MessageBroker.Default.Publish("goldFever");
                        }
                        break;
                    case BuffType.GoldFeverTimeUp:
                        {
                            MessageBroker.Default.Publish(new KeyValuePair<string, float>("goldFeverTimeUp", buffDatas[i].buffValue));
                        }
                        break;
                    case BuffType.Gigantic:
                        {
                            entityContainer.playerCharacterEntity.giganticImplement.earthShakeCounterProperty.statusValue = 1;
                            entityContainer.playerCharacterEntity.characterScalerImplement.transform.localScale = (1 + GameSettings.GiganticScaleAdd) * Vector3.one;
                            entityContainer.playerCharacterEntity.transformImplement.localScale = Vector3.one;
                            entityContainer.playerCharacterEntity.micrifyImplement.MicrifyTimeProperty.statusValue = 0f;
                            buffImpl.appliedBuff.RemoveAll(elmt => elmt.statusValue.buffDeviceId == GameSettings.MicrifyBuffId);
                            if (entityContainer.playerCharacterEntity.giganticImplement.giganticTimeProperty.statusValue <= 0)
                            {
                                entityContainer.playerCharacterEntity.buffImplement.appliedBuff.AddRange(new Buff[] {
                                    new Buff(){ statusValue = new BuffData()
                                    {
                                        buffDeviceId = GameSettings.GiganticBuffId,
                                        buffType = BuffType.DamageReduction,
                                        buffValue = 0.5f,
                                        remainTime = 9999,
                                        rootCharacter = new CharacterType(){ statusValue = CharacterTypes.Token },
                                        rootEntityId = 0,
                                    } },
                                    new Buff(){ statusValue = new BuffData()
                                    {
                                        buffDeviceId = GameSettings.GiganticBuffId,
                                        buffType = BuffType.MoveSpeedIncrement,
                                        buffValue = 0.5f,
                                        remainTime = 9999,
                                        rootCharacter = new CharacterType(){ statusValue = CharacterTypes.Token },
                                        rootEntityId = 0,
                                    } },
                                });
                                entityContainer.playerCharacterEntity.OnApplyPassiveArcaneDevice();
                            }
                            entityContainer.playerCharacterEntity.giganticImplement.giganticTimeProperty.statusValue = GameSettings.GiganticDuration;
                        }
                        break;
                    case BuffType.Micrify:
                        {
                            entityContainer.playerCharacterEntity.characterScalerImplement.transform.localScale = Vector3.one;
                            entityContainer.playerCharacterEntity.transformImplement.localScale = (1 + GameSettings.MicrifyScaleAdd) * Vector3.one;
                            entityContainer.playerCharacterEntity.giganticImplement.giganticTimeProperty.statusValue = 0f;
                            buffImpl.appliedBuff.RemoveAll(elmt => elmt.statusValue.buffDeviceId == GameSettings.GiganticBuffId);
                            if (entityContainer.playerCharacterEntity.micrifyImplement.MicrifyTimeProperty.statusValue <= 0)
                            {
                                entityContainer.playerCharacterEntity.buffImplement.appliedBuff.AddRange(new Buff[] {
                                    new Buff(){ statusValue = new BuffData()
                                    {
                                        buffDeviceId = GameSettings.MicrifyBuffId,
                                        buffType = BuffType.DamageReduction,
                                        buffValue = 0.5f,
                                        remainTime = 9999,
                                        rootCharacter = new CharacterType(){ statusValue = CharacterTypes.Token },
                                        rootEntityId = 0,
                                    } },
                                    new Buff(){ statusValue = new BuffData()
                                    {
                                        buffDeviceId = GameSettings.MicrifyBuffId,
                                        buffType = BuffType.MoveSpeedIncrement,
                                        buffValue = 2.0f,
                                        remainTime = 9999,
                                        rootCharacter = new CharacterType(){ statusValue = CharacterTypes.Token },
                                        rootEntityId = 0,
                                    } },
                                });
                                entityContainer.playerCharacterEntity.OnApplyPassiveArcaneDevice();
                            }
                            entityContainer.playerCharacterEntity.micrifyImplement.MicrifyTimeProperty.statusValue = GameSettings.MicrifyDuration;
                        }
                        break;
                }
            }
            List<Buff> buffsToAdd = new();
            foreach(var buffData in buffImpl.appliedBuff)
            {
                switch (buffData.statusValue.buffType)
                {
                    case BuffType.Magnet:
                        {
                            var playerCharacterEntity = entityContainer.playerCharacterEntity;
                            var tokenEntities = entityContainer.GetEntities<ExpTokenEntity>();

                            var colliders = Physics2D.OverlapCircleAll(playerCharacterEntity.transformImplement.position, GameSettings.MagnetDistance);
                            foreach (var collider in colliders)
                            {
                                if(collider.TryGetComponent<ExpTokenEntity>(out var token))
                                {
                                    if (token.tokenStateImplement.tokenStateProperty.statusValue != TokenStateType.Idle)
                                        continue;

                                    var applyBuffImpl = token.applyBuffImplement;
                                    for (int index = 0; index < applyBuffImpl.applyBuffList.Count; ++index)
                                    {
                                        var buff = applyBuffImpl.applyBuffList[index];

                                        var addExpStatus = buff.statusValue;
                                        addExpStatus.buffValue *= (1f + playerCharacterEntity.characterStatusImplement.buffedexpMultipleProperty.statusValue);

                                        buff.statusValue = addExpStatus;

                                        buffsToAdd.Add(buff);
                                    }
                                    token.tokenStateImplement.tokenStateProperty.statusValue = TokenStateType.Catched;
                                }
                            }
                        }
                        break;
                }
            }
            if(buffsToAdd.Count > 0)
                buffImpl.appliedBuff.AddRange(buffsToAdd);
        }


        private static void ApplyUnitDie(CommonStatusImplement statusImpl, AnimationImplement animationImpl, CharacterDeathImplement characterDeathImpl)
        {
            if (statusImpl.hitPointProperty.statusValue > 0)
            {
                return;
            }
            characterDeathImpl.DeathStateProperty.statusValue = DeathState.Dying;
            animationImpl.animationStateProperty.statusValue = AnimationHashs.Die;
        }

        private static void UpdateDeathState(CharacterDeathImplement characterDeathImpl)
        {
            if (characterDeathImpl.DeathStateProperty.statusValue == DeathState.Living)
                return;

            if(characterDeathImpl.DyingDurationTimerProperty.statusValue < characterDeathImpl.DyingDurationProperty.statusValue)
            {
                characterDeathImpl.DyingDurationTimerProperty.statusValue += Time.deltaTime;
                if(characterDeathImpl.DyingDurationTimerProperty.statusValue >= characterDeathImpl.DyingDurationProperty.statusValue)
                    characterDeathImpl.DeathStateProperty.statusValue = DeathState.Dead;
            }
        }

        private static void ApplyKnockBack(KnockBackReceiveImplement knockBackReceiveImpl, Rigidbody2D rigidBodyImpl)
        {
            if (knockBackReceiveImpl.knockBackResistProperty.statusValue)
                return;

            var knockBackValue = knockBackReceiveImpl.knockBackReceiveProperty.statusValue;
            if (knockBackValue.isKnockBacking && knockBackValue.remainTime > 0f)
            {
                rigidBodyImpl.velocity = knockBackValue.direction * knockBackValue.velocity;

                knockBackValue.remainTime -= Time.deltaTime;
                knockBackReceiveImpl.knockBackReceiveProperty.statusValue = knockBackValue;
            }
            else
            {
                knockBackValue.isKnockBacking = false;
                knockBackReceiveImpl.knockBackReceiveProperty.statusValue = knockBackValue;
            }
        }

        private static void UpdateSpriteGrayScale(SpriteRenderer spriteRendererImpl, GrayScaleImplement grayScaleImpl)
        {
            var propertyBlock = new MaterialPropertyBlock();
            spriteRendererImpl.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInteger(MaterialProperties.GrayScaleId, grayScaleImpl.enableGrayScale ? 1 : 0);
            spriteRendererImpl.SetPropertyBlock(propertyBlock);
        }

        private static void UpdateSpriteColor(Animator animatorImplement, HitTintImplement hitTintImpl)
        {
            if (hitTintImpl.hitTintTriggerProperty.statusValue)
            {
                animatorImplement.SetTrigger(AnimationHashs.Damaged);

                hitTintImpl.hitTintTriggerProperty.statusValue = false;
            }
        }


        protected async UniTask CreateWeapon(int count, GameObjectFactory factory, WeaponDataSet weaponDataSet, CharacterTypes characterType, int groupCount = 0)
        {
            var cnt = 0;
            var cancellationtoken = new CancellationTokenSource();
            while (cnt < count)
            {
                ++cnt;
                if (groupCount > 0)
                {
                    for (int i = 0; i < groupCount; ++i)
                    {
                        CreateWeapon(factory, weaponDataSet, i, groupCount, characterType);
                    }
                }
                else
                {
                    CreateWeapon(factory, weaponDataSet, cnt, count, characterType);
                }

                var copy = CancellationTokenSource.CreateLinkedTokenSource(cancellationtoken.Token);
                if (weaponDataSet.multiCreationTick > 0)
                    await UniTask.Delay((int)(weaponDataSet.multiCreationTick * 1000f), DelayType.DeltaTime, PlayerLoopTiming.Update, copy.Token);
            }
            cancellationtoken.Cancel();
        }

        protected void CreateWeapon(GameObjectFactory factory, WeaponDataSet d, int cnt, int maxCount, CharacterTypes characterType)
        {
            var go = factory.CreateGameObject(d.prefabPath);
            if (go == null)
            {
                return;
            }
            
            

            var attackEntity = go.GetComponent<AttackEntity>();

            if (attackEntity.spearAttackImplement != null)
            {
                attackEntity.spearAttackImplement.attackSpearCountProperty.statusValue = d.spearCount;
            }

            attackEntity.weaponDataSetImplement.weaponDataSetProperty.statusValue = d;

            attackEntity.translateImplement.velocityProperty.statusValue = d.projectileSpeed;
            if (attackEntity.TryGetComponent(out ExchangeAttackType exchangeAttackType))
            {
                if (exchangeAttackType.initAttackTypeProperty.statusValue == d.attackType)
                {
                    d.attackType = exchangeAttackType.exchangeAttackTypeProperty.statusValue;
                }
            }
            attackEntity.attackTypeImplement.attackTypeProperty.statusValue = d.attackType;

            if (d.isSingleCreation)
            {
                attackEntity.refreshSingleAttackImplement = attackEntity.GetOrAddComponent<RefreshSingleInstanceAttackImplement>();
                attackEntity.refreshSingleAttackImplement.attackableId = d.id;
                attackEntity.refreshSingleAttackImplement.attackableEGID = attackEntity.EGID;
            }

            attackEntity.attackLifetimeImplement.lifeTime = d.lifeDuration;

            attackEntity.shootCounterImplement.shootCountProperty.statusValue = cnt;
            attackEntity.shootCounterImplement.maxShootCountProperty.statusValue = maxCount;
            attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue = new KnockBackSendDataSet()
            {
                direction = attackEntity.knockBackSendImplement.knockBackSendProperty.statusValue.direction,
                distance = GameSettings.DefaultKnockBackDistance * d.knockBack
            };
            attackEntity.applyBuffImplement.applyBuffList.Clear();
            attackEntity.applyBuffImplement.applyBuffList.Add(new Buff
            {
                statusValue = new BuffData
                {
                    buffType = d.attackType switch
                    {
                        AttackType.INSTANT_DEATH => BuffType.DeathPenalty,
                        AttackType.PHYSICS => BuffType.Damage,
                        _ => BuffType.MagicDamage
                    },
                    remainTime = -1,
                    rootEntityId = EGID,
                    rootCharacter = new CharacterType()
                    {
                        statusValue = characterType
                    },
                    buffValue = d.damage
                }
                
            });

            if (d.area > 0f)
            {
                attackEntity.applyBuffImplement.applyBuffList.Add(new Buff
                {
                    statusValue = new BuffData
                    {
                        buffType = BuffType.AttackArea,
                        remainTime = -1,
                        rootEntityId = EGID,
                        rootCharacter = new CharacterType()
                        {
                            statusValue = characterType
                        },
                        buffValue = d.area
                    }
                });
            }

            attackEntity.OnApplyShootCountChanged();
            attackEntity.InitWithRootCharacterEGID(EGID);
        }


        public virtual async UniTask TeleportPosition(Vector3 position)
        {
            transformImplement.position = position;
            await UniTask.Delay(100, true, PlayerLoopTiming.Update, new CancellationToken());
        }
        
        #endregion
    }
}
