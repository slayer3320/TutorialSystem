#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// PopupModule Custom Drawer
    /// Displays different inputs based on TutorialManager's global localization settings
    /// </summary>
    [CustomPropertyDrawer(typeof(PopupModule))]
    public class PopupModuleDrawer : PropertyDrawer
    {
        private bool foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取全局本地化设置
            bool useLocalization = GetGlobalLocalizationSetting();

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(headerRect, foldout, label, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                float yOffset = EditorGUIUtility.singleLineHeight + 2;

                // Content Settings
                yOffset = DrawHeader(position, yOffset, "Content Settings");

                if (useLocalization)
                {
                    // Localization mode: show LocalizedString fields
                    yOffset = DrawPropertyField(position, property, "localizedTitle", "Title", yOffset);
                    yOffset = DrawPropertyField(position, property, "localizedContent", "Content", yOffset);
                }
                else
                {
                    // Non-localization mode: show regular string fields
                    yOffset = DrawPropertyField(position, property, "rawTitle", "Title", yOffset);
                    yOffset = DrawPropertyField(position, property, "rawContent", "Content", yOffset);
                }

                // Button Settings
                yOffset = DrawHeader(position, yOffset, "Button Settings");
                yOffset = DrawPropertyField(position, property, "showButton", "Show Button", yOffset);
                if (useLocalization)
                {
                    yOffset = DrawPropertyField(position, property, "buttonText", "Button Text", yOffset);
                }

                // Position Settings
                yOffset = DrawHeader(position, yOffset, "Position Settings");
                yOffset = DrawPropertyField(position, property, "position", "Position", yOffset);
                
                var positionProp = property.FindPropertyRelative("position");
                if (positionProp != null && (PopupPosition)positionProp.enumValueIndex == PopupPosition.Custom)
                {
                    yOffset = DrawPropertyField(position, property, "customPosition", "Custom Position", yOffset);
                }
                yOffset = DrawPropertyField(position, property, "offset", "Offset", yOffset);

                // Appearance Settings
                yOffset = DrawHeader(position, yOffset, "Appearance Settings");
                yOffset = DrawPropertyField(position, property, "width", "Width", yOffset);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!foldout)
                return EditorGUIUtility.singleLineHeight;

            bool useLocalization = GetGlobalLocalizationSetting();
            float height = EditorGUIUtility.singleLineHeight + 2; // Header

            // 内容设置 header
            height += EditorGUIUtility.singleLineHeight + 4;

            if (useLocalization)
            {
                height += GetPropertyHeight(property, "localizedTitle");
                height += GetPropertyHeight(property, "localizedContent");
            }
            else
            {
                height += GetPropertyHeight(property, "rawTitle");
                height += GetPropertyHeight(property, "rawContent");
            }

            // 按钮设置 header + fields
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "showButton");
            if (useLocalization)
            {
                height += GetPropertyHeight(property, "buttonText");
            }

            // 位置设置 header + fields
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "position");
            
            var positionProp = property.FindPropertyRelative("position");
            if (positionProp != null && (PopupPosition)positionProp.enumValueIndex == PopupPosition.Custom)
            {
                height += GetPropertyHeight(property, "customPosition");
            }
            height += GetPropertyHeight(property, "offset");

            // 外观设置 header + fields
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "width");

            return height;
        }

        private bool GetGlobalLocalizationSetting()
        {
            // 在编辑器中查找 TutorialManager
            var manager = Object.FindFirstObjectByType<TutorialManager>();
            if (manager != null)
            {
                var so = new SerializedObject(manager);
                var locProp = so.FindProperty("useLocalization");
                return locProp != null && locProp.boolValue;
            }
            return false;
        }

        private float DrawHeader(Rect position, float yOffset, string headerText)
        {
            var headerRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, headerText, EditorStyles.boldLabel);
            return yOffset + EditorGUIUtility.singleLineHeight + 4;
        }

        private float DrawPropertyField(Rect position, SerializedProperty parentProperty, string propertyName, string label, float yOffset)
        {
            var prop = parentProperty.FindPropertyRelative(propertyName);
            if (prop == null) return yOffset;

            float propHeight = EditorGUI.GetPropertyHeight(prop, true);
            var propRect = new Rect(position.x, position.y + yOffset, position.width, propHeight);
            EditorGUI.PropertyField(propRect, prop, new GUIContent(label), true);
            return yOffset + propHeight + 2;
        }

        private float GetPropertyHeight(SerializedProperty parentProperty, string propertyName)
        {
            var prop = parentProperty.FindPropertyRelative(propertyName);
            if (prop == null) return 0;
            return EditorGUI.GetPropertyHeight(prop, true) + 2;
        }
    }
}
#endif
