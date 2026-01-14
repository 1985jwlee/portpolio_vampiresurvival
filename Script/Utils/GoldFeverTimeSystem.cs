using Reflex;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public interface IGoldFeverTimeSystem
    {
        bool IsGoldFeverTime { get; }
        float GoldFeverRamainTime { get; }
    }

    public class GoldFeverTimeSystem : IGoldFeverTimeSystem, System.IDisposable
    {
        public bool IsGoldFeverTime => goldFeverTime > 0f;
        public float GoldFeverRamainTime => goldFeverTime;

        private float goldFeverTime = 0f;

        private readonly IEntityContainer entityContainer;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public GoldFeverTimeSystem(IEntityContainer _entityContainer)
        {
            entityContainer = _entityContainer;

            disposables.Add(MessageBroker.Default.Receive<string>().Subscribe(message =>
            {
                switch (message)
                {
                    case "goldFever":
                        {
                            bool wasGoldFeverAlready = IsGoldFeverTime;
                            goldFeverTime = GameSettings.GoldFeverTime;

                            if (wasGoldFeverAlready == false)
                            {
                                var enemyEntities = entityContainer.GetEntities<EnemyCharacterEntity>();

                                foreach (var entity in enemyEntities)
                                {
                                    entity.spriterendererImplement.color = MaterialProperties.GoldColor;
                                    entity.grayscaleImplement.enableGrayScale = true;
                                }

                                MessageBroker.Default.Publish("showGoldFeverView");
                            }
                        }
                        break;
                }
            }));

            disposables.Add(MessageBroker.Default.Receive<KeyValuePair<string, float>>().Subscribe(message =>
            {
                switch (message.Key)
                {
                    case "goldFeverTimeUp":
                        if (IsGoldFeverTime)
                            goldFeverTime = Mathf.Min(goldFeverTime + message.Value, GameSettings.GoldFeverTime);
                        break;
                }
            }));

            Observable.EveryUpdate().Subscribe(_=>OnUpdate());
        }

        public void OnUpdate()
        {
            if(goldFeverTime > 0f)
            {
                goldFeverTime -= Time.deltaTime;

                if (goldFeverTime < 0f)
                {
                    var enemyEntities = entityContainer.GetEntities<EnemyCharacterEntity>();

                    foreach(var entity in enemyEntities)
                    {
                        entity.spriterendererImplement.color = Color.white;
                        entity.grayscaleImplement.enableGrayScale = false;
                    }

                    MessageBroker.Default.Publish("hideGoldFeverView");
                }
            }
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
