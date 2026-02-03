using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    [CustomEditor(typeof(TutorialRunner))]
    public class TutorialRunnerEditor : UnityEditor.Editor
    {
        private SerializedProperty configProp;
        private SerializedProperty autoStartOnEnableProp;

        private void OnEnable()
        {
            configProp = serializedObject.FindProperty("config");
            autoStartOnEnableProp = serializedObject.FindProperty("autoStartOnEnable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var runner = target as TutorialRunner;

            // 教程配置
            EditorGUILayout.PropertyField(configProp, new GUIContent("教程配置"), true);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(autoStartOnEnableProp, new GUIContent("启用时自动开始"));

            EditorGUILayout.Space(10);

            // 运行控制
            DrawRuntimeControls(runner);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && runner.IsRunning)
                Repaint();
        }

        private void DrawRuntimeControls(TutorialRunner runner)
        {
            EditorGUILayout.LabelField("运行控制", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            var phases = configProp.FindPropertyRelative("phases");
            bool hasConfig = phases != null && phases.arraySize > 0;

            if (!hasConfig)
            {
                EditorGUILayout.HelpBox("请配置教程内容", MessageType.Warning);
            }
            else if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("在 Play 模式下可以控制教程", MessageType.Info);
            }
            else
            {
                if (!runner.IsRunning)
                {
                    if (GUILayout.Button("开始教程", GUILayout.Height(30)))
                        runner.StartTutorial();
                }
                else
                {
                    var manager = TutorialManager.Instance;
                    if (manager != null)
                    {
                        EditorGUILayout.LabelField("当前阶段", manager.CurrentPhase?.phaseName ?? "无");
                        EditorGUILayout.LabelField("当前步骤", manager.CurrentStep?.stepName ?? "无");
                        EditorGUILayout.LabelField("进度", $"{manager.Progress * 100:F0}%");
                    }

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("停止"))
                        runner.StopTutorial();
                    if (GUILayout.Button("跳过"))
                        runner.SkipTutorial();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("上一步"))
                        runner.PrevStep();
                    if (GUILayout.Button("下一步"))
                        runner.NextStep();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
