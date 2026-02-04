#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// HighlightModule专属Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(HighlightModule))]
    public class HighlightModuleDrawer : PropertyDrawer
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

                // Highlight Properties
                yOffset = DrawHeader(position, yOffset, "Highlight Properties");
                yOffset = DrawPropertyField(position, property, "highlightType", "Highlight Type", yOffset);
                yOffset = DrawPropertyField(position, property, "shape", "Shape", yOffset);
                yOffset = DrawPropertyField(position, property, "maskColor", "Mask Color", yOffset);
                yOffset = DrawPropertyField(position, property, "highlightColor", "Highlight Color", yOffset);
                yOffset = DrawPropertyField(position, property, "padding", "Padding", yOffset);
                yOffset = DrawPropertyField(position, property, "cornerRadius", "Corner Radius", yOffset);
                yOffset = DrawPropertyField(position, property, "blockRaycasts", "Block Raycasts", yOffset);
                yOffset = DrawPropertyField(position, property, "clickMaskToNext", "Click Mask To Next", yOffset);

                // Effects
                yOffset = DrawHeader(position, yOffset, "Effects");
                yOffset = DrawPropertyField(position, property, "serializedEffects", "Effects", yOffset);

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

            // Highlight Properties header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "highlightType");
            height += GetPropertyHeight(property, "shape");
            height += GetPropertyHeight(property, "maskColor");
            height += GetPropertyHeight(property, "highlightColor");
            height += GetPropertyHeight(property, "padding");
            height += GetPropertyHeight(property, "cornerRadius");
            height += GetPropertyHeight(property, "blockRaycasts");
            height += GetPropertyHeight(property, "clickMaskToNext");

            // Effects header
            height += EditorGUIUtility.singleLineHeight + 4;
            height += GetPropertyHeight(property, "serializedEffects");

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
