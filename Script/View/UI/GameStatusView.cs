using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Game.ECS;
using Reflex.Scripts.Attributes;

public class GameStatusView : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI killText;
    public Button pauseButton;

    [Inject] private IEntityContainer entityContainer;
    [Inject] private TimelineManager timelineManager;
    [Inject] private IMonsterKillAchivementSystem monsterKillAchivementSystem;
    [Inject] private IGetGoldAchievementSystem getGoldAchievementSystem;

    private void Start()
    {
        pauseButton.OnClickAsObservable().
            TakeUntilDestroy(this).
            Subscribe(_ =>
            {
                MessageBroker.Default.Publish("pause");
            });
        
        getGoldAchievementSystem.reactiveProperty
            .TakeUntil(MessageBroker.Default.Receive<string>().Where(message => message == "gameOver"))
            .Subscribe(gold =>
            {
                goldText.text = gold.ToString();
            });

        monsterKillAchivementSystem.reactiveProperty
            .TakeUntil(MessageBroker.Default.Receive<string>().Where(message => message == "gameOver"))
            .Subscribe(kill =>
            {
                killText.text = kill.ToString();
            });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (entityContainer.playerCharacterEntity.mobileJoystickImplement.IsPaused() == false)
                MessageBroker.Default.Publish("pause");
        }

        timeText.text = System.TimeSpan.FromSeconds(timelineManager.time).ToString(@"mm\:ss");
    }
}
