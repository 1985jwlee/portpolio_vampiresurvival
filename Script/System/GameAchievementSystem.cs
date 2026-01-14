using System;
using System.Collections.Generic;
using UniRx;

namespace Game.ECS
{

    public enum AchievementType : long
    {
        MONSTER_KILL_COUNT,
        SPECIPIC_MONSTER_KILL_COUNT,
        DEALING_DAMAGE,
        DEALING_DAMAGE_SPECIFIC_DEVICE,
        GET_GOLD,
    }

    public interface IAchievementType
    {
        AchievementType Achivement { get; }
        void ExtractAchiveValue(out object t);
    }
    
    public interface IGetGoldAchievementSystem : IReactValueGameSystem<long>, IAchievementType{}
    public interface IMonsterKillAchivementSystem : IReactValueGameSystem<long>, IAchievementType{}
    public interface IDealingDamageAchivementSystem : IReactValueGameSystem<long>, IAchievementType{}
    public interface IDealingDamageSpecificDeviceAchievementSystem : IReactCmdGameSystem<(string, long)>, IAchievementType {}
    public class DealingDamageSpecificDeviceAchievementSystem : IDealingDamageSpecificDeviceAchievementSystem
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ReactiveCommand<(string, long)> damageCount = new();
        public IReactiveCommand<(string, long)> reactiveCommand => damageCount;
        public AchievementType Achivement => AchievementType.DEALING_DAMAGE_SPECIFIC_DEVICE;

        private readonly Dictionary<string, long> damageFromDeviceId = new Dictionary<string, long>();
        
        public void InitGameSystem()
        {
            disposables.Add(damageCount);
            disposables.Add(damageCount.Subscribe(kv =>
            {
                if (damageFromDeviceId.ContainsKey(kv.Item1))
                {
                    damageFromDeviceId[kv.Item1] += kv.Item2;
                }
                else
                {
                    damageFromDeviceId.Add(kv.Item1, kv.Item2);
                }
            }));
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        
        public void ExtractAchiveValue(out object t)
        {
            t = damageFromDeviceId;
        }
    }

    public class DealingDamageAchivementSystem : IDealingDamageAchivementSystem
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        
        private ReactiveProperty<long> damageCount = new ReactiveProperty<long>();

        public IReactiveProperty<long> reactiveProperty => damageCount;
        public AchievementType Achivement => AchievementType.DEALING_DAMAGE;

        public void ExtractAchiveValue(out object t)
        {
            t = reactiveProperty.Value;
        }

        public void InitGameSystem()
        {
            disposables.Add(damageCount);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }

    public class MonsterKillAchivementSystem : IMonsterKillAchivementSystem
    {
        public AchievementType Achivement => AchievementType.MONSTER_KILL_COUNT;
        
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        
        private ReactiveProperty<long> monsterKillCount = new ReactiveProperty<long>();
        
        public IReactiveProperty<long> reactiveProperty => monsterKillCount;

        public void ExtractAchiveValue(out object t)
        {
            t = reactiveProperty.Value;
        }
        
        public void InitGameSystem()
        {
            disposables.Add(monsterKillCount);
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }

    public class GetGoldAchievementSystem : IGetGoldAchievementSystem
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        
        private ReactiveProperty<long> getGoldCount = new ReactiveProperty<long>();

        public AchievementType Achivement => AchievementType.GET_GOLD;
        public IReactiveProperty<long> reactiveProperty => getGoldCount;
        
        public void ExtractAchiveValue(out object t)
        {
            t = reactiveProperty.Value;
        }
        
        public void InitGameSystem()
        {
            disposables.Add(getGoldCount);
            
            disposables.Add(MessageBroker.Default
                .Receive<KeyValuePair<string, uint>>()
                .Subscribe(message =>
                {
                    switch(message.Key)
                    {
                        case "addGold":
                            getGoldCount.Value += message.Value;
                            break;
                    }
                }));
        }

        public void RegistComponent(IComponent component, IEntity sourceEntity)
        {
            
        }

        public void UnRegistComponent(IEntity sourceEntity)
        {
            
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        
    }
}
