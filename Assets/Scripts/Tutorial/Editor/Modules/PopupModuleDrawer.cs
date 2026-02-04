#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TutorialSystem;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// PopupModule专属Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(PopupModule))]
    public class PopupModuleDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取全局本地化设置
            bool useLocalization = GetGlobalLocalizationSetting();

            float yOffset = 0;

            // Base Properties
            yOffset = DrawHeader(position, yOffset, "Base Properties");
            yOffset = DrawPropertyField(position, property, "updateEveryFrame", "Update Every Frame", yOffset);
            yOffset = DrawPropertyField(position, property, "targetCanvas", "Target Canvas", yOffset);
            yOffset = DrawPropertyField(position, property, "positionMode", "Position Mode", yOffset);

            var positionModeProp = property.FindPropertyRelative("positionMode");
            if (positionModeProp != null && (ModulePositionMode)positionModeProp.enumValueIndex == ModulePositionMode.TransformBased)
            {
                yOffset = DrawPropertyField(position, property, "targetType", "Target Type", yOffset);
                yOffset = DrawPropertyField(position, property, "target", "Target", yOffset);
                yOffset = DrawPropertyField(position, property, "placementType", "Placement Type", yOffset);
                yOffset = DrawPropertyField(position, property, "constrainToCanvas", "Constrain To Canvas", yOffset);
            }
            else
            {
                yOffset = DrawPropertyField(position, property, "manualPosition", "Manual Position", yOffset);
            }
            yOffset = DrawPropertyField(position, property, "positionOffset", "Position Offset", yOffset);

            // Module Properties
            yOffset = DrawHeader(position, yOffset, "Module Properties");
            yOffset = DrawPropertyField(position, property, "sizeType", "Size Type", yOffset);
            
            var sizeTypeProp = property.FindPropertyRelative("sizeType");
            if (sizeTypeProp != null && (ModuleSizeType)sizeTypeProp.enumValueIndex == ModuleSizeType.CustomSize)
            {
                yOffset = DrawPropertyField(position, property, "customSize", "Custom Size", yOffset);
            }

            // Content Settings
            yOffset = DrawHeader(position, yOffset, "Content Settings");

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

            // Button Settings
            yOffset = DrawHeader(position, yOffset, "Button Settings");
            yOffset = DrawPropertyField(position, property, "showButton", "Show Button", yOffset);
            if (useLocalization)
            {
                yOffset = DrawPropertyField(position, property, "buttonText", "Button Text", yOffset);
            }

            // Effects
            yOffset = DrawHeader(position, yOffset, "Effects");
            yOffset = DrawPropertyField(position, property, "serializedEffects", "Effects", yOffset);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool useLocalization = GetGlobalLocalizationSetting();
            float height = 0;

            // Base Properties header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "updateEveryFrame");
            height += GetPropertyHeight(property, "targetCanvas");
            height += GetPropertyHeight(property, "positionMode");

            var positionModeProp = property.FindPropertyRelative("positionMode");
            if (positionModeProp != null && (ModulePositionMode)positionModeProp.enumValueIndex == ModulePositionMode.TransformBased)
            {
                height += GetPropertyHeight(property, "targetType");
                height += GetPropertyHeight(property, "target");
                height += GetPropertyHeight(property, "placementType");
                height += GetPropertyHeight(property, "constrainToCanvas");
            }
            else
            {
                height += GetPropertyHeight(property, "manualPosition");
            }
            height += GetPropertyHeight(property, "positionOffset");

            // Module Properties header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "sizeType");
            
            var sizeTypeProp = property.FindPropertyRelative("sizeType");
            if (sizeTypeProp != null && (ModuleSizeType)sizeTypeProp.enumValueIndex == ModuleSizeType.CustomSize)
            {
                height += GetPropertyHeight(property, "customSize");
            }

            // Content Settings header
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

            // Button Settings header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "showButton");
            if (useLocalization)
            {
                height += GetPropertyHeight(property, "buttonText");
            }

            // Effects header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "serializedEffects");

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
