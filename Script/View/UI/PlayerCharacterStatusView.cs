using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using UnityEngine.U2D;
using Reflex.Scripts.Attributes;

namespace Game.ECS
{
    public enum CharacterSpeechType
    {
        Start,
        LevelUp,
        GameOver,
        StageClear,
        BossEncounter,
        FoodItemText
    }

    public class PlayerCharacterStatusView : MonoBehaviour
    {
        public TextMeshProUGUI playerCharacterLevel;
        public RectTransform playerCharacterSpeech;
        public TextMeshProUGUI playerCharacterSpeechText;
        public TextMeshProUGUI playerCharacterTimelineSpeechText;
        public Image playerCharacterPortrait;
        public Slider playerCharacterExpGaugeFill;
        public AudioSource expAudioSource;

        [Inject] private MainGameSceneContextModel sceneModel;

        private ReactiveProperty<uint> exp = new();
        private ReactiveProperty<int> level = new(1);
        private ReactiveCommand<string> timeStopEvent = new();
        private uint nextLevelExp;

        void Start()
        {
            //playerCharacterPortrait.sprite = Resources.Load<SpriteAtlas>("SpriteAtlantes/UI_Etc").GetSprite(sceneModel.characterDataEntity.portraitName);

            MessageBroker.Default.Receive<KeyValuePair<string, uint>>().TakeUntil(MessageBroker.Default.Receive<string>().Where(message => message == "gameOver")).Subscribe(message =>
            {
                switch (message.Key)
                {
                    case "addExp":
                        exp.Value += message.Value;
                        expAudioSource.Play();
                        break;
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<CharacterSpeechType>().Subscribe(message =>
            {
                playerCharacterSpeech.gameObject.SetActive(true);
                var str = message switch
                {
                    CharacterSpeechType.Start => sceneModel.characterDataEntity.startText,
                    CharacterSpeechType.LevelUp => sceneModel.characterDataEntity.levelUpText,
                    CharacterSpeechType.GameOver => sceneModel.characterDataEntity.gameOverText,
                    CharacterSpeechType.StageClear => sceneModel.characterDataEntity.stageClearText,
                    CharacterSpeechType.BossEncounter => sceneModel.characterDataEntity.bossEncounterText,
                    CharacterSpeechType.FoodItemText => sceneModel.characterDataEntity.foodItemText,
                    _ => "",
                };
                playerCharacterTimelineSpeechText.text = playerCharacterSpeechText.text = str;
            }).AddTo(this);

            MessageBroker.Default.Receive<CharacterSpeechType>().Throttle(System.TimeSpan.FromSeconds(3)).Subscribe(value =>
            {
                playerCharacterSpeech.gameObject.SetActive(false);
            }).AddTo(this);

            level.CombineLatest(exp, (level, exp) => (level, exp)).Subscribe(data =>
            {
                playerCharacterExpGaugeFill.value = data.exp / (float)GameSettings.GetRequiredExpForNextLevel(data.level);
            }).AddTo(this);

            exp.Subscribe(value =>
            {
                var expForLevelUp = GameSettings.GetRequiredExpForNextLevel(level.Value);
                if (value >= expForLevelUp)
                {
                    level.Value += 1;
                    exp.Value -= expForLevelUp;
                }
            }).AddTo(this);

            level.Subscribe(value =>
            {
                playerCharacterLevel.text = value.ToString();
            }).AddTo(this);

            level.Skip(1).Subscribe(value =>
            {
                MessageBroker.Default.Publish(CharacterSpeechType.LevelUp);
                timeStopEvent.Execute("selectDeviceByLevelUp");
            }).AddTo(this);

            MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch(message)
                {
                    case "requestDeviceBoxEvent":
                        timeStopEvent.Execute("deviceBoxEvent");
                        break;
                }
            });

            var timeStopEventDone = MessageBroker.Default.Receive<string>().Where(message => message == "timeStopEventDone").StartWith("timeStopEventDone");
            timeStopEvent.Zip(timeStopEventDone, (selection, _) => selection).Subscribe(eventName =>
            {
                MessageBroker.Default.Publish(eventName);
            }).AddTo(this);


            MessageBroker.Default.Publish(CharacterSpeechType.Start);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                exp.Value += GameSettings.GetRequiredExpForNextLevel(level.Value);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                exp.Value += GameSettings.GetRequiredExpForNextLevel(level.Value) + GameSettings.GetRequiredExpForNextLevel(level.Value + 1);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                exp.Value += GameSettings.GetRequiredExpForNextLevel(level.Value) + GameSettings.GetRequiredExpForNextLevel(level.Value + 1) + GameSettings.GetRequiredExpForNextLevel(level.Value + 2);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                MessageBroker.Default.Publish("requestDeviceBoxEvent");
            }
        }
    }
}

