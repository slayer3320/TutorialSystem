using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// ITutorialTrigger 的序列化引用绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ITutorialTrigger), true)]
    public class TutorialTriggerDrawer : PropertyDrawer
    {
        private const float ClearButtonWidth = 56f;
        private const float FieldSpacing = 4f;

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
            var dropdownRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                position.width - EditorGUIUtility.labelWidth - 25,
                EditorGUIUtility.singleLineHeight
            );
            var clearRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

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

            if (GUI.Button(clearRect, "X"))
            {
                property.managedReferenceValue = null;
            }

            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                var childrenRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    position.width,
                    position.height - EditorGUIUtility.singleLineHeight - 2
                );

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
                        {
                            break;
                        }

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

            menu.AddSeparator(string.Empty);

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
            float yOffset = 0f;

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        break;
                    }

                    if (ShouldHideProperty(property, iterator))
                    {
                        continue;
                    }

                    float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propRect = new Rect(position.x, position.y + yOffset, position.width, propHeight);

                    // KeyPressTrigger 的 triggerKey 使用可搜索弹窗
                    if (property.managedReferenceValue is KeyPressTrigger && iterator.name == "triggerKey")
                    {
                        DrawKeyCodePicker(propRect, iterator);
                    }
                    else
                    {
                        EditorGUI.PropertyField(propRect, iterator, true);
                    }

                    yOffset += propHeight + 2f;
                }
                while (iterator.NextVisible(false));
            }
        }

        private static bool ShouldHideProperty(SerializedProperty parentProperty, SerializedProperty childProperty)
        {
            if (parentProperty.managedReferenceValue is KeyPressTrigger keyPressTrigger)
            {
                if (childProperty.name == "triggerKey" && keyPressTrigger.AnyKey)
                {
                    return true;
                }
            }

            return false;
        }

        private void DrawKeyCodePicker(Rect position, SerializedProperty keyProperty)
        {
            var labelContent = new GUIContent(keyProperty.displayName, keyProperty.tooltip);
            var contentRect = EditorGUI.PrefixLabel(position, labelContent);

            var keyRect = new Rect(
                contentRect.x,
                contentRect.y,
                contentRect.width - ClearButtonWidth - FieldSpacing,
                contentRect.height
            );
            var clearRect = new Rect(
                keyRect.xMax + FieldSpacing,
                contentRect.y,
                ClearButtonWidth,
                contentRect.height
            );

            var currentKey = (KeyCode)keyProperty.intValue;
            if (EditorGUI.DropdownButton(keyRect, new GUIContent(currentKey.ToString()), FocusType.Keyboard))
            {
                PopupWindow.Show(keyRect, new KeyCodeSearchPopup(keyProperty));
            }

            if (GUI.Button(clearRect, "None"))
            {
                keyProperty.intValue = (int)KeyCode.None;
                keyProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private sealed class KeyCodeSearchPopup : PopupWindowContent
        {
            private static readonly KeyCode[] QuickKeys =
            {
                KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
                KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
                KeyCode.Space, KeyCode.Return, KeyCode.Escape, KeyCode.Tab
            };

            private static readonly KeyCode[] AllKeys = Enum.GetValues(typeof(KeyCode))
                .Cast<KeyCode>()
                .OrderBy(k => k.ToString())
                .ToArray();

            private readonly UnityEngine.Object targetObject;
            private readonly string propertyPath;
            private readonly KeyCode currentKey;

            private readonly SearchField searchField = new SearchField();
            private Vector2 scroll;
            private string searchText = string.Empty;

            public KeyCodeSearchPopup(SerializedProperty property)
            {
                targetObject = property.serializedObject.targetObject;
                propertyPath = property.propertyPath;
                currentKey = (KeyCode)property.intValue;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(300f, 360f);
            }

            public override void OnGUI(Rect rect)
            {
                if (targetObject == null)
                {
                    EditorGUILayout.HelpBox("目标对象已失效", MessageType.Warning);
                    return;
                }

                DrawQuickKeySection();
                GUILayout.Space(4f);

                // 使用自动布局，避免和常用按键区域重叠
                var searchRect = GUILayoutUtility.GetRect(
                    0f,
                    10000f,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight
                );
                searchText = searchField.OnGUI(searchRect, searchText);

                GUILayout.Space(4f);
                DrawKeyList();
            }

            private void DrawQuickKeySection()
            {
                EditorGUILayout.LabelField("常用按键", EditorStyles.boldLabel);

                const int columnCount = 4;
                for (int i = 0; i < QuickKeys.Length; i += columnCount)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < columnCount && i + j < QuickKeys.Length; j++)
                    {
                        DrawQuickKeyButton(QuickKeys[i + j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            private void DrawQuickKeyButton(KeyCode key)
            {
                bool isSelected = currentKey == key;
                string text = isSelected ? $"* {key}" : key.ToString();

                if (GUILayout.Button(text, EditorStyles.miniButton, GUILayout.Height(20f)))
                {
                    SetKey(key);
                }
            }

            private void DrawKeyList()
            {
                var style = new GUIStyle(EditorStyles.miniButtonLeft)
                {
                    alignment = TextAnchor.MiddleLeft
                };

                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));

                bool hasMatch = false;
                foreach (var key in AllKeys)
                {
                    if (!IsMatch(key, searchText))
                    {
                        continue;
                    }

                    hasMatch = true;
                    string keyLabel = key == currentKey ? $"* {key}" : key.ToString();
                    if (GUILayout.Button(keyLabel, style, GUILayout.Height(20f)))
                    {
                        SetKey(key);
                    }
                }

                if (!hasMatch)
                {
                    EditorGUILayout.HelpBox("没有匹配的按键", MessageType.Info);
                }

                EditorGUILayout.EndScrollView();
            }

            private static bool IsMatch(KeyCode key, string keyword)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return true;
                }

                return key.ToString().IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            private void SetKey(KeyCode key)
            {
                var serializedObject = new SerializedObject(targetObject);
                var property = serializedObject.FindProperty(propertyPath);
                if (property == null)
                {
                    return;
                }

                property.intValue = (int)key;
                serializedObject.ApplyModifiedProperties();
                editorWindow.Close();
            }
        }
    }
}
