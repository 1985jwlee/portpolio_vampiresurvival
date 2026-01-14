using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class AttackDroneAlphaEntity : AttackDroneEntity
    {
        protected override void CreateIndicator()
        {
            
            foreach (WeaponData weaponData in weaponInventoyImplement.WeaponDatas)
            {
                var indicator = indicatorFactory.CreateGameObject("Prefabs/Indicator/DroneIndicator");
                if (indicator.TryGetComponent(out DroneAttackIndicatorEntity droneAttackIndicatorEntity))
                {
                    indicatorWeaponTarget.Add(new AttackDroneIndicatorData
                    {
                        indicatorEGID = droneAttackIndicatorEntity.EGID,
                        weaponDataSet = weaponData.statusValue,
                        appliedColor = new Color(0.8f, 0f, 0f, 1f)
                    });
                }
            }
            
            foreach (var createdIndicator in indicatorWeaponTarget)
            {
                if (entityContainer.GetEntity(createdIndicator.indicatorEGID, out DroneAttackIndicatorEntity entity))
                {
                    entity.rotateDirection = -1;
                    entity.droneAttackIndicatorImplement.reactiveCommand
                        .TakeUntilDisable(this)
                        .Subscribe(vec2 =>
                        {
                            OnPopAttackablePositions(vec2, createdIndicator.indicatorEGID);
                        });
                    entity.circleSprite.color = createdIndicator.appliedColor;
                }
            }
        }
    }
}
