using UniRx;
using UnityEngine;

namespace Game.ECS
{
    
    public class MobileJoystickImplement : MonoBehaviour, IComponent
    {
        [SerializeField] private TouchAxisCtrl touchAxisCtrl;
        private PlayerInput playerInput;

        public ref PlayerInput playerInputProperty => ref playerInput;

        private void Update()
        {
            if(touchAxisCtrl.gameObject.activeInHierarchy)
            {
                playerInputProperty.statusValue = touchAxisCtrl.spawnOnTouch ? touchAxisCtrl.Axis.normalized : Vector2.zero;

                if(playerInputProperty.statusValue == Vector2.zero)
                {
                    float x = 0;
                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                        x -= 1;
                    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                        x += 1;
                    float y = 0;
                    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                        y -= 1;
                    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                        y += 1;
                    playerInputProperty.statusValue = new Vector2(x, y).normalized;
                }
            }
            else
            {
                playerInputProperty.statusValue = Vector2.zero;
            }
        }

        public void Pause()
        {
            touchAxisCtrl.Lock();
            touchAxisCtrl.gameObject.SetActive(false);
        }

        public void Resume()
        {
            touchAxisCtrl.Unlock();
            touchAxisCtrl.gameObject.SetActive(true);
        }

        public bool IsPaused()
        {
            return touchAxisCtrl.gameObject.activeInHierarchy == false;
        }

        public void InitializeComponent()
        {
            
        }
    }

}
