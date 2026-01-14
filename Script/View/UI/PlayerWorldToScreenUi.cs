using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerWorldToScreenUi : MonoBehaviour
    {
        [SerializeField] private Vector2 worldOffset;
        [SerializeField] private Vector2 screenOffset;

        private Camera mainCam;
        private RectTransform parentRectTransform;
        private RectTransform rectTransform;

        [Inject] private IEntityContainer entityContainer;

        private void Awake()
        {
            mainCam = Camera.main;
            rectTransform = GetComponent<RectTransform>();
            parentRectTransform = rectTransform.parent.GetComponentInParent<RectTransform>();
        }

        private void Update()
        {
            var rectPos = ExtensionFunction.WorldPostionToCanvasPosition(mainCam, parentRectTransform, entityContainer.playerCharacterEntity.translateImplement.positionProperty.statusValue + (Vector3)worldOffset);
            rectTransform.anchoredPosition = (Vector2)rectPos + screenOffset;
        }
    }
}
