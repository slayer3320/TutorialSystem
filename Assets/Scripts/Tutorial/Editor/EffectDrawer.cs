#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// IEffect PropertyDrawer - 支持在Inspector中选择Effect类型，防止重复添加
    /// </summary>
    [CustomPropertyDrawer(typeof(IEffect), true)]
    public class EffectDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Type> EffectTypes = new Dictionary<string, Type>
        {
            { "Floating Effect", typeof(FloatingEffect) },
            { "Fade In Effect", typeof(FadeInEffect) },
            { "Pulse Effect", typeof(PulseEffect) }
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
            string currentTypeName = "None";
            if (property.managedReferenceValue != null)
            {
                var currentType = property.managedReferenceValue.GetType();
                currentTypeName = EffectTypes.FirstOrDefault(x => x.Value == currentType).Key ?? currentType.Name;
            }

            // 折叠按钮
            if (property.managedReferenceValue != null)
            {
                foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none);
            }

            EditorGUI.LabelField(labelRect, label);

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                ShowEffectMenu(property);
            }

            if (GUI.Button(clearRect, "×"))
            {
                property.managedReferenceValue = null;
            }

            // 绘制子属性
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

        private void ShowEffectMenu(SerializedProperty property)
        {
            // 获取同级Effect列表中已存在的类型
            var existingTypes = GetExistingEffectTypes(property);

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            foreach (var kvp in EffectTypes)
            {
                var type = kvp.Value;
                var name = kvp.Key;

                // 检查是否已存在该类型（排除当前项自身）
                bool isDuplicate = existingTypes.Contains(type) && 
                    (property.managedReferenceValue == null || property.managedReferenceValue.GetType() != type);

                if (isDuplicate)
                {
                    // 显示为禁用状态
                    menu.AddDisabledItem(new GUIContent(name + " (Already Added)"));
                }
                else
                {
                    menu.AddItem(new GUIContent(name), false, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// 获取列表中已存在的Effect类型
        /// </summary>
        private HashSet<Type> GetExistingEffectTypes(SerializedProperty property)
        {
            var types = new HashSet<Type>();

            // 获取父级列表属性
            string path = property.propertyPath;
            int arrayIndex = path.LastIndexOf(".Array.data[");
            if (arrayIndex < 0) return types;

            string arrayPath = path.Substring(0, arrayIndex);
            var arrayProperty = property.serializedObject.FindProperty(arrayPath);

            if (arrayProperty != null && arrayProperty.isArray)
            {
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    var element = arrayProperty.GetArrayElementAtIndex(i);
                    if (element.managedReferenceValue != null)
                    {
                        types.Add(element.managedReferenceValue.GetType());
                    }
                }
            }

            return types;
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
#endif
