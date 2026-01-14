using Game.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Game.ECS.Editor
{
    [CustomEditor(typeof(StartMapInfoImplement))]
    public class StartMapInfoImplementEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var linkedObject = target as StartMapInfoImplement;

            var newTargetPosition = Handles.PositionHandle(linkedObject.StartPositionProperty.statusValue, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(linkedObject, "Change Start Position");
                linkedObject.StartPositionProperty.statusValue = newTargetPosition;
            }
        }
    }
}