using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace Game.ECS.Editor
{
    [CustomEditor(typeof(EventMapTriggerSettingImplement))]
    public class EventMapTriggerSettingImplementEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var linkedObject = target as EventMapTriggerSettingImplement;

            if (linkedObject.IsEditingProperty.statusValue == false)
                return;

            var triggerSetting = linkedObject.TriggerSettingProperty.statusValue;

            if (triggerSetting.isEditing != false)
            {
                EditorGUI.BeginChangeCheck();

                var baseColor = Color.white;
                triggerSetting.position = (Vector2)Handles.PositionHandle(triggerSetting.position, Quaternion.identity);
                switch (triggerSetting.eventTriggerShape)
                {
                    case EventTriggerShape.Circle:
                        {
                            Handles.color = baseColor;
                            triggerSetting.radius = Handles.RadiusHandle(Quaternion.identity, triggerSetting.position, triggerSetting.radius);
                        }
                        break;
                    case EventTriggerShape.Rect:
                        {
                            var faceColor = baseColor.SetAlpha(0.1f);
                            var scaleHandlePosition = triggerSetting.position + triggerSetting.rectXMaxYMin;
                            triggerSetting.rectXMaxYMin = (Vector2)Handles.PositionHandle(scaleHandlePosition, Quaternion.identity) - triggerSetting.position;
                            var rect = new Rect(triggerSetting.position - triggerSetting.rectXMaxYMin, triggerSetting.rectXMaxYMin * 2);
                            Handles.DrawSolidRectangleWithOutline(rect, faceColor, baseColor);
                        }
                        break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(linkedObject, "Change Trigger Settings");
                    linkedObject.TriggerSettingProperty.statusValue = triggerSetting;
                }
            }

            var bossSetting = linkedObject.BossSpawnSettingProperty.statusValue;
            if(bossSetting.bossId != 0)
            {
                bossSetting.spawnPosition = (Vector2)Handles.PositionHandle(bossSetting.spawnPosition, Quaternion.identity);
                bossSetting.iconPosition = (Vector2)Handles.PositionHandle(bossSetting.iconPosition, Quaternion.identity);
                bossSetting.teleportPosition = (Vector2)Handles.PositionHandle(bossSetting.teleportPosition, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(linkedObject, "Change Boss Settings");
                    linkedObject.BossSpawnSettingProperty.statusValue = bossSetting;
                }
            }

            for (int i = 0; i < linkedObject.enemySpawnSettings.Count; i++)
            {
                EditorGUI.BeginChangeCheck();

                var newPosition = (Vector2)Handles.PositionHandle(linkedObject.enemySpawnSettings[i].statusValue.spawnPosition, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(linkedObject, "Change Enemy Spawn Position");
                    linkedObject.enemySpawnSettings[i] = new EnemySpawnData() { statusValue = new EnemySpawnDataSet(){
                        enemyId = linkedObject.enemySpawnSettings[i].statusValue.enemyId,
                        spawnPosition = newPosition,
                    }};
                }
            }

            for (int i = 0; i < linkedObject.dummySpawnSettings.Count; i++)
            {
                EditorGUI.BeginChangeCheck();

                var newPosition = (Vector2)Handles.PositionHandle(linkedObject.dummySpawnSettings[i].statusValue.spawnPosition, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(linkedObject, "Change Dummy Spawn Position");
                    linkedObject.dummySpawnSettings[i] = new DummySpawnData()
                    {
                        statusValue = new DummySpawnDataSet()
                        {
                            dummyId = linkedObject.dummySpawnSettings[i].statusValue.dummyId,
                            spawnPosition = newPosition,
                        }
                    };
                }
            }
        }
    }
}