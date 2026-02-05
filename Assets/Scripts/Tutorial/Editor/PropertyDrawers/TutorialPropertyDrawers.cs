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
            { "Manual Trigger", typeof(ManualTrigger) },
            { "Timer Trigger", typeof(TimerTrigger) },
            { "Button Click Trigger", typeof(ButtonClickTrigger) },
            { "Key Press Trigger", typeof(KeyPressTrigger) }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                position.width - EditorGUIUtility.labelWidth - 25, EditorGUIUtility.singleLineHeight);
            var clearRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            // Get current type name
            string currentTypeName = "None";
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

                        // 跳过隐藏的属性
                        if (ShouldHideProperty(property, iterator))
                        {
                            continue;
                        }

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
            menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
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

                    // 特殊处理：KeyPressTrigger 的 triggerKey 字段
                    if (ShouldHideProperty(property, iterator))
                    {
                        continue;
                    }

                    var propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propRect = new Rect(position.x, position.y + yOffset, position.width, propHeight);
                    EditorGUI.PropertyField(propRect, iterator, true);
                    yOffset += propHeight + 2;
                }
                while (iterator.NextVisible(false));
            }
        }

        private bool ShouldHideProperty(SerializedProperty parentProperty, SerializedProperty childProperty)
        {
            // 如果是 KeyPressTrigger 类型
            if (parentProperty.managedReferenceValue is KeyPressTrigger keyPressTrigger)
            {
                // 如果 anyKey 为 true，隐藏 triggerKey 字段
                if (childProperty.name == "triggerKey" && keyPressTrigger.AnyKey)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
