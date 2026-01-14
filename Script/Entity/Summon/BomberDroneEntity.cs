using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class BomberDroneEntity : AttackDroneEntity
    {
        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            
            transformImplement.position = GetSeekingPosition(1);
            seekingTargetImplement.seekingTargetProperty.seekTargetPosition = GetSeekingPosition(1);
           
            seekingTargetImplement.seekingTargetProperty.seekingTime = 0.5f;
            seekingTargetImplement.seekingTargetProperty.startSeek = true;
            transformImplement.rotation = Quaternion.identity;

            seekingTargetImplement.reactiveCommand
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    seekingTargetImplement.seekingTargetProperty.seekTargetPosition = GetSeekingPosition(1);
                });
        }

        protected override void CreateIndicator()
        {
            var colorArray = new Color[2]
            {
                new Color(0f, 0.6f, 0.45f, 1f), new Color(0.8f, 0f, 0f, 1f)
            };

            var rotDirectionArray = new int[2] { -1, 1 };

            for (int i = 0; i < weaponInventoyImplement.WeaponDatas.Count; ++i)
            {
                var indicator = indicatorFactory.CreateGameObject("Prefabs/Indicator/DroneIndicator");
                if (indicator.TryGetComponent(out DroneAttackIndicatorEntity droneAttackIndicatorEntity))
                {
                    indicatorWeaponTarget.Add(new AttackDroneIndicatorData
                    {
                        indicatorEGID = droneAttackIndicatorEntity.EGID,
                        weaponDataSet = weaponInventoyImplement.WeaponDatas[i].statusValue,
                        appliedColor = colorArray[i % 2]
                    });
                }
            }

            for (int i = 0; i < indicatorWeaponTarget.Count; ++i)
            {
                var createdIndicator = indicatorWeaponTarget[i];
                if (entityContainer.GetEntity(createdIndicator.indicatorEGID, out DroneAttackIndicatorEntity entity))
                {
                    entity.rotateDirection = rotDirectionArray[i % 2];
                    entity.circleSprite.color = createdIndicator.appliedColor;
                    entity.droneAttackIndicatorImplement.reactiveCommand
                        .TakeUntilDisable(this)
                        .Subscribe(vec2 =>
                        {
                            OnPopAttackablePositions(vec2, createdIndicator.indicatorEGID);
                        });
                }
            }
        }
    }
}
