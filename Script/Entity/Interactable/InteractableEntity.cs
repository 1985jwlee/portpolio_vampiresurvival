using Cysharp.Threading.Tasks;
using Reflex.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class InteractableEntity : Entity
    {
        [Inject] private DummyFactory dummyFactory;
        [Inject] private VfxFactory vfxFactory;

        public Transform transformImplement;
        public SpriteRenderer spriteRendererImplement;
        public InteractableColliderImplement interactableColliderImplement;
        public InteractableTeleportImplement interactableTeleportImplement;
        public InteractableSanctumImplement interactableSanctumImplement;
        public GrayScaleImplement grayscaleImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();

            transformImplement = transform;
            TryGetComponent(out spriteRendererImplement);

            if(TryGetComponent(out interactableColliderImplement))
            {
                Components.Add(interactableColliderImplement);
            }
            if (TryGetComponent(out interactableTeleportImplement))
            {
                Components.Add(interactableTeleportImplement);
            }
            if (TryGetComponent(out interactableSanctumImplement))
            {
                Components.Add(interactableSanctumImplement);
            }
            if (TryGetComponent(out grayscaleImplement))
            {
                Components.Add(grayscaleImplement);
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateCoolDownTimer(interactableColliderImplement);
            UpdateActivationTimer(interactableColliderImplement);

            if(interactableTeleportImplement != null)
            {
                InitTeleportFx(interactableTeleportImplement);
                UpdateTeleportActivation(interactableColliderImplement, interactableTeleportImplement, entityContainer, EGID);
                UpdateTeleportFx(interactableColliderImplement, interactableTeleportImplement, entityContainer);
            }

            if(interactableSanctumImplement != null)
            {
                UpdateSanctumActivation(interactableColliderImplement, interactableSanctumImplement, entityContainer, vfxFactory, EGID);
            }

            ResetActivationTimer(interactableColliderImplement);
            UpdateDeactivationGrayScale(interactableColliderImplement, grayscaleImplement);
            UpdateSpriteGrayScale(spriteRendererImplement, grayscaleImplement);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            foreach(var egid in interactableTeleportImplement.fxEgids)
            {
                if (entityContainer.GetEntity(egid.statusValue, out DummyEntity entity))
                {
                    dummyFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                }
            }
        }

        private static void UpdateCoolDownTimer(InteractableColliderImplement interactableColliderImplement)
        {
            if (interactableColliderImplement.CoolDownDurationTimerProperty.statusValue > 0f)
                interactableColliderImplement.CoolDownDurationTimerProperty.statusValue -= Time.deltaTime;
        }

        private static void UpdateActivationTimer(InteractableColliderImplement interactableColliderImplement)
        {
            if (interactableColliderImplement.CoolDownDurationTimerProperty.statusValue > 0f || interactableColliderImplement.IsActivatingProperty.statusValue == false)
                interactableColliderImplement.ActivationDurationTimerProperty.statusValue = 0;
            else
                interactableColliderImplement.ActivationDurationTimerProperty.statusValue += Time.deltaTime;
        }

        private void UpdateDeactivationGrayScale(InteractableColliderImplement interactableColliderImpl, GrayScaleImplement grayscaleImpl)
        {
            if (interactableColliderImpl.CoolDownDurationTimerProperty.statusValue > 0f)
                grayscaleImpl.enableGrayScale = true;
            else
                grayscaleImpl.enableGrayScale = false;
        }

        private static void UpdateSpriteGrayScale(SpriteRenderer spriteRendererImpl, GrayScaleImplement grayscaleImpl)
        {
            var propertyBlock = new MaterialPropertyBlock();
            spriteRendererImpl.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInteger(MaterialProperties.GrayScaleId, grayscaleImpl.enableGrayScale ? 1 : 0);
            spriteRendererImpl.SetPropertyBlock(propertyBlock);
        }

        private static void ResetActivationTimer(InteractableColliderImplement interactableColliderImplement)
        {
            if (interactableColliderImplement.ActivationDurationTimerProperty.statusValue > interactableColliderImplement.ActivationDurationProperty.statusValue)
                interactableColliderImplement.ActivationDurationTimerProperty.statusValue = 0;
        }

#region Teleport
        private void InitTeleportFx(InteractableTeleportImplement interactableTeleportImpl)
        {
            if(interactableTeleportImpl.IsInitializedProperty.statusValue == false)
            {
                foreach (var positions in interactableTeleportImplement.teleportFxPositions)
                {
                    var dummyGo = dummyFactory.CreateGameObject("Prefabs/Dummy/TeleportFx");
                    if (dummyGo.TryGetComponent(out DummyEntity dummyEntity))
                    {
                        dummyEntity.transformImplement.position = (Vector2)transformImplement.position + positions.statusValue;
                        dummyEntity.spriteRendererImplement.enabled = false;
                        interactableTeleportImpl.fxEgids.Add(new FxEGID { statusValue = dummyEntity.EGID });
                    }
                }
                interactableTeleportImpl.IsInitializedProperty.statusValue = true;
            }
        }

        private static void UpdateTeleportActivation(InteractableColliderImplement interactableColliderImpl, InteractableTeleportImplement interactableTeleportImpl, IEntityContainer entityContainer, uint EGID)
        {
            if (interactableColliderImpl.ActivationDurationTimerProperty.statusValue > interactableColliderImpl.ActivationDurationProperty.statusValue)
            {
                var interactableEntities = entityContainer.GetEntities<InteractableEntity>();
                foreach(var entity in interactableEntities)
                {
                    if (entity.EGID == EGID)
                        continue;

                    if (entity.interactableTeleportImplement != null)
                    {
                        if(entity.interactableTeleportImplement.TeleportIdProperty.statusValue == interactableTeleportImpl.TeleportIdProperty.statusValue)
                        {
                            entityContainer.playerCharacterEntity.TeleportPosition((Vector2)entity.transformImplement.position + entity.interactableTeleportImplement.TeleportPositionProperty.statusValue).Forget();
                            entity.interactableColliderImplement.CoolDownDurationTimerProperty.statusValue = entity.interactableColliderImplement.CoolDownDurationProperty.statusValue;
                        }
                    }
                }
            }
        }

        private static void UpdateTeleportFx(InteractableColliderImplement interactableColliderImpl, InteractableTeleportImplement interactableTeleportImpl, IEntityContainer entityContainer)
        {
            if(interactableColliderImpl.CoolDownDurationTimerProperty.statusValue <= 0f)
            {
                var rate = interactableColliderImpl.ActivationDurationTimerProperty.statusValue / interactableColliderImpl.ActivationDurationProperty.statusValue;

                {
                    if (entityContainer.GetEntity(interactableTeleportImpl.fxEgids[0].statusValue, out DummyEntity entity))
                        entity.spriteRendererImplement.enabled = rate > 0.0f;
                }
                {
                    if (entityContainer.GetEntity(interactableTeleportImpl.fxEgids[1].statusValue, out DummyEntity entity))
                        entity.spriteRendererImplement.enabled = rate > 0.3f;
                }
                {
                    if (entityContainer.GetEntity(interactableTeleportImpl.fxEgids[2].statusValue, out DummyEntity entity))
                        entity.spriteRendererImplement.enabled = rate > 0.6f;
                }
                {
                    if (entityContainer.GetEntity(interactableTeleportImpl.fxEgids[3].statusValue, out DummyEntity entity))
                        entity.spriteRendererImplement.enabled = rate > 0.9f;
                }
            }
        }
#endregion

#region Sanctum
        private static void UpdateSanctumActivation(InteractableColliderImplement interactableColliderImpl, InteractableSanctumImplement interactableSanctumImpl, IEntityContainer entityContainer, VfxFactory vfxFactory, uint EGID)
        {
            if (interactableColliderImpl.ActivationDurationTimerProperty.statusValue > interactableColliderImpl.ActivationDurationProperty.statusValue)
            {
                entityContainer.playerCharacterEntity.buffImplement.appliedBuff.RemoveAll(elmt => elmt.statusValue.rootCharacter.statusValue == CharacterTypes.Sanctum);

                var characterType = new CharacterType() { statusValue = CharacterTypes.Sanctum };
                BuffData buffData = interactableSanctumImpl.SanctumTypeProperty.statusValue switch
                {
                    SanctumType.Experience => new BuffData() { buffValue = 1, buffType = BuffType.ExperienceIncrement, remainTime = 60f, rootCharacter = characterType, rootEntityId = EGID},
                    SanctumType.Speed => new BuffData() { buffValue = 1, buffType = BuffType.MoveSpeedIncrement, remainTime = 60f, rootCharacter = characterType, rootEntityId = EGID },
                    SanctumType.Heal => new BuffData() { buffValue = 5, buffType = BuffType.HealthRegenIncrement, remainTime = 60f, rootCharacter = characterType, rootEntityId = EGID },
                    _=> new BuffData() { buffValue = 1, buffType = BuffType.ExperienceIncrement, remainTime = 60f, rootCharacter = characterType, rootEntityId = EGID },
                };
                entityContainer.playerCharacterEntity.buffImplement.appliedBuff.Add(new Buff() { statusValue = buffData });

                if(entityContainer.GetEntity(entityContainer.playerCharacterEntity.sanctumVfxManageImplement.VfxEgidProperty.statusValue, out VfxEntity oldVfx))
                {
                    vfxFactory.EnqueRecycle(oldVfx, oldVfx.SrcPathHashCode);
                }

                var vfxPath = interactableSanctumImpl.SanctumTypeProperty.statusValue switch
                {
                    SanctumType.Experience => GameSettings.SanctumExperienceVfxPath,
                    SanctumType.Speed => GameSettings.SanctumSpeedVfxPath,
                    SanctumType.Heal => GameSettings.SanctumHealVfxPath,
                    _ => GameSettings.SanctumExperienceVfxPath,
                };
                var sanctumVfx = vfxFactory.CreateGameObject(vfxPath);
                var vfxEntity = sanctumVfx.GetComponent<VfxEntity>();
                vfxEntity.seekingTargetImplement.seekingTargetProperty.seekTargetEGID = entityContainer.playerCharacterEntity.EGID;
                vfxEntity.seekingTargetImplement.seekingTargetProperty.seekingTime = -1f;
                vfxEntity.seekingTargetImplement.seekingTargetProperty.startSeek = true;

                entityContainer.playerCharacterEntity.sanctumVfxManageImplement.VfxEgidProperty.statusValue = vfxEntity.EGID;
                entityContainer.playerCharacterEntity.sanctumVfxManageImplement.LifeTimeProperty.statusValue = 60f;

                interactableColliderImpl.CoolDownDurationTimerProperty.statusValue = interactableColliderImpl.CoolDownDurationProperty.statusValue;
            }
        }
#endregion
    }

}
