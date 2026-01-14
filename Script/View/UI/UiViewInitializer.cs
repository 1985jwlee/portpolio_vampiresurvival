using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class UiViewInitializer : MonoBehaviour
    {
        [SerializeField] private UiView[] uiViews;

        public void Start()
        {
            foreach (var uiView in uiViews)
                uiView.Init();
        }
    }
}
