using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class AxeEntity : AttackEntity
    {
        public Transform childTransformImplment;
        

        public override void ApplyComponentData()
        {
            childTransformImplment = spriterendererImplement.transform;
            
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            transformImplement.localPosition = playerCharacterEntity.transformImplement.localPosition;
            childTransformImplment.transform.localRotation = Quaternion.identity;
            
            
            //var addProjSpeedRatio = EntityContainer.GetEntities<PlayerCharacterEntity>().First().characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue + 1f;
            rigidBodyImplement.gravityScale = 2f;
        }

        protected override void Update()
        {
            base.Update();
            RotateObject(childTransformImplment, 1500f);
        }

        public override void OnApplyShootCountChanged()
        {
            var count = shootCounterImplement.shootCountProperty.statusValue;
            //var addProjSpeedRatio = EntityContainer.GetEntities<PlayerCharacterEntity>().First().characterStatusImplement.buffedprojectileSpeedRatioProperty.statusValue + 1f;
            var upsideForce = Random.Range(500f, 900f);
            var rightsideForce = Random.Range(50f, 150f);
            rigidBodyImplement.AddRelativeForce(count % 2 > 0 ? new Vector2(rightsideForce, upsideForce) : new Vector2(rightsideForce * -1f, upsideForce));
        }
    }
}
