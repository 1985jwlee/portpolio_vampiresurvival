using System.Collections.Generic;
using System.Linq;
using Reflex;
using UnityEngine;

namespace Game.ECS
{
    public class ItemBoxFactory : GameObjectFactory
    {
        private IEntityContainer entityContainer;
        private TimelineManager timelineManager;
        private TableDataHolder tableDataHolder;
        private const float generateCheckTime = 5f;
        private float currentTime = 0f;
        private ObjectGenAreaType recentSelection = ObjectGenAreaType.CL;
        
        
        public ItemBoxFactory(Container _container, TimelineManager _timelineManager, IEntityContainer _entityContainer, TableDataHolder _tableDataHolder)
        {
            container = _container;
            timelineManager = _timelineManager;
            entityContainer = _entityContainer;
            tableDataHolder = _tableDataHolder;
        }
        
        protected override void OnCreateGameObject(Entity o, int pathHash)
        {
            o.SrcPathHashCode = pathHash;
            o.OnCreateEntity();
        }
        
        private Dictionary<ObjectGenAreaType, Bounds2D> GetObjectAreas(Vector2 position)
        {
            var ret = new Dictionary<ObjectGenAreaType, Bounds2D>();
            ret.Add(ObjectGenAreaType.CC, new Bounds2D(position, Vector2.one * 8f));
            ret.Add(ObjectGenAreaType.UL, new Bounds2D(new Vector2(position.x - 11f, position.y + 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.UC,new Bounds2D(new Vector2(position.x, position.y + 11f), new Vector2(8f, 14f)));
            ret.Add(ObjectGenAreaType.UR, new Bounds2D(new Vector2(position.x + 11f, position.y + 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.CL, new Bounds2D(new Vector2(position.x - 11f, position.y), new Vector2(14f, 8f)));
            ret.Add(ObjectGenAreaType.CR, new Bounds2D(new Vector2(position.x + 11f, position.y), new Vector2(14f, 8f)));
            ret.Add(ObjectGenAreaType.DL,  new Bounds2D(new Vector2(position.x - 11f, position.y - 11f), new Vector2(14f, 14f)));
            ret.Add(ObjectGenAreaType.DC, new Bounds2D(new Vector2(position.x, position.y - 11f), new Vector2(8f, 14f)));
            ret.Add(ObjectGenAreaType.DR, new Bounds2D(new Vector2(position.x + 11f, position.y - 11f), new Vector2(14f, 14f)));
            return ret;
        }
        
        public bool TryCreateItemBox(out Vector2 position)
        {
            position = Vector2.zero;
            var currentBoxEntity = entityContainer.GetEntities<ItemBoxEntity>();
            if (currentBoxEntity.Count < 3 == false)
            {
                return false;
            }
            
            var playerCharacterEntity = entityContainer.playerCharacterEntity;
            var maps = entityContainer.GetEntities<MapEntity, MapBoundsImplement>();
            
            var area = GetObjectAreas(playerCharacterEntity.translateImplement.positionProperty.statusValue);
            var boxColliderWalls = Object.FindObjectsOfType<WallObjectCollisionImplement>().Where(_x => _x.GetComponent<BoxCollider2D>() != null).Select(_x => _x.GetComponent<BoxCollider2D>()).ToList();

            var linkedList = new LinkedList<ObjectGenAreaType>();
            linkedList.AddFirst(ObjectGenAreaType.UL);
            linkedList.AddFirst(ObjectGenAreaType.UC);
            linkedList.AddFirst(ObjectGenAreaType.UR);
            linkedList.AddFirst(ObjectGenAreaType.CR);
            linkedList.AddFirst(ObjectGenAreaType.DR);
            linkedList.AddFirst(ObjectGenAreaType.DC);
            linkedList.AddFirst(ObjectGenAreaType.DL);
            linkedList.AddFirst(ObjectGenAreaType.CL);

            var checkCount = 40;
            var startAreaNode = linkedList.Find(linkedList.Random());
            while (checkCount > 0)
            {
                if (recentSelection == startAreaNode.Value)
                {
                    if (Random.value > 0.5f)
                    {
                        startAreaNode = startAreaNode.CircularNextOrFirst();
                        checkCount--;
                        continue;
                    }
                }
                if (area.TryGetValue(startAreaNode.Value, out var bound))
                {
                    var randomPoint = bound.GetRandomPoint();
                    var isOutMap = false;
                    foreach (var map in maps)
                    {
                        var mapBounds = new Bounds2D(map.mapBoundsImplement.tilemapBounds);
                        if (mapBounds.Contains(randomPoint) == false)
                        {
                            isOutMap = true;
                        }
                    }

                    if (isOutMap == false)
                    {
                        foreach (var collider in boxColliderWalls)
                        {
                            var boxToBounds = new Bounds2D(collider.bounds.center, collider.bounds.size);
                            if (boxToBounds.Contains(randomPoint) == false)
                            {
                                recentSelection = startAreaNode.Value;
                                position = randomPoint;
                                return true;
                            }
                        }
                    }
                }
                checkCount--;
                startAreaNode = startAreaNode.CircularNextOrFirst();
            }
            return false;
        }

        public void Tick()
        {
            if (timelineManager.pause)
            {
                return;
            }

            currentTime += Time.deltaTime;
            if (currentTime > generateCheckTime)
            {
                currentTime -= generateCheckTime;
                if (TryCreateItemBox(out var position) == false)
                {
                    return;
                }
                
                Debug.Log($"ItemBox Create Position = ({position.x},{position.y})");

                var mapObjectDataEntity = tableDataHolder.MapObjectCollection.GetIterator()[0];

                var go = CreateGameObject(mapObjectDataEntity.prefabPath);
                go.transform.position = new Vector3(position.x, position.y, 0f);
                SetOnKilledEventData(go.GetComponent<ItemBoxEntity>().onKilledEventImplement, mapObjectDataEntity);
            }
        }

        private void SetOnKilledEventData(OnKilledEventImplement onKilledEventImplement, MapObjectDataEntity mapObjectDataEntity)
        {
            foreach (var dropId in mapObjectDataEntity.itemDropIds)
            {
                if (tableDataHolder.ItemDropCollection.TryGetEntity(dropId, out var ItemDropDataEntity))
                {
                    var index = ExtensionFunction.PerTenThousandRandomIndex(ItemDropDataEntity.itemProps);
                    if (index == -1)
                        continue;

                    onKilledEventImplement.itemIdsToDrop.Add(new ItemIdToDrop() { statusValue = ItemDropDataEntity.itemIds[index] });
                }
            }
        }
    }
}
