using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Reflex.Scripts.Attributes;
using UnityEngine.Playables;

namespace Game.ECS
{
    public class ResultView : UiView
    {
        public PlayableDirector playableDirector;

        public bool isRetreatView = false;

        public TextMeshProUGUI characterAlias;
        public TextMeshProUGUI characterName;

        public Button okButton;
        public Button statisticsButton;
        
        [Inject] private IEntityContainer entityContainer;

        public override void Init()
        {
            gameObject.SetActive(false);

            okButton.OnClickAsObservable().Subscribe(_ =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("GameStartScene");
            }).AddTo(this);

            /*
            statisticsButton.OnClickAsObservable().Subscribe(_ =>
            {
                Debug.Log("Show statistics view");
            }).AddTo(this);
            */

            var targetMessage = isRetreatView ? "gameOver" : "gameClear";
            var targetDialogue = isRetreatView ? CharacterSpeechType.GameOver : CharacterSpeechType.StageClear;
            var gameOverObservable = MessageBroker.Default.Receive<string>().Where(message => message == targetMessage).Take(1);
            gameOverObservable.Subscribe(message =>
            {
                entityContainer.playerCharacterEntity.mobileJoystickImplement.Pause();
            }).AddTo(this);

            gameOverObservable.Delay(System.TimeSpan.FromSeconds(1)).Subscribe(message =>
            {
                Time.timeScale = 0;
                MessageBroker.Default.Publish(targetDialogue);
                playableDirector.Play();
            }).AddTo(this);
        }
    }
}
