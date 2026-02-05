#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// SpriteModule专属Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteModule))]
    public class SpriteModuleDrawer : PropertyDrawer
    {
        private bool foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(headerRect, foldout, label, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                float yOffset = EditorGUIUtility.singleLineHeight + 2;

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

                // Size Properties
                yOffset = DrawHeader(position, yOffset, "Size Properties");
                yOffset = DrawPropertyField(position, property, "sizeType", "Size Type", yOffset);

                var sizeTypeProp = property.FindPropertyRelative("sizeType");
                if (sizeTypeProp != null && (ModuleSizeType)sizeTypeProp.enumValueIndex == ModuleSizeType.CustomSize)
                {
                    yOffset = DrawPropertyField(position, property, "customSize", "Custom Size", yOffset);
                }

                // Sprite Properties
                yOffset = DrawHeader(position, yOffset, "Sprite Properties");
                yOffset = DrawPropertyField(position, property, "sprite", "Sprite", yOffset);
                yOffset = DrawPropertyField(position, property, "color", "Color", yOffset);
                yOffset = DrawPropertyField(position, property, "preserveAspect", "Preserve Aspect", yOffset);

                // Effects
                yOffset = DrawPropertyField(position, property, "effectSettings", "", yOffset);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!foldout)
                return EditorGUIUtility.singleLineHeight;

            float height = EditorGUIUtility.singleLineHeight + 2;

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

            // Size Properties header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "sizeType");

            var sizeTypeProp = property.FindPropertyRelative("sizeType");
            if (sizeTypeProp != null && (ModuleSizeType)sizeTypeProp.enumValueIndex == ModuleSizeType.CustomSize)
            {
                height += GetPropertyHeight(property, "customSize");
            }

            // Sprite Properties header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "sprite");
            height += GetPropertyHeight(property, "color");
            height += GetPropertyHeight(property, "preserveAspect");

            // Effects
            height += GetPropertyHeight(property, "effectSettings");

            return height;
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
