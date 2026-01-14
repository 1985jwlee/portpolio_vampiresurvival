using Reflex;
using Reflex.Scripts;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class ProjectInstaller : Installer
    {
        public TableDataHolder tableDataHolder;

        public override void InstallBindings(Container container)
        {
            Application.targetFrameRate = 60;
            container.BindInstance(tableDataHolder);
            container.BindSingleton<ProjectContextModel, ProjectContextModel>();
            OnBinded(container);
        }

        private static void OnBinded(Container container)
        {
            var tableDataHolder = container.Resolve<TableDataHolder>();
            tableDataHolder.LoadCSV();
        }
    }
}
