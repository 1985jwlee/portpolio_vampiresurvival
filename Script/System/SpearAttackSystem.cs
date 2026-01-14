using System;
using System.Collections.Generic;
using UniRx;

namespace Game.ECS
{
    public interface ISpearAttackSystem : IOneWayReactCmdGameSystem
    {
        
    }
    
    public class SpearAttackSystem : ISpearAttackSystem
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ReactiveCommand<uint> onReceiveEntityEGID = new ReactiveCommand<uint>();

        public IReactiveCommand<uint> receiveEntityEGID => onReceiveEntityEGID;
        private readonly IEntityContainer entityContainer;
        private readonly WeaponFactory weaponFactory;

        public SpearAttackSystem(IEntityContainer _entityContainer, WeaponFactory _factory)
        {
            entityContainer = _entityContainer;
            weaponFactory = _factory;
        }
        
        public void InitGameSystem()
        {
            disposables.Add(onReceiveEntityEGID);
            
            disposables.Add(onReceiveEntityEGID.Subscribe(egid =>
            {
                if (entityContainer.GetEntity<AttackEntity,SpearAttackImplement> (egid, out AttackEntity entity, out var comp))
                {
                    var value = comp.attackSpearCountProperty.statusValue;
                    if (value > 0)
                    {
                        value -= 1;
                        comp.attackSpearCountProperty.statusValue = value;
                    }

                    if (value < 1)
                    {
                        weaponFactory.EnqueRecycle(entity, entity.SrcPathHashCode);
                    }
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
