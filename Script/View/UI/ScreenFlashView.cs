using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS
{
    public class ScreenFlashView : MonoBehaviour
    {
        public Image white;
        public float duration = 0.5f;

        Tween tweening;

        private void Start()
        {
            MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch (message)
                {
                    case "screenFlash":
                        if (tweening != null)
                            tweening.Kill();
                        white.color = Color.white;
                        tweening = white.DOFade(0, duration).SetEase(Ease.InCubic);
                        break;
                };
            }).AddTo(this);
        }
    }
}
