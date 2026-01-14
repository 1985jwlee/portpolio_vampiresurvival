using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    
    public class PopRandomCirclePointsImplement : MonoBehaviour, IComponent, IReactiveCommandComponent<List<Vector2>>
    {
        private ReactiveCommand<List<Vector2>> popRandomPosition;
        public IReactiveCommand<List<Vector2>> reactiveCommand => popRandomPosition;

        public void PopRandomCirclePoint(Transform circleTr, int count)
        {
            PopRandomCirclePoint(circleTr.position, circleTr.localScale.x, count);
        }

        public void PopRandomCirclePoint(Vector2 center, float r, int count)
        {
            var output = new List<Vector2>();
            for (int i = 0; i < count; ++i)
            {
                var randomPos = ExtensionFunction.RandomCirclePosition(center, r);
                output.Add(randomPos);
            }
            reactiveCommand.Execute(output);
        }
        
        public void InitializeComponent()
        {
            if(popRandomPosition != null){popRandomPosition.Dispose();}
            popRandomPosition = new ReactiveCommand<List<Vector2>>();
        }

        
    }
}
