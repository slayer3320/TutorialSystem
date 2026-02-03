#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// TutorialStep Custom Drawer
    /// Displays different inputs based on TutorialManager's global localization settings
    /// </summary>
    [CustomPropertyDrawer(typeof(TutorialStep))]
    public class TutorialStepDrawer : PropertyDrawer
    {
        private bool foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool useLocalization = GetGlobalLocalizationSetting();

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            // 获取步骤名称用于显示
            var stepNameProp = property.FindPropertyRelative("stepName");
            string stepName = stepNameProp != null && !string.IsNullOrEmpty(stepNameProp.stringValue) 
                ? stepNameProp.stringValue 
                : "Unnamed Step";
            
            foldout = EditorGUI.Foldout(headerRect, foldout, $"{label.text}: {stepName}", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                float yOffset = EditorGUIUtility.singleLineHeight + 2;

                // Basic Info
                yOffset = DrawPropertyField(position, property, "stepName", "Step Name", yOffset);
                yOffset = DrawPropertyField(position, property, "enabled", "Enabled", yOffset);

                // Content
                yOffset = DrawHeader(position, yOffset, "Content");
                if (useLocalization)
                {
                    yOffset = DrawPropertyField(position, property, "localizedTitle", "Title", yOffset);
                    yOffset = DrawPropertyField(position, property, "localizedContent", "Content", yOffset);
                }
                else
                {
                    yOffset = DrawPropertyField(position, property, "rawTitle", "Title", yOffset);
                    yOffset = DrawPropertyField(position, property, "rawContent", "Content", yOffset);
                }

                // Completion Trigger
                yOffset = DrawHeader(position, yOffset, "Completion Trigger");
                yOffset = DrawPropertyField(position, property, "completeTrigger", "Trigger", yOffset);

                // Module Configuration
                yOffset = DrawHeader(position, yOffset, "Module Configuration");
                yOffset = DrawPropertyField(position, property, "modules", "Module List", yOffset);

                // Events
                yOffset = DrawHeader(position, yOffset, "Events");
                yOffset = DrawPropertyField(position, property, "events", "Events", yOffset);

                // Settings
                yOffset = DrawHeader(position, yOffset, "Settings");
                yOffset = DrawPropertyField(position, property, "pauseOnEnter", "Pause On Enter", yOffset);
                yOffset = DrawPropertyField(position, property, "resumeOnExit", "Resume On Exit", yOffset);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!foldout)
                return EditorGUIUtility.singleLineHeight;

            bool useLocalization = GetGlobalLocalizationSetting();
            float height = EditorGUIUtility.singleLineHeight + 2;

            // 基本信息
            height += GetPropertyHeight(property, "stepName");
            height += GetPropertyHeight(property, "enabled");

            // 内容 header
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

            // 完成触发器 header + field
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "completeTrigger");

            // 模块配置 header + field
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "modules");

            // 事件 header + field
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "events");

            // 设置 header + fields
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "pauseOnEnter");
            height += GetPropertyHeight(property, "resumeOnExit");

            return height;
        }

        private bool GetGlobalLocalizationSetting()
        {
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
