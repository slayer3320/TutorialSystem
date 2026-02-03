using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty arrowPrefabProp;
        private SerializedProperty popupPrefabProp;
        private SerializedProperty uiContainerProp;
        private SerializedProperty targetCanvasProp;
        private SerializedProperty useLocalizationProp;
        private SerializedProperty debugModeProp;

        private bool showPrefabs = true;
        private bool showLocalization = true;
        private bool showDebug = true;
        private bool showRuntimeInfo = true;

        private void OnEnable()
        {
            arrowPrefabProp = serializedObject.FindProperty("arrowPrefab");
            popupPrefabProp = serializedObject.FindProperty("popupPrefab");
            uiContainerProp = serializedObject.FindProperty("uiContainer");
            targetCanvasProp = serializedObject.FindProperty("targetCanvas");
            useLocalizationProp = serializedObject.FindProperty("useLocalization");
            debugModeProp = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawToolbar();
            EditorGUILayout.Space(5);

            // UI Settings
            showPrefabs = EditorGUILayout.BeginFoldoutHeaderGroup(showPrefabs, "UI Settings");
            if (showPrefabs)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(arrowPrefabProp, new GUIContent("Arrow Prefab"));
                EditorGUILayout.PropertyField(popupPrefabProp, new GUIContent("Popup Prefab"));
                EditorGUILayout.PropertyField(uiContainerProp, new GUIContent("UI Container"));
                EditorGUILayout.PropertyField(targetCanvasProp, new GUIContent("Target Canvas"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);

            // Localization Settings
            showLocalization = EditorGUILayout.BeginFoldoutHeaderGroup(showLocalization, "Localization Settings");
            if (showLocalization)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(useLocalizationProp, new GUIContent("Enable Localization"));
                if (useLocalizationProp.boolValue)
                {
                    EditorGUILayout.HelpBox("Localization enabled, PopupModule will use LocalizedString", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Localization disabled, PopupModule will use regular text", MessageType.Info);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);

            // Debug Settings
            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug Settings");
            if (showDebug)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(debugModeProp, new GUIContent("Debug Mode"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Runtime Info
            if (Application.isPlaying)
            {
                EditorGUILayout.Space(5);
                showRuntimeInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeInfo, "Runtime Info");
                if (showRuntimeInfo)
                    DrawRuntimeInfo();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Debug Window", EditorStyles.toolbarButton))
                TutorialDebugWindow.ShowWindow();

            GUILayout.FlexibleSpace();

            if (Application.isPlaying)
            {
                var manager = target as TutorialManager;
                if (manager != null && manager.IsRunning)
                {
                    if (GUILayout.Button("Stop", EditorStyles.toolbarButton))
                        manager.StopTutorial();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawRuntimeInfo()
        {
            var manager = target as TutorialManager;
            if (manager == null) return;

            EditorGUILayout.BeginVertical("box");

            var statusStyle = new GUIStyle(EditorStyles.label);
            statusStyle.fontStyle = FontStyle.Bold;

            if (manager.IsRunning)
            {
                statusStyle.normal.textColor = Color.green;
                EditorGUILayout.LabelField("Status: Running", statusStyle);
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.LabelField("Current Tutorial", manager.CurrentConfig?.tutorialName ?? "None");
                EditorGUILayout.LabelField("Current Phase", 
                    $"{manager.CurrentPhase?.phaseName ?? "None"} ({manager.CurrentPhaseIndex + 1}/{manager.CurrentConfig?.phases.Count ?? 0})");
                EditorGUILayout.LabelField("Current Step", 
                    $"{manager.CurrentStep?.stepName ?? "None"} ({manager.CurrentStepIndex + 1}/{manager.CurrentPhase?.steps.Count ?? 0})");

                EditorGUILayout.Space(5);
                var progress = manager.Progress;
                var rect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(rect, progress, $"Progress: {progress:P0}");

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous"))
                    manager.PrevStep();
                if (GUILayout.Button("Next"))
                    manager.NextStep();
                if (GUILayout.Button("Next Phase"))
                    manager.NextPhase();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                statusStyle.normal.textColor = Color.gray;
                EditorGUILayout.LabelField("Status: Stopped", statusStyle);
            }

            EditorGUILayout.EndVertical();

            if (manager.IsRunning)
                Repaint();
        }
    }
}
