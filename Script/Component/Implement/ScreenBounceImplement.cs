using System;
using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace Game.ECS
{
    public class ScreenBounceImplement : MonoBehaviour, IComponent, ITickComponent
    {
        [Inject] private Camera cam;
        
        private Bounds2D screenBounce;
        public float fixedTickTime => 0.05f;
        public float currentTickTime { get; set; }
        private TranslateImplement translateImplement;
        // private bool changeDirectionX;
        // private bool changeDirectionY;

        public void InitializeComponent()
        {
            currentTickTime = 0f;
            // changeDirectionX = false;
            // changeDirectionY = false;
            TryGetComponent(out translateImplement);
        }

        private void CheckScreenBounce()
        {
            var position = (Vector2)transform.position;
            var moveDir = translateImplement.moveDirectionProperty.statusValue;
            if (screenBounce.Contains(position) == false)
            {
                // if (changeDirectionX || changeDirectionY)
                // {
                //     return;
                // }
                if (position.x < screenBounce.leftExtents && moveDir.x < 0f)
                {
                    moveDir.x *= -1f;
                }

                if (position.x > screenBounce.rightExtents && moveDir.x > 0f)
                {
                    moveDir.x *= -1f;
                }
                
                if (position.y < screenBounce.downExtents && moveDir.y < 0f)
                {
                    moveDir.y *= -1f;
                }

                if (position.y > screenBounce.upExtents && moveDir.y > 0f)
                {
                    moveDir.y *= -1f;
                }
                translateImplement.moveDirectionProperty.statusValue = moveDir;
            }
            else
            {
                // changeDirectionX = false;
                // changeDirectionY = false;
            }
        }

        
        public bool OnScanTime()
        {
            var lb = cam.ViewportToWorldPoint(new Vector3(0f, 0f));
            var lt = cam.ViewportToWorldPoint(new Vector3(0f, 1f));
            var rb = cam.ViewportToWorldPoint(new Vector3(1f, 0f));
            var rt = cam.ViewportToWorldPoint(new Vector3(1f, 1f));

            var center = (lb + rb + lt + rt) * 0.25f;
            var size = new Vector2(rb.x - lb.x, rt.y - rb.y);
            screenBounce = new Bounds2D(center, size);
            CheckScreenBounce();
            return true;
        }

        private void Update()
        {
            OnScanTime();
        }
    }
}
