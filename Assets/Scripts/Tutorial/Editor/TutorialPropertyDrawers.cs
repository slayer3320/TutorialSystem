using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// ITutorialTrigger 属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ITutorialTrigger), true)]
    public class TutorialTriggerDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Type> TriggerTypes = new Dictionary<string, Type>
        {
            { "手动触发", typeof(ManualTrigger) },
            { "计时器触发", typeof(TimerTrigger) },
            { "按钮点击触发", typeof(ButtonClickTrigger) },
            { "按键触发", typeof(KeyPressTrigger) },
            { "游戏事件触发", typeof(GameEventTrigger) }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                position.width - EditorGUIUtility.labelWidth - 25, EditorGUIUtility.singleLineHeight);
            var clearRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            // 获取当前类型名
            string currentTypeName = "无";
            if (property.managedReferenceValue != null)
            {
                var currentType = property.managedReferenceValue.GetType();
                currentTypeName = TriggerTypes.FirstOrDefault(x => x.Value == currentType).Key ?? currentType.Name;
            }

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                ShowTriggerMenu(property);
            }

            if (GUI.Button(clearRect, "×"))
            {
                property.managedReferenceValue = null;
            }

            // 绘制属性
            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                var childrenRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, 
                    position.width, position.height - EditorGUIUtility.singleLineHeight - 2);
                
                DrawChildProperties(childrenRect, property);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                height += 2;
                var iterator = property.Copy();
                var endProperty = property.GetEndProperty();
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (SerializedProperty.EqualContents(iterator, endProperty))
                            break;
                        height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
                    }
                    while (iterator.NextVisible(false));
                }
            }

            return height;
        }

        private void ShowTriggerMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("无"), property.managedReferenceValue == null, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            foreach (var kvp in TriggerTypes)
            {
                var type = kvp.Value;
                var name = kvp.Key;
                menu.AddItem(new GUIContent(name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private void DrawChildProperties(Rect position, SerializedProperty property)
        {
            var iterator = property.Copy();
            var endProperty = property.GetEndProperty();
            float yOffset = 0;

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                        break;
                    
                    var propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propRect = new Rect(position.x, position.y + yOffset, position.width, propHeight);
                    EditorGUI.PropertyField(propRect, iterator, true);
                    yOffset += propHeight + 2;
                }
                while (iterator.NextVisible(false));
            }
        }
    }

    /// <summary>
    /// ITutorialModule 属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ITutorialModule), true)]
    public class TutorialModuleDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Type> ModuleTypes = new Dictionary<string, Type>
        {
            { "弹窗模块", typeof(PopupModule) },
            { "箭头模块", typeof(ArrowModule) },
            { "高亮模块", typeof(HighlightModule) }
        };

        private bool foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var foldoutRect = new Rect(position.x, position.y, 15, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(position.x + 15, position.y, EditorGUIUtility.labelWidth - 15, EditorGUIUtility.singleLineHeight);
            var dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                position.width - EditorGUIUtility.labelWidth - 25, EditorGUIUtility.singleLineHeight);
            var clearRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);

            // 获取当前类型名
            string currentTypeName = "无";
            if (property.managedReferenceValue != null)
            {
                var module = property.managedReferenceValue as ITutorialModule;
                currentTypeName = module?.ModuleName ?? property.managedReferenceValue.GetType().Name;
            }

            // 折叠按钮
            if (property.managedReferenceValue != null)
            {
                foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none);
            }

            EditorGUI.LabelField(labelRect, label);

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                ShowModuleMenu(property);
            }

            if (GUI.Button(clearRect, "×"))
            {
                property.managedReferenceValue = null;
            }

            // 绘制属性
            if (property.managedReferenceValue != null && foldout)
            {
                EditorGUI.indentLevel++;
                var childrenRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, 
                    position.width, position.height - EditorGUIUtility.singleLineHeight - 2);
                
                DrawChildProperties(childrenRect, property);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null && foldout)
            {
                height += 2;
                var iterator = property.Copy();
                var endProperty = property.GetEndProperty();
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (SerializedProperty.EqualContents(iterator, endProperty))
                            break;
                        height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
                    }
                    while (iterator.NextVisible(false));
                }
            }

            return height;
        }

        private void ShowModuleMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("无"), property.managedReferenceValue == null, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            foreach (var kvp in ModuleTypes)
            {
                var type = kvp.Value;
                var name = kvp.Key;
                menu.AddItem(new GUIContent(name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private void DrawChildProperties(Rect position, SerializedProperty property)
        {
            var iterator = property.Copy();
            var endProperty = property.GetEndProperty();
            float yOffset = 0;

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                        break;
                    
                    var propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propRect = new Rect(position.x, position.y + yOffset, position.width, propHeight);
                    EditorGUI.PropertyField(propRect, iterator, true);
                    yOffset += propHeight + 2;
                }
                while (iterator.NextVisible(false));
            }
        }
    }
}
