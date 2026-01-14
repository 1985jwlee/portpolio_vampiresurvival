using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Game.ECS
{
    public interface ICameraFollowSystem : ITickGameSystem
    {
        
    }
    
    public class CameraFollowSystem : ICameraFollowSystem, IDisposable
    {
        private readonly Camera camera;
        private readonly IEntityContainer entityContainer;
        private readonly TickGameSystemManager tickGameSystemManager;
        private readonly MainGameSceneContextModel mainGameSceneContextModel;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public int ExcuteOrder => 150;

        public CameraFollowSystem(Camera cam, IEntityContainer _entityContainer, TickGameSystemManager _tickGameSystem, MainGameSceneContextModel _mainGameSceneContextModel)
        {
            camera = cam;
            entityContainer = _entityContainer;
            tickGameSystemManager = _tickGameSystem;
            mainGameSceneContextModel = _mainGameSceneContextModel;
        }


        public void InitGameSystem()
        {
            disposables.Add(MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch (message)
                {
                    case "cameraShake":
                        var characterEntity = entityContainer.playerCharacterEntity;
                        CameraSyncImplement cameraSyncImpl = characterEntity.cameraSyncImplement;
                        cameraSyncImpl.ShakeCamera(camera);
                        break;
                }
            }));

            tickGameSystemManager.AddTickSystem(this);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            
        }

        
        public void OnUpdate()
        {
            var bounds = mainGameSceneContextModel.mapBound;
            bounds = new Bounds(mainGameSceneContextModel.mapBound.center + Vector3.up * 0.5f, mainGameSceneContextModel.mapBound.size + Vector3.one * 2 + Vector3.up); // ToDo: 지울 것. 임시 테두리를 위한 처리
            var characterEntity = entityContainer.playerCharacterEntity;
            CameraSyncImplement cameraSyncImpl = characterEntity.cameraSyncImplement;
            var currentCenteringWorld = camera.ViewportToWorldPoint(Vector3.one * 0.5f);
            var needMove = entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue - currentCenteringWorld;
            needMove.z = 0f;
            needMove += entityContainer.playerCharacterEntity.giganticImplement.CalcCameraOffset(entityContainer.playerCharacterEntity.characterScalerImplement.transform.localPosition.y);
            camera.transform.parent.Translate(needMove);
            cameraSyncImpl.MoveTileCamera(camera, Vector2.zero, bounds);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
