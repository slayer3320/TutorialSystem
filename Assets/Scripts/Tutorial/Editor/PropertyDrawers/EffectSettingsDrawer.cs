#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// EffectSettings PropertyDrawer - 使用Toggle控制Effect的启用/禁用
    /// </summary>
    [CustomPropertyDrawer(typeof(EffectSettings))]
    public class EffectSettingsDrawer : PropertyDrawer
    {
        private const float ToggleWidth = 20f;
        private const float Spacing = 2f;

        private static readonly EffectInfo[] Effects = new EffectInfo[]
        {
            new EffectInfo("floatingEnabled", "floatingEffect", "Floating", "浮动效果"),
            new EffectInfo("fadeInEnabled", "fadeInEffect", "Fade In", "渐入效果"),
            new EffectInfo("pulseEnabled", "pulseEffect", "Pulse", "脉冲效果"),
            new EffectInfo("fadeOutEnabled", "fadeOutEffect", "Fade Out", "渐出效果")
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 计算启用的Effect数量
            int enabledCount = GetEnabledCount(property);
            string headerText = enabledCount > 0 ? $"Effects ({enabledCount})" : "Effects";

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, headerText, EditorStyles.boldLabel);

            float yOffset = EditorGUIUtility.singleLineHeight + Spacing;

            EditorGUI.indentLevel++;

            foreach (var effectInfo in Effects)
            {
                var enabledProp = property.FindPropertyRelative(effectInfo.EnabledField);
                var effectProp = property.FindPropertyRelative(effectInfo.EffectField);

                // Toggle行
                var toggleRect = new Rect(
                    position.x,
                    position.y + yOffset,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

                // 绘制Toggle + 标签
                var toggleContent = new GUIContent(effectInfo.DisplayName, effectInfo.Tooltip);
                enabledProp.boolValue = EditorGUI.ToggleLeft(toggleRect, toggleContent, enabledProp.boolValue);
                yOffset += EditorGUIUtility.singleLineHeight + Spacing;

                // 如果启用，显示Effect的属性
                if (enabledProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    yOffset += DrawEffectProperties(position, effectProp, yOffset);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + Spacing; // Header

            foreach (var effectInfo in Effects)
            {
                var enabledProp = property.FindPropertyRelative(effectInfo.EnabledField);
                height += EditorGUIUtility.singleLineHeight + Spacing; // Toggle行

                if (enabledProp.boolValue)
                {
                    var effectProp = property.FindPropertyRelative(effectInfo.EffectField);
                    height += GetEffectPropertiesHeight(effectProp);
                }
            }

            return height;
        }

        private int GetEnabledCount(SerializedProperty property)
        {
            int count = 0;
            foreach (var effectInfo in Effects)
            {
                var enabledProp = property.FindPropertyRelative(effectInfo.EnabledField);
                if (enabledProp.boolValue) count++;
            }
            return count;
        }

        private float DrawEffectProperties(Rect position, SerializedProperty effectProp, float yOffset)
        {
            float totalHeight = 0;
            var iterator = effectProp.Copy();
            var endProperty = effectProp.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                        break;

                    // 跳过非序列化字段
                    if (iterator.name.StartsWith("is") || iterator.name == "target")
                        continue;

                    var propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propRect = new Rect(
                        position.x,
                        position.y + yOffset + totalHeight,
                        position.width,
                        propHeight
                    );

                    EditorGUI.PropertyField(propRect, iterator, true);
                    totalHeight += propHeight + Spacing;
                }
                while (iterator.NextVisible(false));
            }

            return totalHeight;
        }

        private float GetEffectPropertiesHeight(SerializedProperty effectProp)
        {
            float height = 0;
            var iterator = effectProp.Copy();
            var endProperty = effectProp.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                        break;

                    // 跳过非序列化字段
                    if (iterator.name.StartsWith("is") || iterator.name == "target")
                        continue;

                    height += EditorGUI.GetPropertyHeight(iterator, true) + Spacing;
                }
                while (iterator.NextVisible(false));
            }

            return height;
        }

        private struct EffectInfo
        {
            public string EnabledField;
            public string EffectField;
            public string DisplayName;
            public string Tooltip;

            public EffectInfo(string enabledField, string effectField, string displayName, string tooltip)
            {
                EnabledField = enabledField;
                EffectField = effectField;
                DisplayName = displayName;
                Tooltip = tooltip;
            }
        }
    }
}
#endif
