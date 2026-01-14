using DG.Tweening;
using UnityEngine;

namespace Game.ECS
{
    public class CameraSyncImplement : MonoBehaviour, IComponent
    {
        private float CameraLeftBound(Camera _camera, Bounds bounds) => bounds.min.x + CameraHorizontalExtent(_camera);
        private float CameraRightBound(Camera _camera, Bounds bounds) => bounds.max.x - CameraHorizontalExtent(_camera);
        private float CameraBottomBound(Camera _camera, Bounds bounds) => bounds.min.y + CameraVerticalExtent(_camera);
        private float CameraTopBound(Camera _camera, Bounds bounds) => bounds.max.y - CameraVerticalExtent(_camera);
        private float CameraVerticalExtent(Camera _camera) => _camera.orthographicSize;

        private float CameraHorizontalExtent(Camera _camera) => _camera.aspect * CameraVerticalExtent(_camera);

        public void MoveTileCamera(Camera _camera, Vector2 direction, Bounds bounds)
        {
            var curpos = _camera.transform.parent.localPosition;
            var xDelta = Mathf.Clamp(curpos.x - direction.x, CameraLeftBound(_camera, bounds), CameraRightBound(_camera, bounds));
            var yDelta = Mathf.Clamp(curpos.y - direction.y, CameraBottomBound(_camera, bounds), CameraTopBound(_camera, bounds));
            curpos.x = xDelta;
            curpos.y = yDelta;
            _camera.transform.parent.localPosition = curpos;
        }

        public void ShakeCamera(Camera _camera)
        {
            _camera.DOShakePosition(0.25f, 1f).OnComplete(()=>_camera.transform.localPosition = Vector3.zero);
        }

        public void InitializeComponent()
        {
            
        }
    }
}
