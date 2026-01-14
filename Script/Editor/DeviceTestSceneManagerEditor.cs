using UnityEngine;
using UnityEditor;
using static Game.ECS.DeviceTestSceneManager;
using System.Collections.Generic;
using System;

namespace Game.ECS.Editor
{
    [CustomEditor(typeof(DeviceTestSceneManager))]
    public class DeviceTestSceneManagerEditor : UnityEditor.Editor
    {
        bool showResetOption;
        public TestCharacterSelection characterSelection;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DeviceTestSceneManager manager = (DeviceTestSceneManager)target;

            EditorGUILayout.HelpBox("Monster Spawns는 Timeline Table을 설정하지 않았을 때만 동작합니다.", MessageType.Warning);

            if (GUILayout.Button("Reload scene", GUILayout.Height(50)))
                manager.OnClickReload();

            showResetOption = EditorGUILayout.BeginFoldoutHeaderGroup(showResetOption, "Options");
            if (showResetOption)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset characterInfo", GUILayout.Height(30)))
                    manager.OnClickResetCharacter();
                bool shouldSet = GUILayout.Button("Set character status as", GUILayout.Height(30));
                characterSelection = (TestCharacterSelection)EditorGUILayout.EnumPopup(characterSelection);

                if (shouldSet)
                    manager.OnClickSetCharacter(characterSelection);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    public class MyPropertyDrawer : PropertyDrawer
    {
        protected virtual string[] CommonPropertyNames => new string[] { };

        protected virtual System.Func<SerializedProperty, string[]>[] conditions => new System.Func<SerializedProperty, string[]>[] { };

        protected virtual IConditioner[] conditioners => new IConditioner[] { };

        string[] CheckAllConditions(SerializedProperty property, string member)
        {
            var targetProperty = property.FindPropertyRelative(member);
            List<string> result = new();
            foreach (var condition in conditioners)
                result.AddRange(condition.Do(targetProperty));
            return result.ToArray();
        }

        Node[] GetEnumSpecificPropertyNames(SerializedProperty property)
        {
            var commonMembers = CommonPropertyNames;

            List<Node> result = new();
            foreach (var member in commonMembers)
                result.Add(DfsBuildProperty(property, member));

            return result.ToArray();
        }

        Node DfsBuildProperty(SerializedProperty property, string member)
        {
            var childs = CheckAllConditions(property, member);
            Node result = new();
            result.member = member;
            result.childs = new();
            foreach (var child in childs)
                result.childs.Add(DfsBuildProperty(property, child));
            return result;
        }

        private float GetPropertyHeight(SerializedProperty property, Node node)
        {
            float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative(node.member));
            foreach (var child in node.childs)
                height += GetPropertyHeight(property, child);

            return height;
        }

        private float PropertyField(SerializedProperty property, Node node, Rect position)
        {
            var member = property.FindPropertyRelative(node.member);
            var height = EditorGUI.GetPropertyHeight(member);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, height), member);

            Rect newRect = position;
            newRect.y += height;

            EditorGUI.indentLevel++;
            foreach (var child in node.childs)
                newRect.y += PropertyField(property, child, newRect);
            EditorGUI.indentLevel--;

            return newRect.y - position.y;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                Node[] nodes = GetEnumSpecificPropertyNames(property);
                var height = EditorGUIUtility.singleLineHeight;
                foreach (var node in nodes)
                    height += GetPropertyHeight(property, node);
                return height;

            }
            else
                return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);
            position.y += EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                Node[] nodes = GetEnumSpecificPropertyNames(property);
                foreach (var node in nodes)
                    position.y += PropertyField(property, node, position);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        struct Node
        {
            public string member;
            public List<Node> childs;
        }

        public interface IConditioner
        {
            public string[] Do(SerializedProperty property);
        }

        public class Conditioner<T> : IConditioner where T : System.Enum
        {
            Func<T, string[]> _condition;

            public Conditioner(Func<T, string[]> condition)
            {
                _condition = condition;
            }

            public string[] Do(SerializedProperty property)
            {
                string targetPropertyName = nameof(StartDevice.deviceSelection);
                if (property.name == targetPropertyName)
                {
                    var flag = (T)(object)property.enumValueFlag;
                    return _condition(flag);
                }
                else
                    return new string[] { };
            }
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(StartDevice))]
    public class StartDevicePropertyDrawer : MyPropertyDrawer
    {
        protected override string[] CommonPropertyNames => new string[]
        {
            nameof(StartDevice.deviceSelection),
            nameof(StartDevice.level),
        };

        protected override IConditioner[] conditioners => new IConditioner[]
        {
            new Conditioner<TestDeviceSelection>(PropertyNamesOfDeviceSelection),
        };

        static string[] PropertyNamesOfDeviceSelection(TestDeviceSelection propertyName) => propertyName switch
        {
            TestDeviceSelection.아이디로_고르기 => new string[] { nameof(StartDevice.arcaneDeviceType), nameof(StartDevice.deviceId), },
            _ => new string[] { },
        };
    }
}
