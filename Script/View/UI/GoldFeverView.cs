using Reflex.Scripts.Attributes;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Game.ECS
{
    public class GoldFeverView : UiView
    {
        public TextMeshProUGUI gatheredGoldText;
        public TextMeshProUGUI remainTimeText;
        public Slider goldFeverRemainTimeGauge;
        public PlayableDirector playableDirector;

        [Inject] IGoldFeverTimeSystem goldFeverSystem;

        private uint gatheredGolds = new();
        private float goldFeverTimeMax = new();

        public override void Init()
        {
            gameObject.SetActive(false);

            MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch(message)
                {
                    case "showGoldFeverView":
                        playableDirector.Play();
                        gatheredGolds = 0;
                        goldFeverTimeMax = 0f;
                        break;
                    case "hideGoldFeverView":
                        playableDirector.Stop();
                        break;

                }
            }).AddTo(this);

            MessageBroker.Default.Receive<KeyValuePair<string, uint>>().Where(_=>gameObject.activeInHierarchy).Subscribe(message =>
            {
                switch(message.Key)
                {
                    case "addGold":
                        gatheredGolds += message.Value;
                        break;
                }
            }).AddTo(this);
        }

        public void Update()
        {
            if(goldFeverSystem.IsGoldFeverTime)
            {
                if (goldFeverTimeMax < goldFeverSystem.GoldFeverRamainTime)
                    goldFeverTimeMax = goldFeverSystem.GoldFeverRamainTime;

                goldFeverRemainTimeGauge.value = goldFeverTimeMax == 0? 1 : goldFeverSystem.GoldFeverRamainTime / goldFeverTimeMax;
                gatheredGoldText.text = gatheredGolds.ToString("N0");

                TimeSpan time = TimeSpan.FromSeconds(goldFeverSystem.GoldFeverRamainTime);
                remainTimeText.text = $"{time.ToString("ss")}sec";
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
