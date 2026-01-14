using Game.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventMapSettingImplement))]
public class EventMapSettingImplementEditor : UnityEditor.Editor
{
    public void OnSceneGUI()
    {
        var linkedObject = target as EventMapSettingImplement;

        if (linkedObject.IsEditingProperty.statusValue == false)
            return;

        var teleportSetting = linkedObject.TeleportSpawnDataProperty.statusValue;
        if (teleportSetting.shouldSpawnTeleport)
        {
            teleportSetting.spawnPosition = (Vector2)Handles.PositionHandle(teleportSetting.spawnPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Change Teleport Settings");
                linkedObject.TeleportSpawnDataProperty.statusValue = teleportSetting;
            }
        }

        for (int i = 0; i < linkedObject.sanctumSpawnSettings.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            var newPosition = (Vector2)Handles.PositionHandle(linkedObject.sanctumSpawnSettings[i].statusValue.spawnPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Change Sanctum Spawn Position");
                linkedObject.sanctumSpawnSettings[i] = new SanctumSpawnData()
                {
                    statusValue = new SanctumSpawnDataSet()
                    {
                        SanctumType = linkedObject.sanctumSpawnSettings[i].statusValue.SanctumType,
                        spawnPosition = newPosition,
                    }
                };
            }
        }

        for (int i = 0; i < linkedObject.expSpawnSettings.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            var newPosition = (Vector2)Handles.PositionHandle(linkedObject.expSpawnSettings[i].statusValue.spawnPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Change Exp Spawn Position");
                linkedObject.expSpawnSettings[i] = new ExpSpawnData()
                {
                    statusValue = new ExpSpawnDataSet()
                    {
                        value = linkedObject.expSpawnSettings[i].statusValue.value,
                        spawnPosition = newPosition,
                    }
                };
            }
        }

        for (int i = 0; i < linkedObject.itemSpawnSettings.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            var newPosition = (Vector2)Handles.PositionHandle(linkedObject.itemSpawnSettings[i].statusValue.spawnPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Change Item Spawn Position");
                linkedObject.itemSpawnSettings[i] = new ItemSpawnData()
                {
                    statusValue = new ItemSpawnDataSet()
                    {
                        itemId = linkedObject.itemSpawnSettings[i].statusValue.itemId,
                        spawnPosition = newPosition,
                    }
                };
            }
        }
    }
}
