using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerCharacterEntity : CharacterEntity
    {
        [Inject] private VfxFactory vfxFactory;
        [Inject] private Camera viewCamera;
        [Inject] private EnemyCharacterFactory enemyCharacterFactory;

#region Component
        public MobileJoystickImplement mobileJoystickImplement;
        public DirectionIndicatorImplement directionIndicatorImplement;
        public WeaponInventoryImplement weaponInventoryImplement;
        public PassiveDeviceInventoryImplement passiveDeviceInventoryImplement;
        public WeaponFactory weaponFactory;
        public ItemRecieverColliderImplement itemReceiverColliderImplement;
        public CameraSyncImplement cameraSyncImplement;
        /// <summary>
        /// 캐릭터 스테이터스
        /// </summary>
        public CharacterStatusImplement characterStatusImplement;
        public GiganticImplement giganticImplement;
        public MicrifyImplement micrifyImplement;
        public SanctumVfxManageImplement sanctumVfxManageImplement;

        #endregion

        #region Entity
        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            mobileJoystickImplement = GetComponentInChildren<MobileJoystickImplement>();
            directionIndicatorImplement = GetComponentInChildren<DirectionIndicatorImplement>();
            itemReceiverColliderImplement = GetComponentInChildren<ItemRecieverColliderImplement>();
            Components.Add(mobileJoystickImplement);
            Components.Add(directionIndicatorImplement);
            Components.Add(itemReceiverColliderImplement);
            
            
            if (TryGetComponent(out weaponInventoryImplement))
            {
                Components.Add(weaponInventoryImplement);
            }
            if (TryGetComponent(out passiveDeviceInventoryImplement))
            {
                Components.Add(passiveDeviceInventoryImplement);
            }
            if (TryGetComponent(out cameraSyncImplement))
            {
                Components.Add(cameraSyncImplement);
            }
            if (TryGetComponent(out characterStatusImplement))
            {
                Components.Add(characterStatusImplement);
            }
            if (TryGetComponent(out giganticImplement))
            {
                Components.Add(giganticImplement);
            }
            if (TryGetComponent(out micrifyImplement))
            {
                Components.Add(micrifyImplement);
            }
            if(TryGetComponent(out sanctumVfxManageImplement))
            {
                Components.Add(sanctumVfxManageImplement);
            }
        }
        
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            translateImplement.positionProperty.statusValue = Vector3.zero;
            statusImplement.characterTypeProperty.statusValue = CharacterTypes.PlayerCharacter;
            statusImplement.unitScaleProperty.statusValue = UnitScaleType.Small;
            statusImplement.unitDirectionProperty.statusValue = UnitViewDirection.CL;
        }

        public void ApplyDefaultStatusData(CharacterDataEntity characterDataEntity)
        {
            var selectedCharacterStat = characterDataEntity;
            characterStatusImplement.maxHitPointProperty.statusValue = selectedCharacterStat.health;
            characterStatusImplement.hitpointRecoveryRateProperty.statusValue = selectedCharacterStat.healthRegen;
            characterStatusImplement.speedRatioProperty.statusValue = selectedCharacterStat.moveSpeedIncrement;
            characterStatusImplement.projectileSpeedRatioProperty.statusValue = 1 + selectedCharacterStat.projectileSpeedIncrement;
            characterStatusImplement.attackSizeRatioProperty.statusValue = 1 + selectedCharacterStat.areaIncrement;
            characterStatusImplement.cooldownRatioProperty.statusValue = 1 - selectedCharacterStat.cooldownReduction;
            characterStatusImplement.addProjCountProperty.statusValue = selectedCharacterStat.projectileIncrement;
            characterStatusImplement.addCriticalRatioProperty.statusValue = selectedCharacterStat.criticalChanceIncrement;
            characterStatusImplement.addMagicRatioProperty.statusValue = selectedCharacterStat.magicDamageIncrement;
            characterStatusImplement.damageReductionRatioProperty.statusValue = selectedCharacterStat.damageReduction;
            characterStatusImplement.counterAttackMultipleProperty.statusValue = 0f;
            characterStatusImplement.pickAreaSizeMultipleProperty.statusValue = 0f;
            characterStatusImplement.expMultipleProperty.statusValue = 0f;

            characterStatusImplement.attackableLifeTimeMultipleProperty.statusValue = 1f;
            characterStatusImplement.getGoldMultipleProperty.statusValue = 1f;

            characterStatusImplement.buffedmaxHitPointProperty.statusValue = characterStatusImplement.maxHitPointProperty.statusValue;
            characterStatusImplement.buffedhitpointRecoveryRateProperty.statusValue = characterStatusImplement.hitpointRecoveryRateProperty.statusValue;
            characterStatusImplement.buffedspeedRatioProperty.statusValue = characterStatusImplement.speedRatioProperty.statusValue;
            characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue = characterStatusImplement.projectileSpeedRatioProperty.statusValue;
            characterStatusImplement.buffedattackSizeRatioProperty.statusValue = characterStatusImplement.attackSizeRatioProperty.statusValue;
            characterStatusImplement.buffedcooldownRatioProperty.statusValue = characterStatusImplement.cooldownRatioProperty.statusValue;
            characterStatusImplement.buffedaddProjCountProperty.statusValue = characterStatusImplement.addProjCountProperty.statusValue;
            characterStatusImplement.buffedaddCriticalRatioProperty.statusValue = characterStatusImplement.addCriticalRatioProperty.statusValue;
            characterStatusImplement.buffedaddMagicRatioProperty.statusValue = characterStatusImplement.addMagicRatioProperty.statusValue;
            characterStatusImplement.buffeddamageReductionRatioProperty.statusValue = characterStatusImplement.damageReductionRatioProperty.statusValue;
            characterStatusImplement.buffedcounterAttackMultipleProperty.statusValue = characterStatusImplement.counterAttackMultipleProperty.statusValue;
            characterStatusImplement.buffedpickAreaSizeMultipleProperty.statusValue = characterStatusImplement.pickAreaSizeMultipleProperty.statusValue;
            characterStatusImplement.buffedexpMultipleProperty.statusValue = characterStatusImplement.expMultipleProperty.statusValue;
            characterStatusImplement.buffedAttackableLifeTimeMultipleProperty.statusValue = characterStatusImplement.attackableLifeTimeMultipleProperty.statusValue;
            characterStatusImplement.buffedGetGoldMultipleProperty.statusValue = characterStatusImplement.getGoldMultipleProperty.statusValue;

            statusImplement.hitPointProperty.statusValue = characterStatusImplement.buffedmaxHitPointProperty.statusValue;
            translateImplement.velocityProperty.statusValue = GameSettings.DefaultSpeedValue + (GameSettings.DefaultSpeedValue * characterStatusImplement.buffedspeedRatioProperty.statusValue);
        }
#endregion

        public void OnApplyPassiveArcaneDevice()
        {
            var buffDict = new Dictionary<BuffType, float>();

            foreach (var buffType in System.Enum.GetValues(typeof(BuffType)).Cast<BuffType>())
                buffDict.Add(buffType, 0);

            foreach (var buff in buffImplement.appliedBuff)
            {
                var buffType = buff.statusValue.buffType;
                switch (buffType)
                {
                    case BuffType.MagicDamage:
                    case BuffType.CriticalDamage:
                    case BuffType.Stun:
                    case BuffType.Damage:
                    case BuffType.Heal:
                    case BuffType.Magnet:
                    case BuffType.DeviceBox:
                    case BuffType.GoldUp:
                    case BuffType.Bomb:
                    case BuffType.GoldFever:
                    case BuffType.GoldFeverTimeUp:
                    case BuffType.Gigantic:
                    case BuffType.DeathPenalty:
                        continue;
                }

                if (buffDict.ContainsKey(buffType))
                {
                    buffDict[buffType] += buff.statusValue.buffValue;
                }
            }
            
            foreach (var buffKV in buffDict)
            {
                switch (buffKV.Key)
                {
                    case BuffType.MaxHealthIncrement:
                        {
                            var src = characterStatusImplement.maxHitPointProperty.statusValue;
                            characterStatusImplement.buffedmaxHitPointProperty.statusValue = src + (int)buffKV.Value;
                        }
                        break;
                    case BuffType.HealthRegenIncrement:
                        {
                            var src = characterStatusImplement.hitpointRecoveryRateProperty.statusValue;
                            characterStatusImplement.buffedhitpointRecoveryRateProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.MoveSpeedIncrement:
                        {
                            var src = characterStatusImplement.speedRatioProperty.statusValue;
                            characterStatusImplement.buffedspeedRatioProperty.statusValue = src + buffKV.Value;
                            translateImplement.velocityProperty.statusValue = GameSettings.DefaultSpeedValue + (GameSettings.DefaultSpeedValue * (src + buffKV.Value));
                        }
                        break;
                    case BuffType.AttackArea:
                        {
                            var src = characterStatusImplement.attackSizeRatioProperty.statusValue;
                            characterStatusImplement.buffedattackSizeRatioProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.AttackableLifeTime:
                        {
                            var src = characterStatusImplement.attackableLifeTimeMultipleProperty.statusValue;
                            characterStatusImplement.buffedAttackableLifeTimeMultipleProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.GetMoreGold:
                        {
                            var src = characterStatusImplement.getGoldMultipleProperty.statusValue;
                            characterStatusImplement.buffedGetGoldMultipleProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.ProjectileSpeedIncrement:
                        {
                            var src = characterStatusImplement.projectileSpeedRatioProperty.statusValue;
                            characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.CooldownReduction:
                        {
                            var src = characterStatusImplement.cooldownRatioProperty.statusValue;
                            characterStatusImplement.buffedcooldownRatioProperty.statusValue = src - buffKV.Value;
                        }
                        break;
                    case BuffType.ProjectileIncrement:
                        {
                            var src = characterStatusImplement.addProjCountProperty.statusValue;
                            characterStatusImplement.buffedaddProjCountProperty.statusValue = src + (int)buffKV.Value;
                        }
                        break;
                    case BuffType.CriticalChanceIncrement:
                        {
                            var src = characterStatusImplement.addCriticalRatioProperty.statusValue;
                            characterStatusImplement.buffedaddCriticalRatioProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.MagicDamageIncrement:
                        {
                            var src =  characterStatusImplement.addMagicRatioProperty.statusValue;
                            characterStatusImplement.buffedaddMagicRatioProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.DamageReduction:
                        {
                            var src = characterStatusImplement.damageReductionRatioProperty.statusValue;
                            characterStatusImplement.buffeddamageReductionRatioProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.CounterAttack:
                        {
                            var src = characterStatusImplement.counterAttackMultipleProperty.statusValue;
                            characterStatusImplement.buffedcounterAttackMultipleProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.PickupAreaIncrement:
                        {
                            var src = characterStatusImplement.pickAreaSizeMultipleProperty.statusValue;
                            characterStatusImplement.buffedpickAreaSizeMultipleProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                    case BuffType.ExperienceIncrement:
                        {
                            var src = characterStatusImplement.expMultipleProperty.statusValue;
                            characterStatusImplement.buffedexpMultipleProperty.statusValue = src + buffKV.Value;
                        }
                        break;
                }
            }
            
            MessageBroker.Default.Publish(new string("OnApplyPassiveArcaneDevice"));
        }

#region System

        protected override void Update()
        {
            base.Update();
            CharacterMove(rigidBodyImplement, mobileJoystickImplement, statusImplement, translateImplement);
            WeaponCoolDown(characterStatusImplement, weaponInventoryImplement, weaponFactory);
            NofityDeath(statusImplement);
            UpdateAnimation(characterDeathImplement, animationImplement, mobileJoystickImplement);
            UpdateDirectionIndicator(directionIndicatorImplement);
            ProcGigantic(giganticImplement, entityContainer, characterScalerImplement, buffImplement);
            ProcMicrify(micrifyImplement, entityContainer, buffImplement);
            UpdateSanctumVfxManange(sanctumVfxManageImplement, entityContainer, vfxFactory);
        }

        private void UpdateDirectionIndicator(DirectionIndicatorImplement directionIndicatorImpl)
        {
            directionIndicatorImpl.transform.rotation = Quaternion.FromToRotation(Vector3.right, translateImplement.moveDirectionProperty.statusValue);
        }

        private static void UpdateAnimation(CharacterDeathImplement characterDeathImpl, AnimationImplement animationImplement, MobileJoystickImplement joystickImplement)
        {
            if (characterDeathImpl.DeathStateProperty.statusValue != DeathState.Living)
                return;

            animationImplement.animationStateProperty.statusValue = joystickImplement.playerInputProperty.statusValue.SqrMagnitude() > 0 ? AnimationHashs.Walk : AnimationHashs.Idle;
        }

        private void WeaponCoolDown(CharacterStatusImplement characterStatImpl, WeaponInventoryImplement weaponInventoryImpl, GameObjectFactory factory)
        {
            if (weaponInventoryImpl.WeaponCoolTimePauseProperty.statusValue)
                return;

            weaponInventoryImpl.CheckWeaponCooldown(characterStatImpl.buffedcooldownRatioProperty.statusValue);
            weaponInventoryImpl.FireWeapon(out var list);
            foreach (var d in list)
            {
                if (string.IsNullOrEmpty(d.prefabPath) || d.isSummonCreation)
                {
                    continue;
                }

                if (d.mustHaveTarget)
                {
                    var enemies = entityContainer.GetEntities<EnemyCharacterEntity>();
                    if (enemies.Count > 0 == false)
                    {
                        continue;
                    }
                }
                
                var createCount = d.createCount;
                if (d.isSingleCreation == false)
                {
                    //생성갯수가 음수일 경우 캐릭터의 투사체증가 버프와 무관하게 절대값 갯수만큼만 생성한다.
                    if (createCount < 0)
                    {
                        createCount = Mathf.Abs(createCount);
                    }
                    else
                    {
                        createCount += characterStatImpl.buffedaddProjCountProperty.statusValue;
                    }
                }
                    
                if (createCount > 1)
                {
                    CreateWeapon(createCount, factory, d, CharacterTypes.PlayerCharacter, d.groupCount).Forget();
                    continue;
                }
                
                if (d.isGroupAttack == false)
                {
                    CreateWeapon(factory, d, 1, 1, CharacterTypes.PlayerCharacter);
                    continue;
                }
                
                for (int i = 0, len = d.groupCount; i < len; ++i)
                {
                    CreateWeapon(factory, d, i+1, len, CharacterTypes.PlayerCharacter);
                }
            }
        }
        
        
        
        private static void CharacterMove(Rigidbody2D rigidbodyImpl, MobileJoystickImplement joystickImpl, CommonStatusImplement statusImpl, TranslateImplement translateImpl)
        {
            var normalizeValue = joystickImpl.playerInputProperty.statusValue.normalized;
            if (normalizeValue.sqrMagnitude > 0f == false)
            {
                rigidbodyImpl.velocity = Vector2.zero;
                return;
            }
            
            translateImpl.moveDirectionProperty.statusValue = normalizeValue;

            var dot = normalizeValue.x;
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            var div = (int)(angle / 22.5f);
            if (normalizeValue.y < 0f)
            {
                switch (div)
                {
                    case 0:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CR;
                        break;
                    case 1:
                    case 2:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DR;
                        break;
                    case 3:
                    case 4:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DC;
                        break;
                    case 5:
                    case 6:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.DL;
                        break;
                    case 7:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CL;
                        break;
                }
            }
            else
            {
                switch (div)
                {
                    case 0:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CR;
                        break;
                    case 1:
                    case 2:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UR;
                        break;
                    case 3:
                    case 4:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UC;
                        break;
                    case 5:
                    case 6:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.UL;
                        break;
                    case 7:
                        statusImpl.unitDirectionProperty.statusValue = UnitViewDirection.CL;
                        break;
                }
            }

            rigidbodyImpl.velocity = normalizeValue * translateImpl.velocityProperty.statusValue;
        }
        
        private static void NofityDeath(CommonStatusImplement statusImpl)
        {
            if (statusImpl.hitPointProperty.statusValue <= 0)
                MessageBroker.Default.Publish("gameOver");
        }

        private static void ProcGigantic(GiganticImplement gigantImpl, IEntityContainer entityContainer, Transform characterScalerImpl, BuffImplement buffImpl)
        {
            if (gigantImpl.giganticTimeProperty.statusValue > 0f)
            {
                gigantImpl.giganticTimeProperty.statusValue -= Time.deltaTime;
                gigantImpl.cameraFollowProperty.statusValue = Mathf.Min(1, gigantImpl.cameraFollowProperty.statusValue + (Time.deltaTime / GameSettings.GiganticCameraFollowDuration));

                if (gigantImpl.giganticTimeProperty.statusValue <= 0f)
                {
                    characterScalerImpl.transform.localScale = Vector3.one;
                    buffImpl.appliedBuff.RemoveAll(elmt => elmt.statusValue.buffDeviceId == GameSettings.GiganticBuffId);
                    entityContainer.playerCharacterEntity.OnApplyPassiveArcaneDevice();
                }
                else
                {
                    ApplyDamageToContactedEnemies(entityContainer);
                    CollectContactedItems(entityContainer);

                    bool isMoving = entityContainer.playerCharacterEntity.rigidBodyImplement.velocity.sqrMagnitude != 0;
                    bool isCool = GameSettings.GiganticDuration - gigantImpl.giganticTimeProperty.statusValue > GameSettings.GiganticEarthShakeInterval * gigantImpl.earthShakeCounterProperty.statusValue;
                    if (isMoving && isCool)
                    {
                        MessageBroker.Default.Publish("cameraShake");

                        gigantImpl.earthShakeCounterProperty.statusValue++;
                    }
                }
            }
            else
            {
                gigantImpl.cameraFollowProperty.statusValue = Mathf.Max(0, gigantImpl.cameraFollowProperty.statusValue - (Time.deltaTime / GameSettings.GiganticCameraFollowDuration));
            }
        }

        private static void ApplyDamageToContactedEnemies(IEntityContainer entityContainer)
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var colliders = Physics2D.OverlapCircleAll(playerCharacterEntity.transformImplement.position, (1 + GameSettings.GiganticScaleAdd) / 2, GameSettings.enemyLayerMask);

            foreach(var collider in colliders)
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
                            buffType = BuffType.Damage,
                            buffValue = 100,
                            remainTime = -1,
                            rootCharacter = new CharacterType() { statusValue = CharacterTypes.Token },
                            rootEntityId = 0
                        }
                    });

                    var knockBack = new KnockBackSendDataSet() { direction = KnockBackDirection.OppositeOfPlayer, distance = 2f };

                    enemyEntity.knockBackReceiveImplement.knockBackReceiveProperty.statusValue = knockBack.CalcKnockBackReceiveDataSet(playerCharacterEntity.transform.position, Vector2.zero, Vector2.zero, enemyEntity.transform.position);
                }
            }
        }

        private static void CollectContactedItems(IEntityContainer entityContainer)
        {
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var tokenEntities = entityContainer.GetEntities<TokenEntity>();

            var colliders = Physics2D.OverlapCircleAll(playerCharacterEntity.transformImplement.position, (1 + GameSettings.GiganticScaleAdd) / 2, GameSettings.tokenLayerMask);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<TokenEntity>(out var tokenEntity))
                {
                    if (tokenEntity.tokenStateImplement.tokenStateProperty.statusValue != TokenStateType.Idle)
                        continue;

                    var applyBuffImpl = tokenEntity.applyBuffImplement;
                    for (int i = 0; i < applyBuffImpl.applyBuffList.Count; ++i)
                    {
                        var buff = applyBuffImpl.applyBuffList[i];
                        if (buff.statusValue.buffType == BuffType.ExpUp)
                        {
                            var addExpStatus = buff.statusValue;
                            addExpStatus.buffValue *= (1f + playerCharacterEntity.characterStatusImplement.buffedexpMultipleProperty.statusValue);

                            buff.statusValue = addExpStatus;
                        }
                        playerCharacterEntity.buffImplement.appliedBuff.Add(buff);
                    }

                    tokenEntity.tokenStateImplement.tokenStateProperty.statusValue = TokenStateType.Catched;
                }
            }
        }

        private static void ProcMicrify(MicrifyImplement micrifyImpl, IEntityContainer entityContainer, BuffImplement buffImpl)
        {
            if (micrifyImpl.MicrifyTimeProperty.statusValue > 0f)
            {
                micrifyImpl.MicrifyTimeProperty.statusValue -= Time.deltaTime;

                if (micrifyImpl.MicrifyTimeProperty.statusValue <= 0f)
                {
                    entityContainer.playerCharacterEntity.transformImplement.localScale = Vector3.one;
                    buffImpl.appliedBuff.RemoveAll(elmt => elmt.statusValue.buffDeviceId == GameSettings.MicrifyBuffId);
                    entityContainer.playerCharacterEntity.OnApplyPassiveArcaneDevice();
                }
            }
        }

        private static void UpdateSanctumVfxManange(SanctumVfxManageImplement sanctumVfxManageImpl, IEntityContainer entityContainer, VfxFactory vfxFactory)
        {
            if(sanctumVfxManageImpl.LifeTimeProperty.statusValue > 0f)
            {
                sanctumVfxManageImpl.LifeTimeProperty.statusValue -= Time.deltaTime;
                if(sanctumVfxManageImpl.LifeTimeProperty.statusValue <= 0f)
                {
                    if (entityContainer.GetEntity(sanctumVfxManageImpl.VfxEgidProperty.statusValue, out VfxEntity oldVfx))
                    {
                        vfxFactory.EnqueRecycle(oldVfx, oldVfx.SrcPathHashCode);
                    }
                }
            }
        }

        public override async UniTask TeleportPosition(Vector3 position)
        {
            await base.TeleportPosition(position);

            RecycleEnemies(false, false);
        }

        public void RecycleEnemies(bool includeScreen, bool includeElite)
        {
            var enemies = entityContainer.GetEntities<EnemyCharacterEntity>();
            foreach (var enemy in enemies)
            {
                var screenCondition = includeScreen || viewCamera.IsInScreen(enemy.translateImplement.positionProperty.statusValue) == false;
                var typeCondition = enemy.monsterTypeImplement.monsterTypeDataProperty.statusValue == MonsterType.Common || (enemy.monsterTypeImplement.monsterTypeDataProperty.statusValue == MonsterType.Elite && includeElite);

                if (screenCondition && typeCondition)
                    enemyCharacterFactory.EnqueRecycle(enemy, enemy.SrcPathHashCode);
            }
        }

#endregion

    }
}
