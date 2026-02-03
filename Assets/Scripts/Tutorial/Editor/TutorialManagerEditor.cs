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
        private SerializedProperty debugModeProp;

        private bool showPrefabs = true;
        private bool showDebug = true;
        private bool showRuntimeInfo = true;

        private void OnEnable()
        {
            arrowPrefabProp = serializedObject.FindProperty("arrowPrefab");
            popupPrefabProp = serializedObject.FindProperty("popupPrefab");
            uiContainerProp = serializedObject.FindProperty("uiContainer");
            targetCanvasProp = serializedObject.FindProperty("targetCanvas");
            debugModeProp = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawToolbar();
            EditorGUILayout.Space(5);

            // UI 设置
            showPrefabs = EditorGUILayout.BeginFoldoutHeaderGroup(showPrefabs, "UI 设置");
            if (showPrefabs)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(arrowPrefabProp, new GUIContent("箭头预制体"));
                EditorGUILayout.PropertyField(popupPrefabProp, new GUIContent("弹窗预制体"));
                EditorGUILayout.PropertyField(uiContainerProp, new GUIContent("UI 容器"));
                EditorGUILayout.PropertyField(targetCanvasProp, new GUIContent("目标 Canvas"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);

            // 调试设置
            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "调试设置");
            if (showDebug)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(debugModeProp, new GUIContent("调试模式"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // 运行时信息
            if (Application.isPlaying)
            {
                EditorGUILayout.Space(5);
                showRuntimeInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeInfo, "运行时信息");
                if (showRuntimeInfo)
                    DrawRuntimeInfo();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("调试窗口", EditorStyles.toolbarButton))
                TutorialDebugWindow.ShowWindow();

            GUILayout.FlexibleSpace();

            if (Application.isPlaying)
            {
                var manager = target as TutorialManager;
                if (manager != null && manager.IsRunning)
                {
                    if (GUILayout.Button("停止", EditorStyles.toolbarButton))
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
                EditorGUILayout.LabelField("状态: 运行中", statusStyle);
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.LabelField("当前教程", manager.CurrentConfig?.tutorialName ?? "无");
                EditorGUILayout.LabelField("当前阶段", 
                    $"{manager.CurrentPhase?.phaseName ?? "无"} ({manager.CurrentPhaseIndex + 1}/{manager.CurrentConfig?.phases.Count ?? 0})");
                EditorGUILayout.LabelField("当前步骤", 
                    $"{manager.CurrentStep?.stepName ?? "无"} ({manager.CurrentStepIndex + 1}/{manager.CurrentPhase?.steps.Count ?? 0})");

                EditorGUILayout.Space(5);
                var progress = manager.Progress;
                var rect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(rect, progress, $"进度: {progress:P0}");

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("上一步"))
                    manager.PrevStep();
                if (GUILayout.Button("下一步"))
                    manager.NextStep();
                if (GUILayout.Button("下一阶段"))
                    manager.NextPhase();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                statusStyle.normal.textColor = Color.gray;
                EditorGUILayout.LabelField("状态: 已停止", statusStyle);
            }

            EditorGUILayout.EndVertical();

            if (manager.IsRunning)
                Repaint();
        }
    }
}
