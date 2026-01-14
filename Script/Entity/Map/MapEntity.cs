using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.ECS
{
    public class MapEntity : Entity
    {
        public Transform transformImplement;
        public StartMapInfoImplement startMapInfoImplement;
        public EventMapSettingImplement settingImplement;
        public EventMapTriggerSettingImplement triggerSettingImplement;
        public MapBoundsImplement mapBoundsImplement;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            transformImplement = transform;
            

            if(TryGetComponent(out startMapInfoImplement))
            {
                Components.Add(startMapInfoImplement);
            }

            if (TryGetComponent(out settingImplement))
            {
                Components.Add(settingImplement);
            }

            if (TryGetComponent(out triggerSettingImplement))
            {
                Components.Add(triggerSettingImplement);
            }
            
            if (TryGetComponent(out mapBoundsImplement))
            {
                Components.Add(mapBoundsImplement);
            }
        }

        public override void ApplyComponentData()
        {
            mapBoundsImplement.tilemapBounds = GetComponentInChildren<Tilemap>().cellBounds;
        }
    }
}
